namespace RivieraStudio.Magic.Titan.Json
{
    using System.Text;
    using RivieraStudio.Magic.Titan.Debug;
    using RivieraStudio.Magic.Titan.Util;

    public class LogicJSONArray : LogicJSONNode
    {
        private readonly LogicArrayList<LogicJSONNode> _items;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicJSONArray" /> class.
        /// </summary>
        public LogicJSONArray()
        {
            this._items = new LogicArrayList<LogicJSONNode>();
            this._items.EnsureCapacity(20);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicJSONArray" /> class.
        /// </summary>
        public LogicJSONArray(int capacity)
        {
            this._items = new LogicArrayList<LogicJSONNode>();
            this._items.EnsureCapacity(capacity);
        }

        /// <summary>
        ///     Gets the items at specified index.
        /// </summary>
        public LogicJSONNode this[int index]
        {
            get
            {
                return this._items[index];
            }
        }

        /// <summary>
        ///     Adds the specified item to array.
        /// </summary>
        public void Add(LogicJSONNode item)
        {
            this._items.Add(item);
        }

        /// <summary>
        ///     Gets the specified json array.
        /// </summary>
        public LogicJSONArray GetJSONArray(int index)
        {
            LogicJSONNode node = this._items[index];

            if (node.GetJSONNodeType() != 1)
            {
                Debugger.Warning("LogicJSONObject::getJSONArray wrong type " + node.GetJSONNodeType() + ", index " + index);
                return null;
            }

            return (LogicJSONArray) node;
        }

        /// <summary>
        ///     Gets the specified json boolean.
        /// </summary>
        public LogicJSONBoolean GetJSONBoolean(int index)
        {
            LogicJSONNode node = this._items[index];

            if (node.GetJSONNodeType() != 5)
            {
                Debugger.Warning("LogicJSONObject::getJSONBoolean wrong type " + node.GetJSONNodeType() + ", index " + index);
                return null;
            }

            return (LogicJSONBoolean) node;
        }

        /// <summary>
        ///     Gets the specified json boolean.
        /// </summary>
        public LogicJSONNumber GetJSONNumber(int index)
        {
            LogicJSONNode node = this._items[index];

            if (node.GetJSONNodeType() != 3)
            {
                Debugger.Warning("LogicJSONObject::getJSONNumber wrong type " + node.GetJSONNodeType() + ", index " + index);
                return null;
            }

            return (LogicJSONNumber) node;
        }

        /// <summary>
        ///     Gets the specified json object.
        /// </summary>
        public LogicJSONObject GetJSONObject(int index)
        {
            LogicJSONNode node = this._items[index];

            if (node.GetJSONNodeType() != 2)
            {
                Debugger.Warning("LogicJSONObject::getJSONObject wrong type " + node.GetJSONNodeType() + ", index " + index);
                return null;
            }

            return (LogicJSONObject) node;
        }

        /// <summary>
        ///     Gets the specified json object.
        /// </summary>
        public LogicJSONString GetJSONString(int index)
        {
            LogicJSONNode node = this._items[index];

            if (node.GetJSONNodeType() != 4)
            {
                Debugger.Warning("LogicJSONObject::getJSONString wrong type " + node.GetJSONNodeType() + ", index " + index);
                return null;
            }

            return (LogicJSONString) node;
        }

        /// <summary>
        ///     Gets the size of array.
        /// </summary>
        public int Size()
        {
            return this._items.Count;
        }

        /// <summary>
        ///     Gets the json node type.
        /// </summary>
        public override int GetJSONNodeType()
        {
            return 1;
        }

        /// <summary>
        ///     Writes this node to builder.
        /// </summary>
        public override void WriteToString(StringBuilder builder)
        {
            builder.Append('[');

            for (int i = 0; i < this._items.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(',');
                }

                this._items[i].WriteToString(builder);
            }

            builder.Append(']');
        }
    }
}