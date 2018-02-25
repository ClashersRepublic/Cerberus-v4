/*
    Copyright (c) 2011 250bpm s.r.o.
    Copyright (c) 2011-2012 Spotify AB
    Copyright (c) 2011-2015 Other contributors as noted in the AUTHORS file

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

namespace ClashersRepublic.Magic.Services.Core.Libs.NetMQ.Core.Patterns.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using JetBrains.Annotations;

    /// <summary>
    ///     Multi-trie. Each node in the trie is a set of pointers to pipes.
    /// </summary>
    internal class MultiTrie
    {
        private HashSet<Pipe> m_pipes;

        private int m_minCharacter;
        private int m_count;
        private int m_liveNodes;
        private MultiTrie[] m_next;

        public delegate void MultiTrieDelegate([CanBeNull] Pipe pipe, [CanBeNull] byte[] data, int size, [CanBeNull] object arg);

        public MultiTrie()
        {
            this.m_minCharacter = 0;
            this.m_count = 0;
            this.m_liveNodes = 0;

            this.m_pipes = null;
            this.m_next = null;
        }

        /// <summary>
        ///     Add key to the trie. Returns true if it's a new subscription
        ///     rather than a duplicate.
        /// </summary>
        public bool Add([CanBeNull] byte[] prefix, int start, int size, [NotNull] Pipe pipe)
        {
            return this.AddHelper(prefix, start, size, pipe);
        }

        private bool AddHelper([CanBeNull] byte[] prefix, int start, int size, [NotNull] Pipe pipe)
        {
            // We are at the node corresponding to the prefix. We are done.
            if (size == 0)
            {
                bool result = this.m_pipes == null;

                if (this.m_pipes == null)
                {
                    this.m_pipes = new HashSet<Pipe>();
                }

                this.m_pipes.Add(pipe);
                return result;
            }

            Debug.Assert(prefix != null);

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
                    int oldc = this.m_minCharacter;
                    MultiTrie oldp = this.m_next[0];
                    this.m_count = (this.m_minCharacter < currentCharacter ? currentCharacter - this.m_minCharacter : this.m_minCharacter - currentCharacter) + 1;
                    this.m_next = new MultiTrie[this.m_count];
                    this.m_minCharacter = Math.Min(this.m_minCharacter, currentCharacter);
                    this.m_next[oldc - this.m_minCharacter] = oldp;
                }
                else if (this.m_minCharacter < currentCharacter)
                {
                    // The new character is above the current character range.
                    this.m_count = currentCharacter - this.m_minCharacter + 1;
                    this.m_next = this.m_next.Resize(this.m_count, true);
                }
                else
                {
                    // The new character is below the current character range.
                    this.m_count = this.m_minCharacter + this.m_count - currentCharacter;
                    this.m_next = this.m_next.Resize(this.m_count, false);
                    this.m_minCharacter = currentCharacter;
                }
            }

            // If next node does not exist, create one.
            if (this.m_count == 1)
            {
                if (this.m_next == null)
                {
                    this.m_next = new MultiTrie[1];
                    this.m_next[0] = new MultiTrie();
                    ++this.m_liveNodes;
                }

                return this.m_next[0].AddHelper(prefix, start + 1, size - 1, pipe);
            }

            if (this.m_next[currentCharacter - this.m_minCharacter] == null)
            {
                this.m_next[currentCharacter - this.m_minCharacter] = new MultiTrie();
                ++this.m_liveNodes;
            }

            return this.m_next[currentCharacter - this.m_minCharacter].AddHelper(prefix, start + 1, size - 1, pipe);
        }


        /// <summary>
        ///     Remove all subscriptions for a specific peer from the trie.
        ///     If there are no subscriptions left on some topics, invoke the
        ///     supplied callback function.
        /// </summary>
        /// <param name="pipe"></param>
        /// <param name="func"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public bool RemoveHelper([NotNull] Pipe pipe, [NotNull] MultiTrieDelegate func, [CanBeNull] object arg)
        {
            return this.RemoveHelper(pipe, EmptyArray<byte>.Instance, 0, 0, func, arg);
        }

        private bool RemoveHelper([NotNull] Pipe pipe, [NotNull] byte[] buffer, int bufferSize, int maxBufferSize, [NotNull] MultiTrieDelegate func, [CanBeNull] object arg)
        {
            // Remove the subscription from this node.
            if (this.m_pipes != null && this.m_pipes.Remove(pipe) && this.m_pipes.Count == 0)
            {
                func(pipe, buffer, bufferSize, arg);
                this.m_pipes = null;
            }

            // Adjust the buffer.
            if (bufferSize >= maxBufferSize)
            {
                maxBufferSize = bufferSize + 256;
                Array.Resize(ref buffer, maxBufferSize);
            }

            // If there are no subnodes in the trie, return.
            if (this.m_count == 0)
            {
                return true;
            }

            // If there's one subnode (optimisation).
            if (this.m_count == 1)
            {
                buffer[bufferSize] = (byte) this.m_minCharacter;
                bufferSize++;
                this.m_next[0].RemoveHelper(pipe, buffer, bufferSize, maxBufferSize, func, arg);

                // Prune the node if it was made redundant by the removal
                if (this.m_next[0].IsRedundant)
                {
                    this.m_next = null;
                    this.m_count = 0;
                    --this.m_liveNodes;
                    Debug.Assert(this.m_liveNodes == 0);
                }

                return true;
            }

            // If there are multiple subnodes.

            // New min non-null character in the node table after the removal
            int newMin = this.m_minCharacter + this.m_count - 1;

            // New max non-null character in the node table after the removal
            int newMax = this.m_minCharacter;

            for (int currentCharacter = 0; currentCharacter != this.m_count; currentCharacter++)
            {
                buffer[bufferSize] = (byte) (this.m_minCharacter + currentCharacter);
                if (this.m_next[currentCharacter] != null)
                {
                    this.m_next[currentCharacter].RemoveHelper(pipe, buffer, bufferSize + 1,
                        maxBufferSize, func, arg);

                    // Prune redundant nodes from the mtrie
                    if (this.m_next[currentCharacter].IsRedundant)
                    {
                        this.m_next[currentCharacter] = null;

                        Debug.Assert(this.m_liveNodes > 0);
                        --this.m_liveNodes;
                    }
                    else
                    {
                        // The node is not redundant, so it's a candidate for being
                        // the new min/max node.
                        //
                        // We loop through the node array from left to right, so the
                        // first non-null, non-redundant node encountered is the new
                        // minimum index. Conversely, the last non-redundant, non-null
                        // node encountered is the new maximum index.
                        if (currentCharacter + this.m_minCharacter < newMin)
                        {
                            newMin = currentCharacter + this.m_minCharacter;
                        }

                        if (currentCharacter + this.m_minCharacter > newMax)
                        {
                            newMax = currentCharacter + this.m_minCharacter;
                        }
                    }
                }
            }

            Debug.Assert(this.m_count > 1);

            // Free the node table if it's no longer used.
            if (this.m_liveNodes == 0)
            {
                this.m_next = null;
                this.m_count = 0;
            }
            // Compact the node table if possible
            else if (this.m_liveNodes == 1)
            {
                // If there's only one live node in the table we can
                // switch to using the more compact single-node
                // representation
                Debug.Assert(newMin == newMax);
                Debug.Assert(newMin >= this.m_minCharacter && newMin < this.m_minCharacter + this.m_count);

                MultiTrie node = this.m_next[newMin - this.m_minCharacter];

                Debug.Assert(node != null);

                this.m_next = null;
                this.m_next = new[] {node};
                this.m_count = 1;
                this.m_minCharacter = newMin;
            }
            else if (this.m_liveNodes > 1 && (newMin > this.m_minCharacter || newMax < this.m_minCharacter + this.m_count - 1))
            {
                Debug.Assert(newMax - newMin + 1 > 1);

                MultiTrie[] oldTable = this.m_next;
                Debug.Assert(newMin > this.m_minCharacter || newMax < this.m_minCharacter + this.m_count - 1);
                Debug.Assert(newMin >= this.m_minCharacter);
                Debug.Assert(newMax <= this.m_minCharacter + this.m_count - 1);
                Debug.Assert(newMax - newMin + 1 < this.m_count);
                this.m_count = newMax - newMin + 1;
                this.m_next = new MultiTrie[this.m_count];

                Array.Copy(oldTable, newMin - this.m_minCharacter, this.m_next, 0, this.m_count);

                this.m_minCharacter = newMin;
            }

            return true;
        }

        /// <summary>
        ///     Remove specific subscription from the trie. Return true is it was
        ///     actually removed rather than de-duplicated.
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="start"></param>
        /// <param name="size"></param>
        /// <param name="pipe"></param>
        /// <returns></returns>
        public bool Remove([NotNull] byte[] prefix, int start, int size, [NotNull] Pipe pipe)
        {
            return this.RemoveHelper(prefix, start, size, pipe);
        }

        private bool RemoveHelper([NotNull] byte[] prefix, int start, int size, [NotNull] Pipe pipe)
        {
            if (size == 0)
            {
                if (this.m_pipes != null)
                {
                    bool erased = this.m_pipes.Remove(pipe);
                    Debug.Assert(erased);
                    if (this.m_pipes.Count == 0)
                    {
                        this.m_pipes = null;
                    }
                }

                return this.m_pipes == null;
            }

            byte currentCharacter = prefix[start];
            if (this.m_count == 0 || currentCharacter < this.m_minCharacter || currentCharacter >= this.m_minCharacter + this.m_count)
            {
                return false;
            }

            MultiTrie nextNode = this.m_count == 1 ? this.m_next[0] : this.m_next[currentCharacter - this.m_minCharacter];

            if (nextNode == null)
            {
                return false;
            }

            bool ret = nextNode.RemoveHelper(prefix, start + 1, size - 1, pipe);
            if (nextNode.IsRedundant)
            {
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
                        int i;
                        for (i = 0; i < this.m_count; ++i)
                        {
                            if (this.m_next[i] != null)
                            {
                                break;
                            }
                        }

                        Debug.Assert(i < this.m_count);
                        this.m_minCharacter += i;
                        this.m_count = 1;
                        MultiTrie old = this.m_next[i];
                        this.m_next = new[] {old};
                    }
                    else if (currentCharacter == this.m_minCharacter)
                    {
                        // We can compact the table "from the left"
                        int i;
                        for (i = 1; i < this.m_count; ++i)
                        {
                            if (this.m_next[i] != null)
                            {
                                break;
                            }
                        }

                        Debug.Assert(i < this.m_count);
                        this.m_minCharacter += i;
                        this.m_count -= i;
                        this.m_next = this.m_next.Resize(this.m_count, false);
                    }
                    else if (currentCharacter == this.m_minCharacter + this.m_count - 1)
                    {
                        // We can compact the table "from the right"
                        int i;
                        for (i = 1; i < this.m_count; ++i)
                        {
                            if (this.m_next[this.m_count - 1 - i] != null)
                            {
                                break;
                            }
                        }

                        Debug.Assert(i < this.m_count);
                        this.m_count -= i;
                        this.m_next = this.m_next.Resize(this.m_count, true);
                    }
                }
            }

            return ret;
        }

        /// <summary>
        ///     Signal all the matching pipes.
        /// </summary>
        public void Match([NotNull] byte[] data, int offset, int size, [NotNull] MultiTrieDelegate func, [CanBeNull] object arg)
        {
            MultiTrie current = this;

            int index = offset;

            while (true)
            {
                // Signal the pipes attached to this node.
                if (current.m_pipes != null)
                {
                    foreach (Pipe it in current.m_pipes)
                    {
                        func(it, null, 0, arg);
                    }
                }

                // If we are at the end of the message, there's nothing more to match.
                if (size == 0)
                {
                    break;
                }

                // If there are no subnodes in the trie, return.
                if (current.m_count == 0)
                {
                    break;
                }

                byte c = data[index];
                // If there's one subnode (optimisation).
                if (current.m_count == 1)
                {
                    if (c != current.m_minCharacter)
                    {
                        break;
                    }

                    current = current.m_next[0];
                    index++;
                    size--;
                    continue;
                }

                // If there are multiple subnodes.
                if (c < current.m_minCharacter || c >=
                    current.m_minCharacter + current.m_count)
                {
                    break;
                }

                if (current.m_next[c - current.m_minCharacter] == null)
                {
                    break;
                }

                current = current.m_next[c - current.m_minCharacter];
                index++;
                size--;
            }
        }

        private bool IsRedundant
        {
            get
            {
                return this.m_pipes == null && this.m_liveNodes == 0;
            }
        }
    }
}