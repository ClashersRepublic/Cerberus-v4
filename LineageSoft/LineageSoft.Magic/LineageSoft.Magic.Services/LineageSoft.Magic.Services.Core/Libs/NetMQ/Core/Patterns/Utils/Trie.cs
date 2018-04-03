/*
    Copyright (c) 2009-2011 250bpm s.r.o.
    Copyright (c) 2007-2009 iMatix Corporation
    Copyright (c) 2011-2012 Spotify AB
    Copyright (c) 2007-2015 Other contributors as noted in the AUTHORS file

    This file is part of 0MQ.

    0MQ is free software; you can redistribute it and/or modify it under
    the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation; either version 3 of the License, or
    (at your option) any later version.

    0MQ is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

namespace LineageSoft.Magic.Services.Core.Libs.NetMQ.Core.Patterns.Utils
{
    using System;
    using System.Diagnostics;
    using JetBrains.Annotations;

    internal class Trie
    {
        private int m_referenceCount;

        private byte m_minCharacter;
        private short m_count;
        private short m_liveNodes;

        public delegate void TrieDelegate([NotNull] byte[] data, int size, [CanBeNull] object arg);

        private Trie[] m_next;

        /// <summary>
        ///     Add key to the trie. Returns true if this is a new item in the trie
        ///     rather than a duplicate.
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="start"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public bool Add([NotNull] byte[] prefix, int start, int size)
        {
            // We are at the node corresponding to the prefix. We are done.
            if (size == 0)
            {
                ++this.m_referenceCount;
                return this.m_referenceCount == 1;
            }

            byte currentCharacter = prefix[start];
            if (currentCharacter < this.m_minCharacter || currentCharacter >= this.m_minCharacter + this.m_count)
            {
                // The character is out of range of currently handled
                // characters. We have to extend the table.
                if (this.m_count == 0)
                {
                    this.m_minCharacter = currentCharacter;
                    this.m_count = 1;
                    this.m_next = null;
                }
                else if (this.m_count == 1)
                {
                    byte oldc = this.m_minCharacter;
                    Trie oldp = this.m_next[0];
                    this.m_count = (short) ((this.m_minCharacter < currentCharacter ? currentCharacter - this.m_minCharacter : this.m_minCharacter - currentCharacter) + 1);
                    this.m_next = new Trie[this.m_count];
                    this.m_minCharacter = Math.Min(this.m_minCharacter, currentCharacter);
                    this.m_next[oldc - this.m_minCharacter] = oldp;
                }
                else if (this.m_minCharacter < currentCharacter)
                {
                    // The new character is above the current character range.
                    this.m_count = (short) (currentCharacter - this.m_minCharacter + 1);
                    this.m_next = this.m_next.Resize(this.m_count, true);
                }
                else
                {
                    // The new character is below the current character range.
                    this.m_count = (short) (this.m_minCharacter + this.m_count - currentCharacter);
                    this.m_next = this.m_next.Resize(this.m_count, false);
                    this.m_minCharacter = currentCharacter;
                }
            }

            // If next node does not exist, create one.
            if (this.m_count == 1)
            {
                if (this.m_next == null)
                {
                    this.m_next = new Trie[1];
                    this.m_next[0] = new Trie();
                    ++this.m_liveNodes;
                    //alloc_Debug.Assert(next.node);
                }

                return this.m_next[0].Add(prefix, start + 1, size - 1);
            }

            if (this.m_next[currentCharacter - this.m_minCharacter] == null)
            {
                this.m_next[currentCharacter - this.m_minCharacter] = new Trie();
                ++this.m_liveNodes;
                //alloc_Debug.Assert(next.table [c - min]);
            }

            return this.m_next[currentCharacter - this.m_minCharacter].Add(prefix, start + 1, size - 1);
        }


        /// <summary>
        ///     Remove key from the trie. Returns true if the item is actually
        ///     removed from the trie.
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="start"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public bool Remove([NotNull] byte[] prefix, int start, int size)
        {
            if (size == 0)
            {
                if (this.m_referenceCount == 0)
                {
                    return false;
                }

                this.m_referenceCount--;
                return this.m_referenceCount == 0;
            }

            byte currentCharacter = prefix[start];
            if (this.m_count == 0 || currentCharacter < this.m_minCharacter || currentCharacter >= this.m_minCharacter + this.m_count)
            {
                return false;
            }

            Trie nextNode = this.m_count == 1 ? this.m_next[0] : this.m_next[currentCharacter - this.m_minCharacter];

            if (nextNode == null)
            {
                return false;
            }

            bool wasRemoved = nextNode.Remove(prefix, start + 1, size - 1);

            if (nextNode.IsRedundant())
            {
                //delete next_node;
                Debug.Assert(this.m_count > 0);

                if (this.m_count == 1)
                {
                    this.m_next = null;
                    this.m_count = 0;
                    --this.m_liveNodes;
                    Debug.Assert(this.m_liveNodes == 0);
                }
                else
                {
                    this.m_next[currentCharacter - this.m_minCharacter] = null;
                    Debug.Assert(this.m_liveNodes > 1);
                    --this.m_liveNodes;

                    // Compact the table if possible
                    if (this.m_liveNodes == 1)
                    {
                        // If there's only one live node in the table we can
                        // switch to using the more compact single-node
                        // representation
                        Trie node = null;
                        for (short i = 0; i < this.m_count; ++i)
                        {
                            if (this.m_next[i] != null)
                            {
                                node = this.m_next[i];
                                this.m_minCharacter = (byte) (i + this.m_minCharacter);
                                break;
                            }
                        }

                        Debug.Assert(node != null);

                        this.m_next = null;
                        this.m_next = new[] {node};
                        this.m_count = 1;
                    }
                    else if (currentCharacter == this.m_minCharacter)
                    {
                        // We can compact the table "from the left"
                        byte newMin = this.m_minCharacter;
                        for (short i = 1; i < this.m_count; ++i)
                        {
                            if (this.m_next[i] != null)
                            {
                                newMin = (byte) (i + this.m_minCharacter);
                                break;
                            }
                        }

                        Debug.Assert(newMin != this.m_minCharacter);

                        Debug.Assert(newMin > this.m_minCharacter);
                        Debug.Assert(this.m_count > newMin - this.m_minCharacter);
                        this.m_count = (short) (this.m_count - (newMin - this.m_minCharacter));

                        this.m_next = this.m_next.Resize(this.m_count, false);

                        this.m_minCharacter = newMin;
                    }
                    else if (currentCharacter == this.m_minCharacter + this.m_count - 1)
                    {
                        // We can compact the table "from the right"
                        short newCount = this.m_count;
                        for (short i = 1; i < this.m_count; ++i)
                        {
                            if (this.m_next[this.m_count - 1 - i] != null)
                            {
                                newCount = (short) (this.m_count - i);
                                break;
                            }
                        }

                        Debug.Assert(newCount != this.m_count);
                        this.m_count = newCount;

                        this.m_next = this.m_next.Resize(this.m_count, true);
                    }
                }
            }

            return wasRemoved;
        }

        /// <summary>
        ///     Check whether particular key is in the trie.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public bool Check([NotNull] byte[] data, int offset, int size)
        {
            // This function is on critical path. It deliberately doesn't use
            // recursion to get a bit better performance.
            Trie current = this;
            int start = offset;
            while (true)
            {
                // We've found a corresponding subscription!
                if (current.m_referenceCount > 0)
                {
                    return true;
                }

                // We've checked all the data and haven't found matching subscription.
                if (size == 0)
                {
                    return false;
                }

                // If there's no corresponding slot for the first character
                // of the prefix, the message does not match.
                byte character = data[start];
                if (character < current.m_minCharacter || character >= current.m_minCharacter + current.m_count)
                {
                    return false;
                }

                // Move to the next character.
                if (current.m_count == 1)
                {
                    current = current.m_next[0];
                }
                else
                {
                    current = current.m_next[character - current.m_minCharacter];

                    if (current == null)
                    {
                        return false;
                    }
                }

                start++;
                size--;
            }
        }

        // Apply the function supplied to each subscription in the trie.
        public void Apply([NotNull] TrieDelegate func, [CanBeNull] object arg)
        {
            this.ApplyHelper(null, 0, 0, func, arg);
        }

        private void ApplyHelper([NotNull] byte[] buffer, int bufferSize, int maxBufferSize, [NotNull] TrieDelegate func, [CanBeNull] object arg)
        {
            // If this node is a subscription, apply the function.
            if (this.m_referenceCount > 0)
            {
                func(buffer, bufferSize, arg);
            }

            // Adjust the buffer.
            if (bufferSize >= maxBufferSize)
            {
                maxBufferSize = bufferSize + 256;
                Array.Resize(ref buffer, maxBufferSize);
                Debug.Assert(buffer != null);
            }

            // If there are no subnodes in the trie, return.
            if (this.m_count == 0)
            {
                return;
            }

            // If there's one subnode (optimisation).
            if (this.m_count == 1)
            {
                buffer[bufferSize] = this.m_minCharacter;
                bufferSize++;
                this.m_next[0].ApplyHelper(buffer, bufferSize, maxBufferSize, func, arg);
                return;
            }

            // If there are multiple subnodes.
            for (short c = 0; c != this.m_count; c++)
            {
                buffer[bufferSize] = (byte) (this.m_minCharacter + c);
                if (this.m_next[c] != null)
                {
                    this.m_next[c].ApplyHelper(buffer, bufferSize + 1, maxBufferSize, func, arg);
                }
            }
        }

        private bool IsRedundant()
        {
            return this.m_referenceCount == 0 && this.m_liveNodes == 0;
        }
    }
}