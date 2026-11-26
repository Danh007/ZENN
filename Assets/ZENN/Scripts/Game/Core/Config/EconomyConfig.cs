using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Economy Config")]
public class EconomyConfig : ScriptableObject
{
    [Serializable]
    public struct SkinCostEntry
    {
        [Tooltip("Must match SkinDefinition.id (e.g., 'default', 'blue_shader').")]
        public string skinId;

        [Min(0)]
        public int cost;
    }

    [Header("First Boot")]
    public int startingCoins = 100;

    [Tooltip("Optional list for mapping additional skin IDs to centralized prices.")]
    public List<SkinCostEntry> skinCosts = new();

    [Header("Sources")]
    public int coinsPerLevel = 50;
    public int dailyGift = 50;

    [Header("Upgrades")]
    public int upgradeBaseCost = 100;
    public float upgradeGrowth = 1.5f;
    public int upgradeMaxLevel = 10;

    public int UpgradeCost(int level) =>
        Mathf.RoundToInt(upgradeBaseCost * Mathf.Pow(upgradeGrowth, level));

    public int GetSkinCost(string skinId, int fallbackCost = 0)
    {
        int safeFallback = Mathf.Max(0, fallbackCost);

        if (string.IsNullOrWhiteSpace(skinId))
            return safeFallback;

        string normalizedId = skinId.Trim();

        for (int i = 0; i < skinCosts.Count; i++)
        {
            SkinCostEntry entry = skinCosts[i];
            if (!string.IsNullOrWhiteSpace(entry.skinId) &&
                string.Equals(entry.skinId.Trim(), normalizedId, StringComparison.OrdinalIgnoreCase))
            {
                return Mathf.Max(0, entry.cost);
            }
        }

        return safeFallback;
    }
}