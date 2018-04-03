/*
    Copyright (c) 2009-2011 250bpm s.r.o.
    Copyright (c) 2007-2009 iMatix Corporation
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

namespace RivieraStudio.Magic.Services.Core.Libs.NetMQ.Core.Utils
{
    using System;
    using System.Diagnostics;
    using JetBrains.Annotations;

    /// <summary>A FIFO queue.</summary>
    /// <remarks>
    ///     The class supports:
    ///     <list type="bullet">
    ///         <item>Push-front via <see cref="Push" />.</item>
    ///         <item>Pop-back via <see cref="Pop" />.</item>
    ///         <item>Pop-front via <see cref="Unpush" />.</item>
    ///     </list>
    ///     As such it is only one operation short of being a double-ended queue (dequeue or deque).
    ///     <para />
    ///     The internal implementation consists of a doubly-linked list of fixed-size arrays.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    internal sealed class YQueue<T>
    {
        #region Nested class: Chunk

        /// <summary>Individual memory chunk to hold N elements.</summary>
        private sealed class Chunk
        {
            public Chunk(int size, int globalIndex)
            {
                this.Values = new T[size];
                this.GlobalOffset = globalIndex;
                Debug.Assert(this.Values != null);
            }

            [NotNull]
            public T[] Values { get; }

            /// <summary>Contains global index positions of elements in the chunk.</summary>
            public int GlobalOffset { get; }

            /// <summary>Optional link to the previous <see cref="Chunk" />.</summary>
            [CanBeNull]
            public Chunk Previous { get; set; }

            /// <summary>Optional link to the next <see cref="Chunk" />.</summary>
            [CanBeNull]
            public Chunk Next { get; set; }
        }

        #endregion

        private readonly int m_chunkSize;

        // Back position may point to invalid memory if the queue is empty,
        // while begin & end positions are always valid. Begin position is
        // accessed exclusively be queue reader (front/pop), while back and
        // end positions are accessed exclusively by queue writer (back/push).
        private volatile Chunk m_beginChunk;
        private int m_beginPositionInChunk;
        private Chunk m_backChunk;
        private int m_backPositionInChunk;
        private Chunk m_endChunk;
        private int m_endPosition;
        private Chunk m_spareChunk;

        // People are likely to produce and consume at similar rates.  In
        // this scenario holding onto the most recently freed chunk saves
        // us from having to call malloc/free.

        private int m_nextGlobalIndex;

        /// <param name="chunkSize">the size to give the new YQueue</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="chunkSize" /> should be no less than 2</exception>
        public YQueue(int chunkSize)
        {
            if (chunkSize < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(chunkSize), "Should be no less than 2");
            }

            this.m_chunkSize = chunkSize;

            this.m_beginChunk = new Chunk(this.m_chunkSize, 0);
            this.m_nextGlobalIndex = this.m_chunkSize;
            this.m_backChunk = this.m_beginChunk;
            this.m_spareChunk = this.m_beginChunk;
            this.m_endChunk = this.m_beginChunk;
            this.m_endPosition = 1;
        }

        /// <summary>Gets the index of the front element of the queue.</summary>
        /// <value>The index of the front element of the queue.</value>
        /// <remarks>If the queue is empty, it should be equal to <see cref="BackPos" />.</remarks>
        public int FrontPos
        {
            get
            {
                return this.m_beginChunk.GlobalOffset + this.m_beginPositionInChunk;
            }
        }

        /// <summary>Gets the front element of the queue. If the queue is empty, behaviour is undefined.</summary>
        /// <value>The front element of the queue.</value>
        public T Front
        {
            get
            {
                return this.m_beginChunk.Values[this.m_beginPositionInChunk];
            }
        }

        /// <summary>Gets the index of the back element of the queue.</summary>
        /// <value>The index of the back element of the queue.</value>
        /// <remarks>If the queue is empty, it should be equal to <see cref="FrontPos" />.</remarks>
        public int BackPos
        {
            get
            {
                return this.m_backChunk.GlobalOffset + this.m_backPositionInChunk;
            }
        }

        /// <summary>Retrieves the element at the front of the queue.</summary>
        /// <returns>The element taken from queue.</returns>
        public T Pop()
        {
            T value = this.m_beginChunk.Values[this.m_beginPositionInChunk];
            this.m_beginChunk.Values[this.m_beginPositionInChunk] = default(T);

            this.m_beginPositionInChunk++;
            if (this.m_beginPositionInChunk == this.m_chunkSize)
            {
                this.m_beginChunk = this.m_beginChunk.Next;
                this.m_beginChunk.Previous = null;
                this.m_beginPositionInChunk = 0;
            }

            return value;
        }

        /// <summary>Adds an element to the back end of the queue.</summary>
        /// <param name="val">The value to be pushed.</param>
        public void Push(ref T val)
        {
            this.m_backChunk.Values[this.m_backPositionInChunk] = val;
            this.m_backChunk = this.m_endChunk;
            this.m_backPositionInChunk = this.m_endPosition;

            this.m_endPosition++;
            if (this.m_endPosition != this.m_chunkSize)
            {
                return;
            }

            Chunk sc = this.m_spareChunk;
            if (sc != this.m_beginChunk)
            {
                this.m_spareChunk = this.m_spareChunk.Next;
                this.m_endChunk.Next = sc;
                sc.Previous = this.m_endChunk;
            }
            else
            {
                this.m_endChunk.Next = new Chunk(this.m_chunkSize, this.m_nextGlobalIndex);
                this.m_nextGlobalIndex += this.m_chunkSize;
                this.m_endChunk.Next.Previous = this.m_endChunk;
            }

            this.m_endChunk = this.m_endChunk.Next;
            this.m_endPosition = 0;
        }

        /// <summary>Removes element from the back end of the queue, rolling back the last call to <see cref="Push" />.</summary>
        /// <remarks>
        ///     The caller must guarantee that the queue isn't empty when calling this method.
        ///     It cannot be done automatically as the read side of the queue can be managed by different,
        ///     completely unsynchronized threads.
        /// </remarks>
        /// <returns>The last item passed to <see cref="Push" />.</returns>
        public T Unpush()
        {
            // First, move 'back' one position backwards.
            if (this.m_backPositionInChunk > 0)
            {
                this.m_backPositionInChunk--;
            }
            else
            {
                this.m_backPositionInChunk = this.m_chunkSize - 1;
                this.m_backChunk = this.m_backChunk.Previous;
            }

            // Now, move 'end' position backwards. Note that obsolete end chunk
            // is not used as a spare chunk. The analysis shows that doing so
            // would require free and atomic operation per chunk deallocated
            // instead of a simple free.
            if (this.m_endPosition > 0)
            {
                this.m_endPosition--;
            }
            else
            {
                this.m_endPosition = this.m_chunkSize - 1;
                this.m_endChunk = this.m_endChunk.Previous;
                this.m_endChunk.Next = null;
            }

            // Capturing and removing the unpushed value from chunk.
            T value = this.m_backChunk.Values[this.m_backPositionInChunk];
            this.m_backChunk.Values[this.m_backPositionInChunk] = default(T);

            return value;
        }
    }
}