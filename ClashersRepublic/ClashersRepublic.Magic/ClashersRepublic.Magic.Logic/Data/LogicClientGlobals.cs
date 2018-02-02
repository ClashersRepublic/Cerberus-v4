namespace ClashersRepublic.Magic.Logic.Data
{
    public class LogicClientGlobals
    {
        private bool _pepperEnabled;

        /// <summary>
        ///     Creates references.
        /// </summary>
        public void CreateReferences()
        {
            this._pepperEnabled = this.GetBoolValue("PEPPER_ENABLED");
        }

        /// <summary>
        ///     Gets the data instance by the name.
        /// </summary>
        private LogicGlobalData GetGlobalData(string name)
        {
            return LogicDataTables.GetClientGlobalByName(name);
        }

        /// <summary>
        ///     Gets the boolean value of specified data name.
        /// </summary>
        private bool GetBoolValue(string name)
        {
            return this.GetGlobalData(name).BooleanValue;
        }

        /// <summary>
        ///     Gets the integer value of specified data name.
        /// </summary>
        private int GetIntValue(string name)
        {
            return this.GetGlobalData(name).NumberValue; 
        }

        /// <summary>
        ///     Gets a value indicating whether the pepper cryptography is enabled.
        /// </summary>
        public bool PepperEnabled()
        {
            return this._pepperEnabled;
        }
    }
}