using UnityEngine;

public class FramerateController : MonoBehaviour
{
    [SerializeField] private int targetFPS = 60;

    void Awake()
    {
        // Disable VSync so Application.targetFrameRate is respected
        QualitySettings.vSyncCount = 0;

        // Set the desired framerate
        Application.targetFrameRate = targetFPS;
    }
}
