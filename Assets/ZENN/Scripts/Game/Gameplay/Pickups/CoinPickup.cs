using System;
using UnityEngine;
using ZENN.Systems;

namespace ZENN.Gameplay
{
    public class CoinPickup : MonoBehaviour
    {
        [Header("Identity")]
        [SerializeField] private string coinId;

        [Header("Movement Settings")]
        [SerializeField] private float bobHeight = 0.25f;
        [SerializeField] private float bobSpeed = 3f;
        [SerializeField] private float rotationSpeed = 45f;

        [Header("Pickup Settings")]
        [SerializeField] private int coinValue = 1;

        private Vector3 startPosition;
        private bool isCollected;

        public string CoinId => coinId;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this))
                return;

            if (string.IsNullOrWhiteSpace(coinId) || IsDuplicateIdInScene())
            {
                coinId = GenerateUniqueCoinId();
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }

        private bool IsDuplicateIdInScene()
        {
            if (string.IsNullOrWhiteSpace(coinId))
                return false;

            var allCoins = FindObjectsByType<CoinPickup>(FindObjectsInactive.Include);
            foreach (var other in allCoins)
            {
                if (other != null && other != this && other.coinId == coinId)
                    return true;
            }

            return false;
        }

        private static string GenerateUniqueCoinId()
        {
            return $"coin_{Guid.NewGuid().ToString("N")[..8]}";
        }
#endif

        private void Awake()
        {
            if (LevelStateManager.Current != null && LevelStateManager.Current.IsCoinCollected(coinId))
            {
                isCollected = true;
                gameObject.SetActive(false);
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            if (isCollected) return;
            startPosition = transform.position;
        }

        private void Update()
        {
            if (isCollected) return;

            float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(startPosition.x, newY, startPosition.z);

            if (rotationSpeed != 0f)
            {
                transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isCollected) return;

            if (other.CompareTag("Player"))
            {
                isCollected = true;
                Services.Wallet?.AddCoins(coinValue);
                LevelStateManager.Current?.MarkCoinCollected(coinId);
                LevelStateManager.Current?.LogDebugState($"Picked up {coinId}");
                Destroy(gameObject);
            }
        }
    }
}