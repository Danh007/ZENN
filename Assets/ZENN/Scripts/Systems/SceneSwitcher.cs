using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyGame.Systems
{
    public class SceneSwitcher : MonoBehaviour
    {
        [Header("Scene Names")]
        [SerializeField] private string gameSceneName = "GameLevel";
        [SerializeField] private string mainMenuSceneName = "MainMenu";

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
    }
}