namespace ClashersRepublic.Magic.Logic.Command.Battle
{
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.GameObject;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Titan.DataStream;

    public sealed class LogicPlaceAttackerCommand : LogicCommand
    {
        private int _x;
        private int _y;
        private LogicCharacterData _data;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicPlaceAttackerCommand" /> class.
        /// </summary>
        public LogicPlaceAttackerCommand()
        {
            // LogicPlaceAttackerCommand.
        }
        
        /// <summary>
        ///     Decodes this instnace.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            this._x = stream.ReadInt();
            this._y = stream.ReadInt();
            this._data = (LogicCharacterData) stream.ReadDataReference(3);

            base.Decode(stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteInt(this._x);
            encoder.WriteInt(this._y);
            encoder.WriteDataReference(this._data);

            base.Encode(encoder);
        }

        /// <summary>
        ///     Gets the command type of this instance.
        /// </summary>
        public override int GetCommandType()
        {
            return 700;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
            this._data = null;
        }

        /// <summary>
        ///     Places the specified attacker.
        /// </summary>
        public static LogicCharacter PlaceAttacker(LogicAvatar avatar, LogicCharacterData characterData, LogicLevel level, int x, int y)
        {
            LogicCharacter character = (LogicCharacter) LogicGameObjectFactory.CreateGameObject(characterData, level, level.GetVillageType());
            // TODO: Implement LogicPlaceAttackerCommand::placeAttacker.
            return character;
        }
    }
}