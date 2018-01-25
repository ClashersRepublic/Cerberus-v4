namespace ClashersRepublic.Magic.Titan.CSV
{
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Util;

    public class CSVTable
    {
        public LogicArrayList<string> ColumnNames;
        public LogicArrayList<CSVColumn> Columns;

        public CSVNode Node;
        public LogicArrayList<CSVRow> Rows;

        public int Size;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CSVTable" /> class.
        /// </summary>
        public CSVTable(CSVNode node, int size)
        {
            this.ColumnNames = new LogicArrayList<string>();
            this.Columns = new LogicArrayList<CSVColumn>();
            this.Rows = new LogicArrayList<CSVRow>();

            this.Node = node;
            this.Size = size;
        }

        /// <summary>
        ///     Adds and converts the specified value to list.
        /// </summary>
        public void AddAndConvertValue(string value, int columnIndex)
        {
            CSVColumn column = this.Columns[columnIndex];

            if (!string.IsNullOrEmpty(value))
            {
                switch (column.ColumnType)
                {
                    case -1:
                    {
                        column.AddStringValue(value);
                        break;
                    }

                    case 0:
                    {
                        column.AddStringValue(value);
                        break;
                    }

                    case 1:
                    {
                        column.AddIntegerValue(int.Parse(value));
                        break;
                    }

                    case 2:
                    {
                        if (bool.TryParse(value, out bool booleanValue))
                        {
                            column.AddBooleanValue(booleanValue);
                        }
                        else
                        {
                            Debugger.Warning("CSVTable::addAndConvertValue invalid value '" + value + "' in Boolean column '" + this.ColumnNames[columnIndex] + "', " + this.GetFileName());
                            column.AddBooleanValue(false);
                        }

                        break;
                    }
                }
            }
            else
            {
                column.AddEmptyValue();
            }
        }

        /// <summary>
        ///     Adds a new column name to array list.
        /// </summary>
        public void AddColumn(string name)
        {
            this.ColumnNames.Add(name);
        }

        /// <summary>
        ///     Adds a new column to array list.
        /// </summary>
        public void AddColumnType(int type)
        {
            this.Columns.Add(new CSVColumn(type, this.Size));
        }

        /// <summary>
        ///     Adds a new row instance to array list.
        /// </summary>
        public void AddRow(CSVRow row)
        {
            this.Rows.Add(row);
        }

        /// <summary>
        ///     Called when the column names has been loaded.
        /// </summary>
        public void ColumnNamesLoaded()
        {
            this.Columns.EnsureCapacity(this.ColumnNames.Count);
        }

        /// <summary>
        ///     Creates a new row instance.
        /// </summary>
        public void CreateRow()
        {
            this.Rows.Add(new CSVRow(this));
        }

        /// <summary>
        ///     Gets the array size at specified column.
        /// </summary>
        public int GetArraySizeAt(CSVRow row, int columnIdx)
        {
            if (this.Rows.Count > 0)
            {
                int rowIdx = this.Rows.IndexOf(row);

                if (rowIdx != -1)
                {
                    CSVColumn column = this.Columns[columnIdx];

                    int nextRowOffset;

                    if (rowIdx + 1 >= this.Rows.Count)
                    {
                        nextRowOffset = column.GetSize();
                    }
                    else
                    {
                        nextRowOffset = this.Rows[rowIdx + 1].GetRowOffset();
                    }

                    return column.GetArraySize(this.Rows[rowIdx].GetRowOffset(), nextRowOffset);
                }
            }

            return 0;
        }

        /// <summary>
        ///     Gets the column name by index.
        /// </summary>
        public string GetColumnName(int idx)
        {
            return this.ColumnNames[idx];
        }

        /// <summary>
        ///     Gets the index of column by name.
        /// </summary>
        public int GetColumnIndexByName(string name)
        {
            return this.ColumnNames.IndexOf(name);
        }

        /// <summary>
        ///     Gets the number of columns.
        /// </summary>
        public int GetColumnCount()
        {
            return this.ColumnNames.Count;
        }

        /// <summary>
        ///     Gets the number of rows.
        /// </summary>
        public int GetColumnRowCount()
        {
            return this.Columns[0].GetSize();
        }

        /// <summary>
        ///     Gets the number of type.
        /// </summary>
        public int GetColumnTypeCount()
        {
            return this.Columns.Count;
        }

        /// <summary>
        ///     Gets the file name of csv.
        /// </summary>
        public string GetFileName()
        {
            return this.Node.GetFileName();
        }

        /// <summary>
        ///     Gets the boolean value at specified column and specified value index.
        /// </summary>
        public bool GetBooleanValue(string name, int index)
        {
            int columnIndex = this.ColumnNames.IndexOf(name);

            if (columnIndex != -1)
            {
                return this.Columns[columnIndex].GetBooleanValue(index);
            }

            return false;
        }

        /// <summary>
        ///     Gets the boolean value at specified column index and specified value index.
        /// </summary>
        public bool GetBooleanValueAt(int columnIndex, int index)
        {
            if (columnIndex != -1)
            {
                return this.Columns[columnIndex].GetBooleanValue(index);
            }

            return false;
        }

        /// <summary>
        ///     Gets the integer value at specified column and specified value index.
        /// </summary>
        public int GetIntegerValue(string name, int index)
        {
            int columnIndex = this.ColumnNames.IndexOf(name);

            if (columnIndex != -1)
            {
                return this.Columns[columnIndex].GetIntegerValue(index);
            }

            return 0;
        }

        /// <summary>
        ///     Gets the integer value at specified column index and specified value index.
        /// </summary>
        public int GetIntegerValueAt(int columnIndex, int index)
        {
            if (columnIndex != -1)
            {
                return this.Columns[columnIndex].GetIntegerValue(index);
            }

            return 0;
        }

        /// <summary>
        ///     Gets the value at specified column and specified value index.
        /// </summary>
        public string GetValue(string name, int index)
        {
            int columnIndex = this.ColumnNames.IndexOf(name);

            if (columnIndex != -1)
            {
                return this.Columns[columnIndex].GetStringValue(index);
            }

            return string.Empty;
        }

        /// <summary>
        ///     Gets the value at specified column index and specified value index.
        /// </summary>
        public string GetValueAt(int columnIndex, int index)
        {
            if (columnIndex != -1)
            {
                return this.Columns[columnIndex].GetStringValue(index);
            }

            return string.Empty;
        }

        /// <summary>
        ///     Gets the integer value at specified index.
        /// </summary>
        public CSVRow GetRowAt(int index)
        {
            return this.Rows[index];
        }

        /// <summary>
        ///     Gets the number of rows.
        /// </summary>
        public int GetRowCount()
        {
            return this.Rows.Count;
        }

        /// <summary>
        ///     Validates the column types.
        /// </summary>
        public void ValidateColumnTypes()
        {
            if (this.ColumnNames.Count != this.Columns.Count)
            {
                Debugger.Warning($"Column name count {this.ColumnNames.Count}, column type count {this.Columns.Count}, file {this.GetFileName()}");
            }
        }
    }
}