namespace ZENN.Core
{
    [System.Serializable]
    public struct GameSettingsData
    {
        public bool VSync;
        // Add new settings here, one field each.

        // The one place defaults live.
        public static GameSettingsData Default => new GameSettingsData
        {
            VSync = false
        };
    }
}