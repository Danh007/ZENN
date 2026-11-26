using UnityEngine;
using UnityEngine.SceneManagement;
using ZENN.Core;

namespace ZENN.Systems
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private EconomyConfig economyConfig;
        [SerializeField] private string firstSceneName = "Home";

        void Awake()
        {
            DontDestroyOnLoad(gameObject);

            Services.Save = new PlayerPrefsSaveService();

            var settingsService = new SettingsService(Services.Save);
            settingsService.Load();
            Services.Settings = settingsService;

            Services.Config = new ScriptableObjectConfigService(economyConfig);

            var walletService = new WalletService(Services.Save, Services.Config);
            walletService.Load();
            Services.Wallet = walletService;

#if UNITY_EDITOR
            gameObject.AddComponent<DebugWalletHotkey>();
#endif

            Debug.Log("Bootstrap: services ready");
            SceneManager.LoadScene("Home");
        }
    }
}