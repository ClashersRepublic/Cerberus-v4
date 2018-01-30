namespace ClashersRepublic.Magic.Logic.Offer
{
    using ClashersRepublic.Magic.Titan.Json;

    public class LogicOfferManager
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicOfferManager"/> class.
        /// </summary>
        public LogicOfferManager()
        {

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
    }
}