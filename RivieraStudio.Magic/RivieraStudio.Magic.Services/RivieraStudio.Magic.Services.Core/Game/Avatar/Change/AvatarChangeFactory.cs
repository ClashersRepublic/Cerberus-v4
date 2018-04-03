namespace RivieraStudio.Magic.Services.Core.Game.Avatar.Change
{
    using RivieraStudio.Magic.Logic.Avatar.Change;

    public class AvatarChangeFactory
    {
        /// <summary>
        ///     Creates a <see cref="LogicAvatarChange"/> by type.
        /// </summary>
        public static LogicAvatarChange CreateAvatarChangeByType(int type)
        {
            switch (type)
            {
                case 1: return new ExpLevelAvatarChange();
                case 2: return new ScoreAvatarChange();
                case 3: return new NameAvatarChange();
                default: return null;
            }
        }
    }
}