namespace RivieraStudio.Magic.Logic.Command
{
    using RivieraStudio.Magic.Logic.Level;
    using RivieraStudio.Magic.Titan.DataStream;
    using RivieraStudio.Magic.Titan.Debug;
    using RivieraStudio.Magic.Titan.Json;

    public class LogicCommand
    {
        private int _executeSubTick;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicCommand" /> class.
        /// </summary>
        public LogicCommand()
        {
            this._executeSubTick = -1;
        }

        /// <summary>
        ///     Gets a value indicating whether this command is a server command.
        /// </summary>
        public virtual bool IsServerCommand()
        {
            return false;
        }

        /// <summary>
        ///     Gets the command type.
        /// </summary>
        public virtual int GetCommandType()
        {
            return 0;
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public virtual void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteInt(this._executeSubTick);
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public virtual void Decode(ByteStream stream)
        {
            this._executeSubTick = stream.ReadInt();
        }

        /// <summary>
        ///     Executes this command.
        /// </summary>
        public virtual int Execute(LogicLevel level)
        {
            return 0;
        }

        /// <summary>
        ///     Gets the json instance for replay.
        /// </summary>
        public virtual LogicJSONObject GetJSONForReplay()
        {
            LogicJSONObject jsonRoot = new LogicJSONObject();
            jsonRoot.Put("t", new LogicJSONNumber(this._executeSubTick));
            return jsonRoot;
        }

        /// <summary>
        ///     Loads this instance from json.
        /// </summary>
        public virtual void LoadFromJSON(LogicJSONObject jsonRoot)
        {
            LogicJSONNumber tickNumber = jsonRoot.GetJSONNumber("t");

            if (tickNumber != null)
            {
                this._executeSubTick = tickNumber.GetIntValue();
            }
            else
            {
                Debugger.Error("Replay - Load command from JSON failed! Execute sub tick was not found!");
            }
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public virtual void Destruct()
        {
            this._executeSubTick = -1;
        }

        /// <summary>
        ///     Sets the execute subtick value.
        /// </summary>
        public void SetExecuteSubTick(int tick)
        {
            this._executeSubTick = tick;
        }

        /// <summary>
        ///     Gets the execute subtick value.
        /// </summary>
        public int GetExecuteSubTick()
        {
            return this._executeSubTick;
        }

        /// <summary>
        ///     Destructors of this instance.
        /// </summary>
        ~LogicCommand()
        {
            // LogicCommand.
        }
    }
}