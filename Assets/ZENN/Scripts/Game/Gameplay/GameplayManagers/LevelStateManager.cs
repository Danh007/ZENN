using UnityEngine;
using UnityEngine.SceneManagement;
using ZENN.Core;

namespace ZENN.Gameplay
{
    public class LevelStateManager : MonoBehaviour
    {
        private const string SaveKeyPrefix = "level_state_";

        public static LevelStateManager Current { get; private set; }

        [Header("Level Identity")]
        [Tooltip("Leave empty to use the active scene name.")]
        [SerializeField] private string levelId;

        [Header("Scene References")]
        [SerializeField] private SphereBody player;

        [Header("Position Sampling")]
        [Tooltip("How often (seconds) to sample the player's position & grounded state in memory.")]
        [SerializeField] private float sampleInterval = 0.25f;

        private LevelSaveData currentData;
        private float sampleTimer;
        private bool isDirty;

        public string LevelId => string.IsNullOrWhiteSpace(levelId)
            ? SceneManager.GetActiveScene().name
            : levelId;

        private string SaveKey => $"{SaveKeyPrefix}{LevelId}";

        private void Awake()
        {
            if (Current != null && Current != this)
            {
                Debug.LogWarning($"Duplicate LevelStateManager in scene '{SceneManager.GetActiveScene().name}'. Replacing reference.", this);
            }
            Current = this;

            if (player == null)
            {
                player = FindAnyObjectByType<SphereBody>();
            }

            LoadLevelData();
        }

        private void Start()
        {
            RestorePlayerPositionIfSaved();
        }

        private void Update()
        {
            if (player == null || currentData == null) return;

            sampleTimer += Time.deltaTime;
            if (sampleTimer >= sampleInterval)
            {
                sampleTimer = 0f;
                SamplePlayerState();
            }
        }

        public bool IsCoinCollected(string coinId)
        {
            return currentData != null && currentData.IsCoinCollected(coinId);
        }

        public void MarkCoinCollected(string coinId)
        {
            if (currentData == null || string.IsNullOrWhiteSpace(coinId)) return;
            if (currentData.IsCoinCollected(coinId)) return;

            currentData.MarkCoinCollected(coinId);
            isDirty = true;
        }

        public void SaveNow()
        {
            if (currentData == null) return;

            SamplePlayerState();

            if (ZENN.Systems.Services.Save == null)
            {
                Debug.LogWarning("[LevelStateManager] Services.Save is null; skipping save.", this);
                return;
            }

            string json = JsonUtility.ToJson(currentData);
            LogDebugState("Before SaveNow");
            ZENN.Systems.Services.Save.SetString(SaveKey, json);
            isDirty = false;
        }

        public void ResetLevelForFreshRun(bool saveImmediately = true)
        {
            if (currentData == null)
            {
                currentData = new LevelSaveData(LevelId);
            }
            else
            {
                currentData.ResetForFreshRun();
            }

            isDirty = true;

            if (saveImmediately)
            {
                SaveNow();
            }
        }

        private void LoadLevelData()
        {
            currentData = new LevelSaveData(LevelId);

            if (ZENN.Systems.Services.Save == null)
            {
                Debug.LogWarning("[LevelStateManager] Services.Save is null on load; starting with fresh in-memory level data.", this);
                return;
            }

            string json = ZENN.Systems.Services.Save.GetString(SaveKey, string.Empty);
            if (string.IsNullOrWhiteSpace(json)) return;

            try
            {
                var loaded = JsonUtility.FromJson<LevelSaveData>(json);
                if (loaded != null)
                {
                    currentData = loaded;
                    if (string.IsNullOrWhiteSpace(currentData.levelId))
                    {
                        currentData.levelId = LevelId;
                    }
                }
            }
            catch
            {
                Debug.LogWarning($"[LevelStateManager] Failed to parse save data for '{LevelId}'. Starting fresh.", this);
                currentData = new LevelSaveData(LevelId);
            }
            LogDebugState("After LoadLevelData");
        }

        private void RestorePlayerPositionIfSaved()
        {
            if (player == null || currentData == null) return;

            if (currentData.TryGetRestorePosition(out Vector3 restorePos))
            {
                player.TeleportTo(restorePos);
            }
        }

        private void SamplePlayerState()
        {
            if (player == null || currentData == null) return;

            Vector3 pos = player.transform.position;
            bool grounded = player.IsGrounded;

            bool positionChanged = !currentData.hasLastPosition ||
                                   (currentData.lastPosition - pos).sqrMagnitude > 0.0001f;
            bool groundedChanged = grounded && (!currentData.hasGroundedPosition ||
                                                (currentData.lastGroundedPosition - pos).sqrMagnitude > 0.0001f);

            currentData.RecordPosition(pos, grounded);

            if (positionChanged || groundedChanged)
            {
                isDirty = true;
            }
        }

        private void OnDestroy()
        {
            if (isDirty)
            {
                SaveNow();
            }

            if (Current == this)
            {
                Current = null;
            }
        }
        public void LogDebugState(string context = "")
        {
            string prefix = string.IsNullOrWhiteSpace(context) ? "[LevelStateManager]" : $"[LevelStateManager | {context}]";
            if (currentData == null)
            {
                Debug.Log($"{prefix} currentData is NULL (isDirty={isDirty})", this);
                return;
            }

            Debug.Log($"{prefix} isDirty={isDirty} | {currentData.ToDebugString()}", this);
        }
    }
}