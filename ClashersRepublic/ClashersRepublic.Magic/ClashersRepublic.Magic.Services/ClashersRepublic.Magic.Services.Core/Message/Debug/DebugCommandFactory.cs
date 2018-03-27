namespace ClashersRepublic.Magic.Services.Core.Message.Debug
{
    public class DebugCommandFactory
    {
        /// <summary>
        ///     Creates a debug command by type.
        /// </summary>
        public static DebugCommand CreateDebugCommandByType(int type)
        {
            switch (type)
            {
                case 1000: return new DebugResetZoneCommand();
                default: return null;
            }
        }

        /// <summary>
        ///     Creates a debug command by name.
        /// </summary>
        public static DebugCommand CreateDebugCommandByName(string cmd)
        {
            switch (cmd)
            {
                case "reset acc": return new DebugResetZoneCommand();
                default: return null;
            }
        }
    }
}