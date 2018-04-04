namespace RivieraStudio.Magic.Logic.Command
{
    using RivieraStudio.Magic.Logic.Command.Battle;
    using RivieraStudio.Magic.Logic.Command.Home;
    using RivieraStudio.Magic.Logic.Command.Listener;
    using RivieraStudio.Magic.Logic.Command.Server;
    using RivieraStudio.Magic.Logic.Level;
    using RivieraStudio.Magic.Titan.DataStream;
    using RivieraStudio.Magic.Titan.Debug;
    using RivieraStudio.Magic.Titan.Json;
    using RivieraStudio.Magic.Titan.Util;

    public class LogicCommandManager
    {
        private LogicLevel _level;
        private LogicCommandManagerListener _listener;
        private LogicArrayList<LogicCommand> _commandList;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicCommandManager" /> class.
        /// </summary>
        public LogicCommandManager(LogicLevel level)
        {
            this._level = level;
            this._commandList = new LogicArrayList<LogicCommand>();
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public void Destruct()
        {
            if (this._commandList != null)
            {
                if (this._commandList.Count != 0)
                {
                    do
                    {
                        this._commandList[0].Destruct();
                        this._commandList.Remove(0);
                    } while (this._commandList.Count != 0);
                }

                this._commandList = null;
            }

            this._listener = null;
            this._level = null;
        }

        /// <summary>
        ///     Sets the listener of this instance.
        /// </summary>
        public void SetListener(LogicCommandManagerListener listener)
        {
            this._listener = listener;
        }

        /// <summary>
        ///     Adds the specified command.
        /// </summary>
        public void AddCommand(LogicCommand command)
        {
            if (command != null)
            {
                if (this._level.GetState() == 4)
                {
                    command.Destruct();
                    command = null;
                }
                else
                {
                    this._commandList.Add(command);
                }
            }
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public void Decode(ByteStream stream)
        {
            if (this._commandList.Count != 0)
            {
                do
                {
                    this._commandList[0].Destruct();
                    this._commandList.Remove(0);
                } while (this._commandList.Count != 0);
            }

            stream.EnableCheckSum(false);

            for (int i = 0, commandCount = stream.ReadInt(); i < commandCount; i++)
            {
                this._commandList.Add(LogicCommandManager.DecodeCommand(stream));
            }

            stream.EnableCheckSum(true);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public void Encode(ChecksumEncoder encoder)
        {
            encoder.EnableCheckSum(false);
            encoder.WriteInt(this._commandList.Count);

            for (int i = 0; i < this._commandList.Count; i++)
            {
                LogicCommand command = this._commandList[i];

                encoder.WriteInt(command.GetCommandType());
                command.Encode(encoder);
            }

            encoder.EnableCheckSum(true);
        }

        /// <summary>
        ///     Gets a value indicating whether the specified command is allowed in current state.
        /// </summary>
        public bool IsCommandAllowedInCurrentState(LogicCommand command)
        {
            int commandType = command.GetCommandType();
            int state = this._level.GetState();

            if (commandType >= 1000)
            {
                Debugger.Error("Execute command failed! Debug commands are not allowed when debug is off.");
                return false;
            }

            if (commandType >= 500
                && commandType < 700
                && state != 1)
            {
                Debugger.Error("Execute command failed! Command is only allowed in home state.");
                return false;
            }

            if (commandType >= 700
                && commandType < 800
                && state != 2
                && state != 5)
            {
                Debugger.Error("Execute command failed! Command is only allowed in attack state.");
                return false;
            }

            if (state == 4)
            {
                Debugger.Warning("Execute command failed! Commands are not allowed in visit state.");
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Called for update this instance.
        /// </summary>
        public void SubTick()
        {
            int subTick = this._level.GetLogicTime();
            int state = this._level.GetState();

            for (int i = 0; i < this._commandList.Count; i++)
            {
                LogicCommand command = this._commandList[i];

                if (command.GetExecuteSubTick() < subTick)
                {
                    Debugger.Error(string.Format("Execute command failed! Command should have been executed already. (type={0} server_tick={1} command_tick={2}",
                        command.GetCommandType(),
                        subTick,
                        command.GetExecuteSubTick()));
                }

                if (command.GetExecuteSubTick() == subTick)
                {
                    if (this.IsCommandAllowedInCurrentState(command))
                    {
                        int result = command.Execute(this._level);

                        if (result != 0)
                        {
                            Debugger.Warning("Execute command failed, code: " + result);
                        }
                        else
                        {
                            if (this._listener != null)
                            {
                                this._listener.CommandExecuted(command);
                            }
                        }

                        this._commandList.Remove(i--);
                    }
                    else
                    {
                        Debugger.Error(string.Format("Execute command failed! Command not allowed in current state. (type={0} current_state={1}",
                            command.GetCommandType(),
                            this._level.GetState()));
                    }
                }
            }
        }

        /// <summary>
        ///     Creates a command instance by type.
        /// </summary>
        public static LogicCommand CreateCommand(int type)
        {
            LogicCommand command = null;

            if (type >= 1000)
            {
                if (type == 1000)
                {
                    Debugger.Error("LogicCommandManager::createCommand() - Debug command is not allowed when debug is off.");
                    return null;
                }

                Debugger.Error("LogicCommandManager::createCommand() - Unknown command type: " + type);
            }

            if (type < 500)
            {
                switch (type)
                {
                    case 1:
                        command = new LogicJoinAllianceCommand();
                        break;
                    case 2:
                        command = new LogicLeaveAllianceCommand();
                        break;
                    case 3:
                        command = new LogicChangeAvatarNameCommand();
                        break;
                    case 7:
                        command = new LogicDiamondsAddedCommand();
                        break;

                    default:
                    {
                        Debugger.Error("LogicCommandManager::createCommand() - Unknown command type: " + type);
                        break;
                    }
                }
            }
            else
            {
                switch (type)
                {
                    case 500:
                    {
                        command = new LogicBuyBuildingCommand();
                        break;
                    }

                    case 501:
                    {
                        command = new LogicMoveBuildingCommand();
                        break;
                    }

                    case 502:
                    {
                        command = new LogicUpgradeBuildingCommand();
                        break;
                    }

                    case 504:
                    {
                        command = new LogicSpeedUpConstructionCommand();
                        break;
                    }

                    case 506:
                    {
                        command = new LogicCollectResourcesCommand();
                        break;
                    }

                    case 507:
                    {
                        command = new LogicClearObstacleCommand();
                        break;
                    }

                    case 508:
                    {
                        command = new LogicTrainUnitCommand();
                        break;
                    }

                    case 518:
                    {
                        command = new LogicBuyResourcesCommand();
                        break;
                    }

                    case 519:
                    {
                        command = new LogicMissionProgressCommand();
                        break;
                    }

                    case 520:
                    {
                        command = new LogicUnlockBuildingCommand();
                        break;
                    }

                    case 521:
                    {
                        command = new LogicFreeWorkerCommand();
                        break;
                    }

                    case 523:
                    {
                        command = new LogicClaimAchievementRewardCommand();
                        break;
                    }

                    case 532:
                    {
                        command = new LogicNewShopItemsSeenCommand();
                        break;
                    }

                    case 533:
                    {
                        command = new LogicMoveMultipleBuildingCommand();
                        break;
                    }

                    case 539:
                    {
                        command = new LogicNewsSeenCommand();
                        break;
                    }

                    case 544:
                    {
                        command = new LogicEditModeShownCommand();
                        break;
                    }

                    case 549:
                    {
                        command = new LogicUpgradeMultipleBuildingCommand();
                        break;
                    }

                    case 550:
                    {
                        command = new LogicRemoveUnitsCommand();
                        break;
                    }

                    case 577:
                    {
                        command = new LogicSwapBuildingCommand();
                        break;
                    }

                    case 604:
                    {
                        command = new LogicSeenBuilderMenuCommand();
                        break;
                    }

                    case 700:
                    {
                        command = new LogicPlaceAttackerCommand();
                        break;
                    }

                    default:
                    {
                        Debugger.Error("LogicCommandManager::createCommand() - Unknown command type: " + type);
                        break;
                    }
                }
            }

            return command;
        }

        /// <summary>
        ///     Decodes a command.
        /// </summary>
        public static LogicCommand DecodeCommand(ByteStream stream)
        {
            LogicCommand command = LogicCommandManager.CreateCommand(stream.ReadInt());

            if (command == null)
            {
                Debugger.Error("LogicCommandManager::decodeCommand() - command is null");
            }
            else
            {
                command.Decode(stream);
            }

            return command;
        }

        /// <summary>
        ///     Encodes a command.
        /// </summary>
        public static void EncodeCommand(ChecksumEncoder encoder, LogicCommand command)
        {
            encoder.WriteInt(command.GetCommandType());
            command.Encode(encoder);
        }

        /// <summary>
        ///     Loads command from json.
        /// </summary>
        public static LogicCommand LoadCommandFromJSON(LogicJSONObject jsonObject)
        {
            LogicJSONNumber jsonNumber = jsonObject.GetJSONNumber("ct");

            if (jsonNumber == null)
            {
                Debugger.Error("loadCommandFromJSON - Unknown command type");
            }
            else
            {
                LogicCommand command = LogicCommandManager.CreateCommand(jsonNumber.GetJSONNodeType());

                if (command != null)
                {
                    command.LoadFromJSON(jsonObject.GetJSONObject("c"));
                }

                return command;
            }

            return null;
        }

        /// <summary>
        ///     Saves the specified command to json.
        /// </summary>
        public static void SaveCommandToJSON(LogicJSONObject jsonObject, LogicCommand command)
        {
            jsonObject.Put("ct", new LogicJSONNumber(command.GetCommandType()));
            jsonObject.Put("c", command.GetJSONForReplay());
        }
    }
}