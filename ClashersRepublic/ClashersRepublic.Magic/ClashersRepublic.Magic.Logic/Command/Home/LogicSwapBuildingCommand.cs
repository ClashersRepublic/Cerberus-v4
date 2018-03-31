namespace ClashersRepublic.Magic.Logic.Command.Home
{
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.GameObject;
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Titan.DataStream;

    public sealed class LogicSwapBuildingCommand : LogicCommand
    {
        private int _gameObject1;
        private int _gameObject2;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicSwapBuildingCommand"/> class.
        /// </summary>
        public LogicSwapBuildingCommand()
        {
            // LogicSwapBuildingCommand.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicSwapBuildingCommand"/> class.
        /// </summary>
        public LogicSwapBuildingCommand(int gameObject1, int gameObject2)
        {
            this._gameObject1 = gameObject1;
            this._gameObject2 = gameObject2;
        }

        /// <summary>
        ///     Decodes this instnace.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            this._gameObject1 = stream.ReadInt();
            this._gameObject2 = stream.ReadInt();

            base.Decode(stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteInt(this._gameObject1);
            encoder.WriteInt(this._gameObject2);

            base.Encode(encoder);
        }

        /// <summary>
        ///     Gets the command type of this instance.
        /// </summary>
        public override int GetCommandType()
        {
            return 577;
        }

        /// <summary>
        ///     Executes this instance.
        /// </summary>
        public override int Execute(LogicLevel level)
        {
            if (LogicDataTables.GetGlobals().UseSwapBuildings())
            {
                if (this._gameObject1 != this._gameObject2)
                {
                    LogicGameObject gameObject1 = level.GetGameObjectManager().GetGameObjectByID(this._gameObject1);
                    LogicGameObject gameObject2 = level.GetGameObjectManager().GetGameObjectByID(this._gameObject2);

                    if (gameObject1 != null)
                    {
                        if (gameObject2 != null)
                        {
                            int gameObjectType1 = gameObject1.GetGameObjectType();

                            if (gameObjectType1 <= 6)
                            {
                                if (gameObjectType1 == 0 || gameObjectType1 == 4 || gameObjectType1 == 6)
                                {
                                    int gameObjectType2 = gameObject2.GetGameObjectType();

                                    if (gameObjectType2 <= 6)
                                    {
                                        if (gameObjectType2 == 0 || gameObjectType2 == 4 || gameObjectType2 == 6)
                                        {
                                            int width1 = gameObject1.GetWidthInTiles();
                                            int width2 = gameObject2.GetWidthInTiles();
                                            int height1 = gameObject1.GetHeightInTiles();
                                            int height2 = gameObject2.GetHeightInTiles();

                                            if (width1 == width2 && height1 == height2)
                                            {
                                                if (gameObject1.GetGameObjectType() == 0)
                                                {
                                                    if (((LogicBuilding) gameObject1).IsLocked())
                                                    {
                                                        return -6;
                                                    }

                                                    if (gameObject1.IsWall())
                                                    {
                                                        return -7;
                                                    }
                                                }

                                                if (gameObject2.GetGameObjectType() == 0)
                                                {
                                                    if (((LogicBuilding) gameObject2).IsLocked())
                                                    {
                                                        return -8;
                                                    }

                                                    if (gameObject2.IsWall())
                                                    {
                                                        return -9;
                                                    }
                                                }

                                                int x1 = gameObject1.GetX();
                                                int y1 = gameObject1.GetY();
                                                int x2 = gameObject2.GetX();
                                                int y2 = gameObject2.GetY();

                                                gameObject1.SetPositionXY(x2, y2);
                                                gameObject2.SetPositionXY(x1, y1);

                                                return 0;
                                            }

                                            return -5;
                                        }
                                    }

                                    return -4;
                                }
                            }

                            return -3;
                        }

                        return -2;
                    }

                    return -1;
                }

                return -98;
            }

            return -99;
        }
    }
}