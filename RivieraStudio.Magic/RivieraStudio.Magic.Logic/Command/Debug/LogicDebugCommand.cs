namespace RivieraStudio.Magic.Logic.Command.Debug
{
    using RivieraStudio.Magic.Logic.Level;
    using RivieraStudio.Magic.Titan.DataStream;
    using RivieraStudio.Magic.Titan.Debug;
    using RivieraStudio.Magic.Titan.Json;
    using RivieraStudio.Magic.Titan.Util;

    public sealed class LogicDebugCommand : LogicCommand
    {
        private int _debugAction;
        private int _intArg1;
        private int _intArg2;
        private string _stringArg;

        private LogicArrayList<int> _intArrayArg1;
        private LogicArrayList<int> _intArrayArg2;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicDebugCommand" /> class.
        /// </summary>
        public LogicDebugCommand()
        {
            // LogicDebugCommand.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicDebugCommand" /> class.
        /// </summary>
        public LogicDebugCommand(int actionType) : this()
        {
            this._debugAction = actionType;
        }

        /// <summary>
        ///     Executes this instance.
        /// </summary>
        public override int Execute(LogicLevel level)
        {
            switch (this._debugAction)
            {
                case 0:
                    Debugger.HudPrint("Fast forward 1 hour");
                    level.FastForwardTime(3600);
                    break;
                case 1:
                    Debugger.HudPrint("Fast forward 24 hours");
                    level.FastForwardTime(86400);
                    break;
                case 9:
                    Debugger.HudPrint("Fast forward 24 hours");
                    level.FastForwardTime(60);
                    break;
                case 53:
                    bool state = level.InvulnerabilityEnabled();
                    level.SetInvulnerability(!state);

                    if (state)
                    {
                        Debugger.HudPrint("Invulnerability is OFF");
                    }
                    else
                    {
                        Debugger.HudPrint("Invulnerability is ON");
                    }

                    break;
            }
            return 0;
        }

        /// <summary>
        ///     Decodes this instnace.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            this._debugAction = stream.ReadInt();
            this._stringArg = stream.ReadString(900000);
            this._intArg1 = stream.ReadInt();
            this._intArg2 = stream.ReadInt();

            int intArrayArg1Size = stream.ReadInt();

            if (intArrayArg1Size > 0)
            {
                this._intArrayArg1.EnsureCapacity(intArrayArg1Size);

                for (int i = 0; i < intArrayArg1Size; i++)
                {
                    this._intArrayArg1[i] = stream.ReadInt();
                }
            }

            int intArrayArg2Size = stream.ReadInt();

            if (intArrayArg2Size > 0)
            {
                this._intArrayArg2.EnsureCapacity(intArrayArg2Size);

                for (int i = 0; i < intArrayArg2Size; i++)
                {
                    this._intArrayArg2[i] = stream.ReadInt();
                }
            }

            base.Decode(stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteInt(this._debugAction);
            encoder.WriteString(this._stringArg);
            encoder.WriteInt(this._intArg1);
            encoder.WriteInt(this._intArg2);

            if (this._intArrayArg1 != null)
            {
                encoder.WriteInt(this._intArrayArg1.Count);

                for (int i = 0; i < this._intArrayArg1.Count; i++)
                {
                    encoder.WriteInt(this._intArrayArg1[i]);
                }
            }
            else
            {
                encoder.WriteInt(0);
            }

            if (this._intArrayArg2 != null)
            {
                encoder.WriteInt(this._intArrayArg2.Count);

                for (int i = 0; i < this._intArrayArg2.Count; i++)
                {
                    encoder.WriteInt(this._intArrayArg2[i]);
                }
            }
            else
            {
                encoder.WriteInt(0);
            }

            base.Encode(encoder);
        }

        /// <summary>
        ///     Gets the command type of this instance.
        /// </summary>
        public override int GetCommandType()
        {
            return 1000;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
            this._stringArg = null;
            this._intArrayArg1 = null;
            this._intArrayArg2 = null;
        }

        /// <summary>
        ///     Gets the json for replay.
        /// </summary>
        public override LogicJSONObject GetJSONForReplay()
        {
            LogicJSONObject jsonObject = new LogicJSONObject();

            jsonObject.Put("base", base.GetJSONForReplay());
            jsonObject.Put("dA", new LogicJSONNumber(this._debugAction));
            jsonObject.Put("gi", new LogicJSONNumber(this._intArg1));
            jsonObject.Put("int", new LogicJSONNumber(this._intArg2));

            return jsonObject;
        }

        /// <summary>
        ///     Loads this instance from json.
        /// </summary>
        public override void LoadFromJSON(LogicJSONObject jsonRoot)
        {
            LogicJSONObject baseObject = jsonRoot.GetJSONObject("base");

            if (baseObject != null)
            {
                base.LoadFromJSON(baseObject);
            }
            else
            {
                Debugger.Error("Replay LogicDebugCommand load failed! Base missing!");
            }

            LogicJSONNumber debugActionObject = jsonRoot.GetJSONNumber("dA");

            if (debugActionObject != null)
            {
                this._debugAction = debugActionObject.GetIntValue();
            }

            LogicJSONNumber intArg1Object = jsonRoot.GetJSONNumber("gi");

            if (intArg1Object != null)
            {
                this._intArg1 = intArg1Object.GetIntValue();
            }

            LogicJSONNumber intArg2Object = jsonRoot.GetJSONNumber("int");

            if (intArg2Object != null)
            {
                this._intArg2 = intArg2Object.GetIntValue();
            }
        }
    }
}