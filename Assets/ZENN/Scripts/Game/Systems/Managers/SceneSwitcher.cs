using UnityEngine;
using UnityEngine.SceneManagement;

namespace ZENN.Systems
{
    public class SceneSwitcher : MonoBehaviour
    {
        [Header("Scene Names")]
        [SerializeField] private string gameSceneName = "Level";
        [SerializeField] private string mainMenuSceneName = "Home";
        [SerializeField] private string settingsSceneName = "Settings";
        [SerializeField] private string shopSceneName = "Shop";

        // Call this from the Main Menu Play Button
        public void LoadGameScene()
        {
            SceneManager.LoadScene(gameSceneName);
        }

        // Call this from the Game Level Back Button
        public void LoadMainMenuScene()
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }

        // Call this from the Settings Button
        public void LoadSettingsScene()
        {
            SceneManager.LoadScene(settingsSceneName);
        }

        // Call this from the Shop Button
        public void LoadShopScene()
        {
            SceneManager.LoadScene(shopSceneName);
        }

    }
}