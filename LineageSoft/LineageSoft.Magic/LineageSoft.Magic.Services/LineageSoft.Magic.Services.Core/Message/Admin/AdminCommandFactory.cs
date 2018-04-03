namespace LineageSoft.Magic.Services.Core.Message.Admin
{
    public class AdminCommandFactory
    {
        /// <summary>
        ///     Creates a debug command by type.
        /// </summary>
        public static AdminCommand CreateDebugCommandByType(int type)
        {
            switch (type)
            {
                case 1000: return new AdminResetZoneCommand();
                case 1001: return new AdminAddDiamondsCommand();
                default: return null;
            }
        }

        /// <summary>
        ///     Creates a debug command by name.
        /// </summary>
        public static AdminCommand CreateDebugCommandByName(string cmd)
        {
            switch (cmd)
            {
                case "reset acc": return new AdminResetZoneCommand();
                case "add diamonds": return new AdminAddDiamondsCommand();
                default: return null;
            }
        }
    }
}