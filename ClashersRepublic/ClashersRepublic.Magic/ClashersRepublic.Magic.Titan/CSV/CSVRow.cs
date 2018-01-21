namespace ClashersRepublic.Magic.Titan.CSV
{
    using ClashersRepublic.Magic.Titan.Math;

    public class CSVRow
    {
        private int _rowOffset;
        private CSVTable _table;

        /// <summary>
        /// Initializes a new instance of the <see cref="CSVRow"/> class.
        /// </summary>
        public CSVRow(CSVTable table)
        {
            this._table = table;
            this._rowOffset = table.GetColumnRowCount();
        }

        /// <summary>
        /// Gets the array size of specified column.
        /// </summary>
        public int GetBiggestArraySize(string column)
        {
            int columnIndex = this.GetColumnIndexByName(column);

            if (columnIndex == -1)
            {
                return 0;
            }

            return this._table.GetArraySizeAt(this, columnIndex);
        }

        /// <summary>
        /// Gets the biggest array size.
        /// </summary>
        public int GetBiggestArraySize()
        {
            int columnCount = this._table.GetColumnCount();
            int maxSize = 0;

            for (int i = 0; i < columnCount; i++)
            {
                maxSize = LogicMath.Max(this._table.GetArraySizeAt(this, i), maxSize);
            }

            return maxSize;
        }

        /// <summary>
        /// Gets the number of columns.
        /// </summary>
        public int GetColumnCount()
        {
            return this._table.GetColumnCount();
        }

        /// <summary>
        /// Gets the index of specified column.
        /// </summary>
        public int GetColumnIndexByName(string name)
        {
            return this._table.GetColumnIndexByName(name);
        }

        /// <summary>
        /// Gets the boolean value at specified column and specified index.
        /// </summary>
        public bool GetBooleanValue(string columnName, int index)
        {
            return this._table.GetBooleanValue(columnName, this._rowOffset + index);
        }

        /// <summary>
        /// Gets the boolean value at specified column index and specified index.
        /// </summary>
        public bool GetBooleanValueAt(int columnIndex, int index)
        {
            return this._table.GetBooleanValueAt(columnIndex, this._rowOffset + index);
        }

        /// <summary>
        /// Gets the integer value at specified column and specified index.
        /// </summary>
        public int GetIntegerValue(string columnName, int index)
        {
            return this._table.GetIntegerValue(columnName, this._rowOffset + index);
        }

        /// <summary>
        /// Gets the integer value at specified column index and specified index.
        /// </summary>
        public int GetIntegerValueAt(int columnIndex, int index)
        {
            return this._table.GetIntegerValueAt(columnIndex, this._rowOffset + index);
        }

        /// <summary>
        /// Gets the value at specified column and specified index.
        /// </summary>
        public string GetValue(string columnName, int index)
        {
            return this._table.GetValue(columnName, this._rowOffset + index);
        }

        /// <summary>
        /// Gets the value at specified column index and specified index.
        /// </summary>
        public string GetValueAt(int columnIndex, int index)
        {
            return this._table.GetValueAt(columnIndex, this._rowOffset + index);
        }

        /// <summary>
        /// Gets the name of this row.
        /// </summary>
        public string GetName()
        {
            return this._table.GetValueAt(0, this._rowOffset);
        }

        /// <summary>
        /// Gets the row offset.
        /// </summary>
        public int GetRowOffset()
        {
            return this._rowOffset;
        }
    }
}