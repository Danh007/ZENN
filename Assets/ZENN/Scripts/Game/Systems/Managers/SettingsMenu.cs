using UnityEngine;
using UnityEngine.UI;
using ZENN.Core;
using ZENN.Systems;

namespace ZENN.Gameplay
{
    public class SettingsMenu : MonoBehaviour
    {
        [SerializeField] private Toggle vsyncToggle;

        private void OnEnable()
        {
            if (vsyncToggle == null)
            {
                Debug.LogError("SettingsMenu: vsyncToggle is not assigned in the Inspector.", this);
                return;
            }

            if (Services.Settings == null)
            {
                Debug.LogError("SettingsMenu: Services.Settings is null. Did Bootstrap run first?", this);
                return;
            }

            vsyncToggle.onValueChanged.AddListener(OnVSyncChanged);
            SyncUI();
        }

        private void OnDisable()
        {
            if (vsyncToggle != null)
                vsyncToggle.onValueChanged.RemoveListener(OnVSyncChanged);
        }

        private void SyncUI()
        {
            vsyncToggle.SetIsOnWithoutNotify(Services.Settings.Current.VSync);
        }

        private void OnVSyncChanged(bool isOn)
        {
            if (Services.Settings == null) return;

            GameSettingsData updated = Services.Settings.Current;
            updated.VSync = isOn;
            Services.Settings.Apply(updated);
        }
    }
}
