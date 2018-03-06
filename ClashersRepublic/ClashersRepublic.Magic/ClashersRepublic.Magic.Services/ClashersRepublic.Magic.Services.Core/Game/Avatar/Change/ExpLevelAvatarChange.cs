namespace ClashersRepublic.Magic.Services.Core.Game.Avatar.Change
{
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Avatar.Change;
    using ClashersRepublic.Magic.Titan.DataStream;

    public class ExpLevelAvatarChange : LogicAvatarChange
    {
        private int _expLevel;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExpLevelAvatarChange"/> class.
        /// </summary>
        public ExpLevelAvatarChange()
        {
            // ExpLevelAvatarChange.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExpLevelAvatarChange"/> class.
        /// </summary>
        public ExpLevelAvatarChange(int expLevel)
        {
            this._expLevel = expLevel;
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            base.Decode(stream);
            this._expLevel = stream.ReadVInt();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            base.Encode(encoder);
            encoder.WriteVInt(this._expLevel);
        }

        /// <summary>
        ///     Applies the avatar change to the specified <see cref="LogicClientAvatar"/> instance.
        /// </summary>
        public override void ApplyAvatarChange(LogicClientAvatar avatar)
        {
            avatar.SetExpLevel(this._expLevel);
        }

        /// <summary>
        ///     Gets the avatar change type.
        /// </summary>
        public override int GetAvatarChangeType()
        {
            return 1;
        }
    }
}