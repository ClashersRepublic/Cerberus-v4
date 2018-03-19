namespace ClashersRepublic.Magic.Logic.Command.Battle
{
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.GameObject;
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Titan.DataStream;

    public sealed class LogicPlaceAttackerCommand : LogicCommand
    {
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
            base.Decode(stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            base.Encode(encoder);
        }

        /// <summary>
        ///     Gets the command type of this instance.
        /// </summary>
        public override int GetCommandType()
        {
            return 800;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
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