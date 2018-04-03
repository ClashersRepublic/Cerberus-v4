namespace RivieraStudio.Magic.Services.Core.Game.Avatar.Change
{
    using RivieraStudio.Magic.Logic.Avatar;
    using RivieraStudio.Magic.Logic.Avatar.Change;
    using RivieraStudio.Magic.Titan.DataStream;

    public class ScoreAvatarChange : LogicAvatarChange
    {
        private int _score;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ScoreAvatarChange"/> class.
        /// </summary>
        public ScoreAvatarChange()
        {
            // ScoreAvatarChange.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ScoreAvatarChange"/> class.
        /// </summary>
        public ScoreAvatarChange(int score)
        {
            this._score = score;
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            base.Decode(stream);
            this._score = stream.ReadVInt();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            base.Encode(encoder);
            encoder.WriteVInt(this._score);
        }

        /// <summary>
        ///     Applies the avatar change to the specified <see cref="LogicClientAvatar"/> instance.
        /// </summary>
        public override void ApplyAvatarChange(LogicClientAvatar avatar)
        {
            avatar.SetScore(this._score);
        }

        /// <summary>
        ///     Gets the avatar change type.
        /// </summary>
        public override int GetAvatarChangeType()
        {
            return 2;
        }
    }
}