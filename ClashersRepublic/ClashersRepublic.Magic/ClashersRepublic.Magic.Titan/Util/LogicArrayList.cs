﻿namespace ClashersRepublic.Magic.Titan.Util
{
    using System;
    using ClashersRepublic.Magic.Titan.Debug;

    public class LogicArrayList<T>
    {
        private T[] _items;
        private int _index;

        /// <summary>
        ///     Gets a value indicating the number of items in list.
        /// </summary>
        public int Count
        {
            get
            {
                return this._index;
            }
        }

        /// <summary>
        ///     Gets the item at specified index.
        /// </summary>
        public T this[int index]
        {
            get
            {
                if (index >= this._index || index < 0)
                {
                    Debugger.Error("LogicArrayList.get out of bounds " + index + "/" + this._index);
                    return default(T);
                }

                return this._items[index];
            }
            set
            {
                if (index >= this._index || index < 0)
                {
                    Debugger.Error("LogicArrayList.set out of bounds " + index + "/" + this._index);
                    return;
                }

                this._items[index] = value;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicArrayList{T}"/> class.
        /// </summary>
        public LogicArrayList()
        {
            this._items = new T[0];
        }

        /// <summary>
        ///     Adds the specified item to list.
        /// </summary>
        public void Add(T item)
        {
            int size = this._items.Length;

            if (size == this._index)
            {
                this.EnsureCapacity(size != 0 ? size * 2 : 5);
            }

            this._items[this._index++] = item;
        }

        /// <summary>
        ///     Gets the index of specified item.
        /// </summary>
        public int IndexOf(T item)
        {
            for (int i = 0; i < this._index; i++)
            {
                if (this._items[i].Equals(item))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        ///     Removes the item at specified index to list.
        /// </summary>
        public void Remove(int index)
        {
            if (index > this._index || index < 0)
            {
                Debugger.Error("LogicArrayList.remove out of bounds " + index + "/" + this._index);
                return;
            }

            if (index != this._index - 1)
            {
                Array.Copy(this._items, index + 1, this._items, index, this._index - index - 1);
            }

            this._items[--this._index] = default(T);
        }

        /// <summary>
        ///     Ensures the capacity of item array.
        /// </summary>
        public void EnsureCapacity(int count)
        {
            int size = this._items.Length;

            if (size < count)
            {
                T[] tmp = new T[count];
                Array.Copy(this._items, tmp, size);
                this._items = tmp;
            }
        }

        /// <summary>
        ///     Called for destructs this instance.
        /// </summary>
        ~LogicArrayList()
        {
            this._items = null;
            this._index = 0;
        }
    }
}