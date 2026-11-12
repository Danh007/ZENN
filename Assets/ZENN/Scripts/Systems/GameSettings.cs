using UnityEngine;

namespace MyGame.Systems
{
    public class GameSettingsManager : MonoBehaviour
    {
        [Header("Performance Defaults")]
        [SerializeField] private int targetFrameRate = 60;
        [SerializeField] private bool enableVSync = false;

        private void Awake()
        {
            ApplySettings();
        }

        public void ApplySettings()
        {
            // Set VSync (0 = Off, 1 = On every vertical blank)
            QualitySettings.vSyncCount = enableVSync ? 1 : 0;

            // Set Target Frame Rate
            Application.targetFrameRate = targetFrameRate;

            Debug.Log($"[GameSettings] VSync: {enableVSync}, Target FPS: {targetFrameRate}");
        }

        // Future settings can easily be added here as clean methods:
        public void SetTargetFPS(int newFps)
        {
            targetFrameRate = newFps;
            Application.targetFrameRate = targetFrameRate;
        }
    }
}