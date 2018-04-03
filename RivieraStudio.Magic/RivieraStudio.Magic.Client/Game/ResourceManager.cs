namespace ClashersRepublic.Magic.Client.Game
{
    using System.IO;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Titan.Json;

    internal class ResourceManager
    {
        internal static string FingerprintJson;
        internal static string FingerprintSha;
        internal static string FingerprintVersion;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            ResourceManager.FingerprintJson = File.ReadAllText("Assets/fingerprint.json");

            LogicJSONObject jsonObject = (LogicJSONObject) LogicJSONParser.Parse(ResourceManager.FingerprintJson);

            ResourceManager.FingerprintSha = LogicJSONHelper.GetJSONString(jsonObject, "sha");
            ResourceManager.FingerprintVersion = LogicJSONHelper.GetJSONString(jsonObject, "version");
        }
    }
}