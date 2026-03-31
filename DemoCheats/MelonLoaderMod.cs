using MelonLoader;

namespace DemoCheats
{
    public static class BuildInfo
    {
        public const string Name = "DemoCheats";
        public const string Author = "REPLACE_ME";
        public const string Company = null;
        public const string Version = "1.0.0";
        public const string DownloadLink = null;
    }

    public class DemoCheats : MelonMod
    {
        public override void OnApplicationStart()
        {
            MelonLogger.Msg("OnApplicationStart");
        }

        public override void OnSceneWasInitialized(int buildindex, string sceneName)
        {
            MelonLogger.Msg("OnSceneWasInitialized: " + buildindex.ToString() + " | " + sceneName);
        }

        public override void OnUpdate()
        {
            MelonLogger.Msg("OnUpdate");
        }
    }
}
