namespace RivieraStudio.Magic.Logic.Offer
{
    using RivieraStudio.Magic.Titan.Json;

    public class LogicOfferManager
    {
        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public void Destruct()
        {
            // Destruct.
        }

        /// <summary>
        ///     Loads this instance from json.
        /// </summary>
        public void Load(LogicJSONObject root)
        {
            LogicJSONObject jsonObject = root.GetJSONObject("offer");

            if (jsonObject != null)
            {
            }
        }

        /// <summary>
        ///     Saves this instance to json.
        /// </summary>
        public void Save(LogicJSONObject root)
        {
            LogicJSONObject jsonObject = new LogicJSONObject();

            root.Put("offer", jsonObject);
        }

        /// <summary>
        ///     Ticks for update this instance.
        /// </summary>
        public void Tick()
        {
            // Tick.
        }
    }
}