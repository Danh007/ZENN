using UnityEngine;
using ZENN.Core;

namespace ZENN.Systems
{
    public class PlayerPrefsSaveService : ISaveService
    {
        public string GetString(string key, string fallback = "")
            => PlayerPrefs.GetString(key, fallback);

        public void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
            PlayerPrefs.Save();
        }
    }
}
