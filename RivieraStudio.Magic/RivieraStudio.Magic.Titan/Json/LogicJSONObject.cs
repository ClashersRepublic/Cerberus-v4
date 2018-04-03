namespace RivieraStudio.Magic.Titan.Json
{
    using System.Text;
    using RivieraStudio.Magic.Titan.Debug;
    using RivieraStudio.Magic.Titan.Util;

    public class LogicJSONObject : LogicJSONNode
    {
        private readonly LogicArrayList<string> _keys;
        private readonly LogicArrayList<LogicJSONNode> _items;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicJSONObject" /> class.
        /// </summary>
        public LogicJSONObject()
        {
            this._keys = new LogicArrayList<string>();
            this._items = new LogicArrayList<LogicJSONNode>();

            this._keys.EnsureCapacity(20);
            this._items.EnsureCapacity(20);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicJSONObject" /> class.
        /// </summary>
        public LogicJSONObject(int capacity)
        {
            this._keys = new LogicArrayList<string>();
            this._items = new LogicArrayList<LogicJSONNode>();

            this._keys.EnsureCapacity(capacity);
            this._items.EnsureCapacity(capacity);
        }

        /// <summary>
        ///     Gets the specified json array.
        /// </summary>
        public LogicJSONArray GetJSONArray(string key)
        {
            int itemIndex = this._keys.IndexOf(key);

            if (itemIndex == -1)
            {
                return null;
            }

            LogicJSONNode node = this._items[itemIndex];

            if (node.GetJSONNodeType() == 1)
            {
                return (LogicJSONArray) node;
            }

            Debugger.Warning("LogicJSONObject::getJSONArray type is " + node.GetJSONNodeType() + ", key " + key);

            return null;
        }

        /// <summary>
        ///     Gets the specified json boolean.
        /// </summary>
        public LogicJSONBoolean GetJSONBoolean(string key)
        {
            int itemIndex = this._keys.IndexOf(key);

            if (itemIndex == -1)
            {
                return null;
            }

            LogicJSONNode node = this._items[itemIndex];

            if (node.GetJSONNodeType() == 5)
            {
                return (LogicJSONBoolean) node;
            }

            Debugger.Warning("LogicJSONObject::getJSONBoolean type is " + node.GetJSONNodeType() + ", key " + key);

            return null;
        }

        /// <summary>
        ///     Gets the specified json boolean.
        /// </summary>
        public LogicJSONNumber GetJSONNumber(string key)
        {
            int itemIndex = this._keys.IndexOf(key);

            if (itemIndex == -1)
            {
                return null;
            }

            LogicJSONNode node = this._items[itemIndex];

            if (node.GetJSONNodeType() == 3)
            {
                return (LogicJSONNumber) node;
            }

            Debugger.Warning("LogicJSONObject::getJSONNumber type is " + node.GetJSONNodeType() + ", key " + key);

            return null;
        }

        /// <summary>
        ///     Gets the specified json object.
        /// </summary>
        public LogicJSONObject GetJSONObject(string key)
        {
            int itemIndex = this._keys.IndexOf(key);

            if (itemIndex == -1)
            {
                return null;
            }

            LogicJSONNode node = this._items[itemIndex];

            if (node.GetJSONNodeType() == 2)
            {
                return (LogicJSONObject) node;
            }

            Debugger.Warning("LogicJSONObject::getJSONObject type is " + node.GetJSONNodeType() + ", key " + key);

            return null;
        }

        /// <summary>
        ///     Gets the specified json object.
        /// </summary>
        public LogicJSONString GetJSONString(string key)
        {
            int itemIndex = this._keys.IndexOf(key);

            if (itemIndex == -1)
            {
                return null;
            }

            LogicJSONNode node = this._items[itemIndex];

            if (node.GetJSONNodeType() == 4)
            {
                return (LogicJSONString) node;
            }

            Debugger.Warning("LogicJSONObject::getJSONString type is " + node.GetJSONNodeType() + ", key " + key);

            return null;
        }

        /// <summary>
        ///     Adds the specified item to array.
        /// </summary>
        public void Put(string key, LogicJSONNode item)
        {
            int keyIndex = this._keys.IndexOf(key);

            if (keyIndex != -1)
            {
                Debugger.Error("LogicJSONObject::put already contains key " + key);
            }
            else
            {
                int itemIndex = this._items.IndexOf(item);

                if (itemIndex != -1)
                {
                    Debugger.Error("LogicJSONObject::put already contains the given JSONNode pointer. Key " + key);
                }
                else
                {
                    this._items.Add(item);
                    this._keys.Add(key);
                }
            }
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
            return 2;
        }

        /// <summary>
        ///     Writes this node to builder.
        /// </summary>
        public override void WriteToString(StringBuilder builder)
        {
            builder.Append('{');

            for (int i = 0; i < this._items.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(',');
                }

                LogicJSONParser.WriteString(this._keys[i], builder);
                builder.Append(':');
                this._items[i].WriteToString(builder);
            }

            builder.Append('}');
        }
    }
}