namespace LineageSoft.Magic.Services.Core.Game.Avatar.Change
{
    using LineageSoft.Magic.Logic.Avatar;
    using LineageSoft.Magic.Logic.Avatar.Change;
    using LineageSoft.Magic.Titan.DataStream;

    public class NameAvatarChange : LogicAvatarChange
    {
        private string _name;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NameAvatarChange"/> class.
        /// </summary>
        public NameAvatarChange()
        {
            // NameAvatarChange.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NameAvatarChange"/> class.
        /// </summary>
        public NameAvatarChange(string name)
        {
            this._name = name;
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            base.Decode(stream);
            this._name = stream.ReadString(900000);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            base.Encode(encoder);
            encoder.WriteString(this._name);
        }

        /// <summary>
        ///     Applies the avatar change to the specified <see cref="LogicClientAvatar"/> instance.
        /// </summary>
        public override void ApplyAvatarChange(LogicClientAvatar avatar)
        {
            avatar.SetName(this._name);
        }

        /// <summary>
        ///     Gets the avatar change type.
        /// </summary>
        public override int GetAvatarChangeType()
        {
            return 3;
        }
    }
}