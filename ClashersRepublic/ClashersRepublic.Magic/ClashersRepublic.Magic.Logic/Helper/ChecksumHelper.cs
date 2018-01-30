namespace ClashersRepublic.Magic.Logic.Helper
{
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Util;

    public class ChecksumHelper
    {
        private int _checksum;

        private LogicArrayList<Element> _childs;
        private LogicJSONObject _root;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChecksumHelper"/> class.
        /// </summary>
        public ChecksumHelper()
        {
            this._root = new LogicJSONObject();
            this._childs = new LogicArrayList<Element>();
        }

        /// <summary>
        /// Starts to write a object.
        /// </summary>
        public void StartObject(string name)
        {
            this._childs.Add(new Element
            {
                Node = new LogicJSONObject(),
                Key = name
            });
        }

        /// <summary>
        /// Ends to write a object.
        /// </summary>
        public void EndObject()
        {
            if (this._childs.Count != 0)
            {
                if (this._childs.Count > 1)
                {
                    Element lastElement = this._childs[this._childs.Count - 1];
                    Element previousElement = this._childs[this._childs.Count - 2];

                    Debugger.DoAssert(lastElement.Node.GetJSONNodeType() == 2, "ChecksumHelper::endObject() called but top is not an object");

                    if (previousElement.Node.GetJSONNodeType() == 2)
                    {
                        ((LogicJSONObject)previousElement.Node).Put(lastElement.Key, lastElement.Node);
                    }
                    else
                    {
                        ((LogicJSONArray)previousElement.Node).Add(lastElement.Node);
                    }

                    this._childs.Remove(this._childs.Count - 1);
                }
                else
                {
                    Element last = this._childs[0];

                    this._root.Put(last.Key, last.Node);
                    this._childs.Remove(0);
                }
            }
        }

        /// <summary>
        /// Starts to write a array.
        /// </summary>
        public void StartArray(string name)
        {
            if (this._childs.Count > 0)
            {
                Element previousElement = this._childs[this._childs.Count - 1];

                if (previousElement.Node.GetJSONNodeType() != 2)
                {
                    Debugger.Error("ChecksumHelper::startArray can't handle the truth (array inside array)");
                }
                else
                {
                    this._childs.Add(new Element
                    {
                        Key = name,
                        Node = new LogicJSONArray()
                    });
                }
            }
        }

        /// <summary>
        /// Writes a value.
        /// </summary>
        public void WriteValue(string name, int value)
        {
            this._checksum += value;

            if (this._childs.Count > 0)
            {
                Element current = this._childs[this._childs.Count - 1];

                if (current.Node.GetJSONNodeType() == 2)
                {
                    ((LogicJSONObject)current.Node).Put(name, new LogicJSONNumber(value));
                }
            }
        }

        /// <summary>
        /// Ends to write a array.
        /// </summary>
        public void EndArray()
        {
            if (this._childs.Count != 0)
            {
                if (this._childs.Count > 1)
                {
                    Element lastElement = this._childs[this._childs.Count - 1];
                    Element previousElement = this._childs[this._childs.Count - 2];

                    Debugger.DoAssert(lastElement.Node.GetJSONNodeType() == 1, "ChecksumHelper::endArray() called but top is not an array");

                    ((LogicJSONObject)previousElement.Node).Put(lastElement.Key, lastElement.Node);

                    this._childs.Remove(this._childs.Count - 1);
                }
                else
                {
                    this._root.Put(this._childs[0].Key, this._childs[0].Node);
                    this._childs.Remove(0);
                }
            }
        }

        /// <summary>
        /// Gets this instance to string.
        /// </summary>
        public override string ToString()
        {
            return LogicJSONParser.CreateJSONString(this._root);
        }

        public static implicit operator int(ChecksumHelper helper)
        {
            return helper._checksum;
        }
        
        private struct Element
        {
            internal string Key;
            internal LogicJSONNode Node;
        }

        /// <summary>
        ///     Destructes this instance.
        /// </summary>
        public void Destruct()
        {
            while (this._childs.Count != 0)
            {
                this._childs.Remove(0);
            }

            this._root = new LogicJSONObject();
        }

        /// <summary>
        ///     Destructor of this instance.
        /// </summary>
        ~ChecksumHelper()
        {
            while (this._childs.Count != 0)
            {
                this._childs.Remove(0);
            }

            this._childs = null;
            this._root = null;
        }
    }
}