﻿namespace RivieraStudio.Magic.Logic.Mode
{
    using RivieraStudio.Magic.Logic.Command;
    using RivieraStudio.Magic.Logic.Data;

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

        public virtual void NotEnoughWorkers(LogicCommand command, int villageType)
        {
        }

        public virtual void NotEnoughResources(LogicResourceData data, int count, LogicCommand command, bool unk)
        {
        }

        public virtual void NotEnoughDiamonds()
        {
        }

        public virtual void BuildingCapReached(LogicBuildingData data)
        {
        }

        public virtual void TrapCapReached(LogicTrapData data)
        {
        }

        public virtual void AllianceCreated()
        {
        }

        public virtual void AllianceJoined()
        {
        }

        public virtual void AllianceLeft()
        {
        }

        public virtual void LevelUp(int expLevel)
        {
        }

        public virtual void ShowTroopPlacementTutorial(int data)
        {
        }

        public virtual void TownHallLevelTooLow(int lvl)
        {
        }

        public virtual void AchievementCompleted(LogicAchievementData data)
        {
        }

        public virtual void AchievementProgress(LogicAchievementData data)
        {
        }

        public virtual void DiamondsBought()
        {
        }
    }
}