using UnityEngine;
using UnityEngine.SceneManagement;
using ZENN.Core;

namespace ZENN.Systems
{
    public class Bootstrap : MonoBehaviour
    {
        void Awake()
        {
            DontDestroyOnLoad(gameObject);

            Services.Save = new PlayerPrefsSaveService();

            var settingsService = new SettingsService(Services.Save);
            settingsService.Load();
            Services.Settings = settingsService;

            Debug.Log("Bootstrap: services ready");
            SceneManager.LoadScene("MainMenu");
        }
    }
}
