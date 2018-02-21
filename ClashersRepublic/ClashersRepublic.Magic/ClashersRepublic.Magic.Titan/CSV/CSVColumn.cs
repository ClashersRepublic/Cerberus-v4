﻿namespace ClashersRepublic.Magic.Titan.CSV
{
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Util;

    public class CSVColumn
    {
        private readonly LogicArrayList<bool> _boolValue;
        private readonly LogicArrayList<int> _intValue;
        private readonly LogicArrayList<string> _stringValue;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CSVColumn" /> class.
        /// </summary>
        public CSVColumn(int type, int size)
        {
            this.ColumnType = type;

            this._intValue = new LogicArrayList<int>();
            this._boolValue = new LogicArrayList<bool>();
            this._stringValue = new LogicArrayList<string>();

            switch (type)
            {
                case -1:
                case 0:
                {
                    this._stringValue.EnsureCapacity(size);
                    break;
                }

                case 1:
                {
                    this._intValue.EnsureCapacity(size);
                    break;
                }

                case 2:
                {
                    this._boolValue.EnsureCapacity(size);
                    break;
                }

                default:
                {
                    Debugger.Error("Invalid CSVColumn type");
                    break;
                }
            }
        }

        public int ColumnType { get; }

        /// <summary>
        ///     Adds a empty value to column.
        /// </summary>
        public void AddEmptyValue()
        {
            switch (this.ColumnType)
            {
                case -1:
                case 0:
                {
                    this._stringValue.Add(string.Empty);
                    break;
                }

                case 1:
                {
                    this._intValue.Add(0);
                    break;
                }

                case 2:
                {
                    this._boolValue.Add(false);
                    break;
                }
            }
        }

        /// <summary>
        ///     Adds the specified boolean value to column.
        /// </summary>
        public void AddBooleanValue(bool value)
        {
            this._boolValue.Add(value);
        }

        /// <summary>
        ///     Adds the specified int value to column.
        /// </summary>
        public void AddIntegerValue(int value)
        {
            this._intValue.Add(value);
        }

        /// <summary>
        ///     Adds the specified string value to column.
        /// </summary>
        public void AddStringValue(string value)
        {
            this._stringValue.Add(value);
        }

        /// <summary>
        ///     Gets the array size.
        /// </summary>
        public int GetArraySize(int startOffset, int endOffset)
        {
            return endOffset - startOffset;
        }

        /// <summary>
        ///     Gets the boolean value at specified index.
        /// </summary>
        public bool GetBooleanValue(int index)
        {
            return this._boolValue[index];
        }

        /// <summary>
        ///     Gets the integer value at specified index.
        /// </summary>
        public int GetIntegerValue(int index)
        {
            return this._intValue[index];
        }

        /// <summary>
        ///     Gets the string value at specified index.
        /// </summary>
        public string GetStringValue(int index)
        {
            return this._stringValue[index];
        }

        /// <summary>
        ///     Gets the column size.
        /// </summary>
        public int GetSize()
        {
            switch (this.ColumnType)
            {
                case -1:
                case 0:
                {
                    return this._stringValue.Count;
                }

                case 1:
                {
                    return this._intValue.Count;
                }

                case 2:
                {
                    return this._boolValue.Count;
                }
            }

            return 0;
        }
    }
}