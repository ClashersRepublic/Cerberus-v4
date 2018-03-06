namespace ClashersRepublic.Magic.Logic.Mode
{
    using ClashersRepublic.Magic.Logic.Command;
    using ClashersRepublic.Magic.Logic.Data;

    public class LogicGameListener
    {
        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public void Destruct()
        {
            // Destruct.
        }

        public virtual void ReplayFailed()
        {
        }

        public virtual void NotEnoughResources(LogicResourceData data, int count, LogicCommand command, bool unk)
        {
        }

        public virtual void NotEnoughDiamonds()
        {
        }

        public virtual void AllianceCreated()
        {
        }

        public virtual void AllianceJoined()
        {
        }

        public virtual void LevelUp(int expLevel)
        {
        }
    }
}