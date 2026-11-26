using System.Collections.Generic;
using UnityEngine;

namespace ZENN.Core
{
    [CreateAssetMenu(menuName = "Game/Economy Config")]
    public class EconomyConfig : ScriptableObject
    {
        [Header("First Boot")]
        public int startingCoins = 100;

        [Header("Sources")]
        public int coinsPerLevel = 50;
        public int dailyGift = 50;

        [Header("Upgrades")]
        public int upgradeBaseCost = 100;
        public float upgradeGrowth = 1.5f;
        public int upgradeMaxLevel = 10;

        // Placeholder for future items:
        // [Header("Items")]
        // public List<ItemConfig> items = new();

        public int UpgradeCost(int level) =>
            Mathf.RoundToInt(upgradeBaseCost * Mathf.Pow(upgradeGrowth, level));
    }
}