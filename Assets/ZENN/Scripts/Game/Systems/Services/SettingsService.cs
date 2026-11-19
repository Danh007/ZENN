using UnityEngine;
using ZENN.Core;

namespace ZENN.Systems
{
    public class SettingsService : ISettingsService
    {
        private const string SettingsKey = "settings.data";

        private readonly ISaveService save;

        public GameSettingsData Current { get; private set; }

        public SettingsService(ISaveService save)
        {
            this.save = save;
        }

        public void Load()
        {
            GameSettingsData loaded = GameSettingsData.Default;
            string json = save.GetString(SettingsKey);
            Debug.Log($"[SettingsService] LOAD raw json: '{json}'");

            if (!string.IsNullOrEmpty(json))
            {
                object boxed = loaded;
                JsonUtility.FromJsonOverwrite(json, boxed);
                loaded = (GameSettingsData)boxed;
            }

            Current = loaded;
            Debug.Log($"[SettingsService] LOAD result VSync: {Current.VSync}");
            ApplyToUnity(Current);
        }

        public void Apply(GameSettingsData data)
        {
            Current = data;
            ApplyToUnity(data);
            Save(data);
        }

        private void ApplyToUnity(GameSettingsData data)
        {
            QualitySettings.vSyncCount = data.VSync ? 1 : 0;
        }

        private void Save(GameSettingsData data)
        {
            string json = JsonUtility.ToJson(data);
            Debug.Log($"[SettingsService] SAVE json: '{json}'");
            save.SetString(SettingsKey, json);
        }
    }
}
