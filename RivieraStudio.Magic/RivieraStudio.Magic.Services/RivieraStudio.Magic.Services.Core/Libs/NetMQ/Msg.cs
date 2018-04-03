/*
    Copyright (c) 2007-2012 iMatix Corporation
    Copyright (c) 2009-2011 250bpm s.r.o.
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

namespace RivieraStudio.Magic.Services.Core.Libs.NetMQ
{
    using System;
    using RivieraStudio.Magic.Services.Core.Libs.NetMQ.Core.Utils;
    using JetBrains.Annotations;

    /// <summary>Defines a set of flags applicable to a <see cref="Msg" /> instance: None (default), More, Identity, Shared</summary>
    [Flags]
    public enum MsgFlags : byte
    {
        /// <summary>Indicates no flags are set (the default).</summary>
        None = 0,

        /// <summary>Indicates that more frames of the same message follow.</summary>
        More = 1,

        /// <summary>Indicates that this frame conveys the identity of a connected peer.</summary>
        Identity = 64,

        /// <summary>Indicates that this frame's internal data is shared with other <see cref="Msg" /> objects.</summary>
        Shared = 128
    }

    /// <summary>Enumeration of possible <see cref="Msg" /> types: Uninitialised, GC, Pool, Delimiter.</summary>
    public enum MsgType : byte
    {
        /// <summary>The <see cref="Msg" /> has not yet been initialised (default value).</summary>
        Uninitialised = 0,

        /// <summary>The <see cref="Msg" /> is empty.</summary>
        Empty = 101,

        /// <summary>The <see cref="Msg" /> data will be garbage collected when no longer needed.</summary>
        GC = 102,

        /// <summary>
        ///     The <see cref="Msg" /> data was allocated by <see cref="BufferPool" />, and must be released back
        ///     to this pool when no longer needed. This happens when <see cref="Msg.Close" /> is called.
        /// </summary>
        Pool = 103,

        /// <summary>The <see cref="Msg" /> is a delimiter frame and doesn't contain any data.</summary>
        /// <remarks>Delimiters are commonly used to mark a boundary between groups frames.</remarks>
        Delimiter = 104
    }

    /// <summary>
    ///     A Msg struct is the lowest-level interpretation of a ZeroMQ message, and simply contains byte-array data
    ///     and MsgType and MsgFlags properties.
    ///     It supports message buffer pooling.
    /// </summary>
    /// <remarks>
    ///     Many users will not use this class directly. However in high-performance situations it
    ///     may be useful. When used correctly it's possible to have zero-copy and zero-allocation
    ///     behaviour.
    /// </remarks>
    public struct Msg
    {
        /// <summary>
        ///     An atomic reference count for knowing when to release a pooled data buffer back to the
        ///     <see cref="BufferPool" />.
        /// </summary>
        /// <remarks>Will be <c>null</c> unless <see cref="MsgType" /> equals <see cref="NetMQ.MsgType.Pool" />.</remarks>
        private AtomicCounter m_refCount;

        /// <summary>
        ///     Get the number of bytes within the Data property.
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        ///     Gets the position of the first element in the Data property delimited by the message,
        ///     relative to the start of the original array.
        /// </summary>
        public int Offset { get; private set; }

        #region MsgType

        /// <summary>Get the type of this message, from the MsgType enum.</summary>
        public MsgType MsgType { get; private set; }

        /// <summary>
        ///     Get whether the Delimiter bit is set on the Flags property,
        ///     which would indicate that this message is intended for use simply to mark a boundary
        ///     between other parts of some unit of communication.
        /// </summary>
        public bool IsDelimiter
        {
            get
            {
                return this.MsgType == MsgType.Delimiter;
            }
        }

        /// <summary>Get whether this <see cref="Msg" /> is initialised and ready for use.</summary>
        /// <remarks>
        ///     A newly constructed <see cref="Msg" /> is uninitialised, and can be initialised via one
        ///     of <see cref="InitEmpty" />, <see cref="InitDelimiter" />, <see cref="InitGC(byte[],int)" />,
        ///     <see cref="InitGC(byte[],int,int)" />, or <see cref="InitPool" />.
        ///     Calling <see cref="Close" /> will cause the <see cref="Msg" /> to become uninitialised again.
        /// </remarks>
        /// <returns><c>true</c> if the <see cref="Msg" /> is initialised, otherwise <c>false</c>.</returns>
        public bool IsInitialised
        {
            get
            {
                return this.MsgType != MsgType.Uninitialised;
            }
        }

        #endregion

        #region MsgFlags

        /// <summary>
        ///     Get the flags-enum MsgFlags value, which indicates which of the More, Identity, or Shared bits are set.
        /// </summary>
        public MsgFlags Flags { get; private set; }

        /// <summary>
        ///     Get the "Has-More" flag, which when set on a message-queue frame indicates that there are more frames to follow.
        /// </summary>
        public bool HasMore
        {
            get
            {
                return (this.Flags & MsgFlags.More) == MsgFlags.More;
            }
        }

        /// <summary>
        ///     Get whether the <see cref="Data" /> buffer of this <see cref="Msg" /> is shared with another instance.
        ///     Only applies to pooled message types.
        /// </summary>
        public bool IsShared
        {
            get
            {
                return (this.Flags & MsgFlags.Shared) != 0;
            }
        }

        /// <summary>
        ///     Get whether the Identity bit is set on the Flags property.
        /// </summary>
        public bool IsIdentity
        {
            get
            {
                return (this.Flags & MsgFlags.Identity) != 0;
            }
        }

        /// <summary>
        ///     Set the indicated Flags bits.
        /// </summary>
        /// <param name="flags">which Flags bits to set (More, Identity, or Shared)</param>
        public void SetFlags(MsgFlags flags)
        {
            this.Flags |= flags;
        }

        /// <summary>
        ///     Clear the indicated Flags bits.
        /// </summary>
        /// <param name="flags">which Flags bits to clear (More, Identity, or Shared)</param>
        public void ResetFlags(MsgFlags flags)
        {
            this.Flags &= ~flags;
        }

        #endregion

        /// <summary>
        ///     Get the byte-array that represents the data payload of this <see cref="Msg" />.
        /// </summary>
        /// <remarks>
        ///     This value will be <c>null</c> if <see cref="MsgType" /> is <see cref="NetMQ.MsgType.Uninitialised" />,
        ///     <see cref="NetMQ.MsgType.Empty" /> or <see cref="NetMQ.MsgType.Delimiter" />.
        /// </remarks>
        public byte[] Data { get; private set; }

        #region Initialisation

        /// <summary>
        ///     Clear this Msg to empty - ie, set MsgFlags to None, MsgType to Empty, and clear the Data.
        /// </summary>
        public void InitEmpty()
        {
            this.MsgType = MsgType.Empty;
            this.Flags = MsgFlags.None;
            this.Size = 0;
            this.Offset = 0;
            this.Data = null;
            this.m_refCount = null;
        }

        /// <summary>
        ///     Initialise this Msg to be of MsgType.Pool, with a data-buffer of the given number of bytes.
        /// </summary>
        /// <param name="size">the number of bytes to allocate in the data-buffer</param>
        public void InitPool(int size)
        {
            this.MsgType = MsgType.Pool;
            this.Flags = MsgFlags.None;
            this.Data = BufferPool.Take(size);
            this.Size = size;
            this.Offset = 0;
            this.m_refCount = new AtomicCounter();
        }

        /// <summary>
        ///     Initialise this Msg to be of MsgType.GC with the given data-buffer value.
        /// </summary>
        /// <param name="data">the byte-array of data to assign to the Msg's Data property</param>
        /// <param name="size">the number of bytes that are in the data byte-array</param>
        public void InitGC([NotNull] byte[] data, int size)
        {
            this.InitGC(data, 0, size);
        }

        /// <summary>
        ///     Initialise this Msg to be of MsgType.GC with the given data-buffer value.
        /// </summary>
        /// <param name="data">the byte-array of data to assign to the Msg's Data property</param>
        /// <param name="offset">first byte in the data array</param>
        /// <param name="size">the number of bytes that are in the data byte-array</param>
        public void InitGC([NotNull] byte[] data, int offset, int size)
        {
            this.MsgType = MsgType.GC;
            this.Flags = MsgFlags.None;
            this.Data = data;
            this.Size = size;
            this.Offset = offset;
            this.m_refCount = null;
        }

        /// <summary>
        ///     Set this Msg to be of type MsgType.Delimiter with no bits set within MsgFlags.
        /// </summary>
        public void InitDelimiter()
        {
            this.MsgType = MsgType.Delimiter;
            this.Flags = MsgFlags.None;
        }

        #endregion

        /// <summary>
        ///     Clear the <see cref="Data" /> and set the MsgType to Invalid.
        ///     If this is not a shared-data Msg (MsgFlags.Shared is not set), or it is shared but the reference-counter has
        ///     dropped to zero,
        ///     then return the data back to the BufferPool.
        /// </summary>
        /// <exception cref="FaultException">The object is not initialised.</exception>
        public void Close()
        {
            if (!this.IsInitialised)
            {
                throw new FaultException("Cannot close an uninitialised Msg.");
            }

            if (this.MsgType == MsgType.Pool)
            {
                // if not shared or reference counter drop to zero
                if (!this.IsShared || this.m_refCount.Decrement() == 0)
                {
                    BufferPool.Return(this.Data);
                }

                this.m_refCount = null;
            }

            // Uninitialise the frame
            this.Data = null;
            this.MsgType = MsgType.Uninitialised;
        }

        /// <summary>
        ///     If this Msg is of MsgType.Pool, then - add the given amount number to the reference-counter
        ///     and set the shared-data Flags bit.
        ///     If this is not a Pool Msg, this does nothing.
        /// </summary>
        /// <param name="amount">the number to add to the internal reference-counter</param>
        public void AddReferences(int amount)
        {
            if (amount == 0)
            {
                return;
            }

            if (this.MsgType == MsgType.Pool)
            {
                if (this.IsShared)
                {
                    this.m_refCount.Increase(amount);
                }
                else
                {
                    this.m_refCount.Set(amount);
                    this.Flags |= MsgFlags.Shared;
                }
            }
        }

        /// <summary>
        ///     If this Msg is of MsgType.Pool and is marked as Shared, then - subtract the given amount number from the
        ///     reference-counter
        ///     and, if that reaches zero - return the data to the shared-data pool.
        ///     If this is not both a Pool Msg and also marked as Shared, this simply Closes this Msg.
        /// </summary>
        /// <param name="amount">the number to subtract from the internal reference-counter</param>
        public void RemoveReferences(int amount)
        {
            if (amount == 0)
            {
                return;
            }

            if (this.MsgType != MsgType.Pool || !this.IsShared)
            {
                this.Close();
                return;
            }

            if (this.m_refCount.Decrement(amount) == 0)
            {
                this.m_refCount = null;

                BufferPool.Return(this.Data);

                // TODO shouldn't we set the type to uninitialised, or call clear, here? the object has a null refCount, but other methods may try to use it
            }
        }

        /// <summary>
        ///     Override the Object ToString method to show the object-type, and values of the MsgType, Size, and Flags properties.
        /// </summary>
        /// <returns>a string that provides some detail about this Msg's state</returns>
        public override string ToString()
        {
            return base.ToString() + "[" + this.MsgType + "," + this.Size + "," + this.Flags + "]";
        }

        /// <summary>
        ///     Copy the given byte-array data to this Msg's Data buffer.
        /// </summary>
        /// <param name="src">the source byte-array to copy from</param>
        /// <param name="i">index within the internal Data array to copy that byte to</param>
        /// <param name="len">the number of bytes to copy</param>
        public void Put([CanBeNull] byte[] src, int i, int len)
        {
            if (len == 0 || src == null)
            {
                return;
            }

            Buffer.BlockCopy(src, 0, this.Data, i, len);
        }

        /// <summary>
        ///     Copy the given byte-array data to this Msg's Data buffer.
        /// </summary>
        /// <param name="src">the source byte-array to copy from</param>
        /// <param name="srcOffset">first byte in the source byte-array</param>
        /// <param name="i">index within the internal Data array to copy that byte to</param>
        /// <param name="len">the number of bytes to copy</param>
        public void Put([CanBeNull] byte[] src, int srcOffset, int i, int len)
        {
            if (len == 0 || src == null)
            {
                return;
            }

            Buffer.BlockCopy(src, srcOffset, this.Data, i, len);
        }

        /// <summary>
        ///     Copy the given single byte to this Msg's Data buffer.
        /// </summary>
        /// <param name="b">the source byte to copy from</param>
        public void Put(byte b)
        {
            this.Data[this.Offset] = b;
        }

        /// <summary>
        ///     Copy the given single byte to this Msg's Data buffer at the given array-index.
        /// </summary>
        /// <param name="b">the source byte to copy from</param>
        /// <param name="i">index within the internal Data array to copy that byte to</param>
        public void Put(byte b, int i)
        {
            this.Data[this.Offset + i] = b;
        }

        /// <summary>
        ///     Get and set the byte value in the <see cref="Data" /> buffer at a specific index.
        /// </summary>
        /// <param name="index">The index to access</param>
        /// <returns></returns>
        public byte this[int index]
        {
            get
            {
                return this.Data[this.Offset + index];
            }
            set
            {
                this.Data[this.Offset + index] = value;
            }
        }

        /// <summary>
        ///     Close this Msg, and effectively make this Msg a copy of the given source Msg
        ///     by simply setting it to point to the given source Msg.
        ///     If this is a Pool Msg, then this also increases the reference-counter and sets the Shared bit.
        /// </summary>
        /// <param name="src">the source Msg to copy from</param>
        /// <exception cref="FaultException">The object is not initialised.</exception>
        public void Copy(ref Msg src)
        {
            // Check the validity of the source.
            if (!src.IsInitialised)
            {
                throw new FaultException("Cannot copy an uninitialised Msg.");
            }

            if (this.IsInitialised)
            {
                this.Close();
            }

            if (src.MsgType == MsgType.Pool)
            {
                // One reference is added to shared messages. Non-shared messages
                // are turned into shared messages and reference count is set to 2.
                if (this.IsShared)
                {
                    src.m_refCount.Increase(1);
                }
                else
                {
                    src.Flags |= MsgFlags.Shared;
                    src.m_refCount.Set(2);
                }
            }

            // Populate this instance via a memberwise-copy from the 'src' instance.
            this = src;
        }

        /// <summary>
        ///     Increase Offset and decrease Size by the given count.
        /// </summary>
        /// <param name="count">Number of bytes to remove from a message</param>
        public void TrimPrefix(int count)
        {
            if (count > this.Size || count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Count should be between 0 and size");
            }

            this.Offset = this.Offset + count;
            this.Size = this.Size - count;
        }

        /// <summary>
        ///     Close this Msg and make it reference the given source Msg, and then clear the Msg to empty.
        /// </summary>
        /// <param name="src">the source-Msg to become</param>
        /// <exception cref="FaultException">The object is not initialised.</exception>
        public void Move(ref Msg src)
        {
            // Check the validity of the source.
            if (!src.IsInitialised)
            {
                throw new FaultException("Cannot move an uninitialised Msg.");
            }

            if (this.IsInitialised)
            {
                this.Close();
            }

            this = src;

            src.InitEmpty();
        }

        /// <summary>Returns a new array containing the first <see cref="Size" /> bytes of <see cref="Data" />.</summary>
        public byte[] CloneData()
        {
            byte[] data = new byte[this.Size];

            if (this.Size > 0)
            {
                Buffer.BlockCopy(this.Data, this.Offset, data, 0, this.Size);
            }

            return data;
        }
    }
}