namespace LineageSoft.Magic.Logic.GameObject.Listener
{
    using LineageSoft.Magic.Logic.Data;

    public class LogicGameObjectListener
    {
        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public virtual void Destruct()
        {
            // Destruct.
        }

        /// <summary>
        ///     Refreshes the gameobject positon from logic.
        /// </summary>
        public virtual void RefreshPositionFromLogic()
        {
            // RefreshPositionFromLogic.
        }

        /// <summary>
        ///     Refreshes the gameobject state.
        /// </summary>
        public virtual void RefreshState()
        {
            // RefreshState.
        }

        /// <summary>
        ///     Randomizes the starting frame.
        /// </summary>
        public virtual void RandomizeStartingFrame()
        {
            // RandomizeStartingFrame.
        }

        /// <summary>
        ///     Called when the gameobject was damaged.
        /// </summary>
        public virtual void Damaged()
        {
            // Damaged.
        }

        /// <summary>
        ///     Called when resources in gameobject was collected.
        /// </summary>
        public virtual void ResourcesCollected(LogicResourceData data, int count, bool unk)
        {
            // ResourcesCollected.
        }

        /// <summary>
        ///     Refreshes the resource count.
        /// </summary>
        public virtual void RefreshResourceCount()
        {
            // RefreshResourceCount.
        }

        /// <summary>
        ///     Called when experience was gained.
        /// </summary>
        public virtual void XpGained(int count)
        {
            // XpGained.
        }

        /// <summary>
        ///     Called the gameobject was loaded from json..
        /// </summary>
        public virtual void LoadedFromJSON()
        {
            // XpGained.
        }

        /// <summary>
        ///     Called when the map is unlocked.
        /// </summary>
        public virtual void MapUnlocked()
        {
            // MapUnlocked.
        }

        /// <summary>
        ///     Called when a extra character is added.
        /// </summary>
        public virtual void ExtraCharacterAdded(LogicCharacterData character, LogicBuilding baseBuilding)
        {
            // ExtraCharacterAdded.
        }
    }
}