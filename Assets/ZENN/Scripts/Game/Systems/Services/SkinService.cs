using System;
using System.Collections.Generic;
using UnityEngine;
using ZENN.Core;

namespace ZENN.Systems
{
    public class SkinService : ISkinService
    {
        private const string SaveKey = "skin_service_state_v1";

        [Serializable]
        private class SkinSaveData
        {
            public string equippedSkinId = string.Empty;
            public List<string> ownedSkinIds = new();
        }

        private readonly ISaveService saveService;
        private readonly IWalletService walletService;
        private readonly IConfigService configService;
        private readonly SkinCatalog catalog;

        private readonly HashSet<string> ownedIds = new(StringComparer.OrdinalIgnoreCase);
        private string equippedSkinId = string.Empty;

        public string EquippedSkinId => equippedSkinId;
        public IReadOnlyCollection<string> OwnedSkinIds => ownedIds;

        public event Action<string> OnEquippedSkinChanged;
        public event Action OnOwnedSkinsChanged;

        public SkinService(
            ISaveService saveService,
            IWalletService walletService,
            IConfigService configService,
            SkinCatalog catalog)
        {
            this.saveService = saveService;
            this.walletService = walletService;
            this.configService = configService;
            this.catalog = catalog;
        }

        public bool IsSkinOwned(string skinId)
        {
            if (string.IsNullOrWhiteSpace(skinId)) return false;
            return ownedIds.Contains(skinId.Trim());
        }

        public bool IsSkinEquipped(string skinId)
        {
            if (string.IsNullOrWhiteSpace(skinId) || string.IsNullOrWhiteSpace(equippedSkinId))
                return false;

            return string.Equals(equippedSkinId, skinId.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        public int GetSkinCost(string skinId, int fallbackCost = 0)
        {
            int safeFallback = Mathf.Max(0, fallbackCost);

            if (configService?.Economy != null)
            {
                return configService.Economy.GetSkinCost(skinId, safeFallback);
            }

            return safeFallback;
        }

        public bool TryGetEquippedSkin(out SkinDefinition skin)
        {
            if (catalog != null && !string.IsNullOrWhiteSpace(equippedSkinId) && catalog.TryGetSkin(equippedSkinId, out skin))
            {
                return true;
            }

            if (catalog != null && catalog.TryGetDefaultSkin(out skin))
            {
                return true;
            }

            skin = default;
            return false;
        }

        public bool EquipSkin(string skinId)
        {
            if (string.IsNullOrWhiteSpace(skinId)) return false;

            string normalizedId = skinId.Trim();

            if (!IsSkinOwned(normalizedId))
            {
                Debug.LogWarning($"[SkinService] Cannot equip unowned skin '{normalizedId}'.", catalog);
                return false;
            }

            if (catalog != null && catalog.Skins.Count > 0 && !catalog.TryGetSkin(normalizedId, out _))
            {
                Debug.LogWarning($"[SkinService] Skin '{normalizedId}' not found in catalog.", catalog);
                return false;
            }

            if (string.Equals(equippedSkinId, normalizedId, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            equippedSkinId = normalizedId;
            Save();
            OnEquippedSkinChanged?.Invoke(equippedSkinId);
            return true;
        }

        public bool PurchaseAndEquipSkin(string skinId, int fallbackCost = 0)
        {
            if (string.IsNullOrWhiteSpace(skinId)) return false;

            string normalizedId = skinId.Trim();

            if (IsSkinOwned(normalizedId))
            {
                return EquipSkin(normalizedId);
            }

            int cost = GetSkinCost(normalizedId, fallbackCost);

            if (cost > 0)
            {
                if (walletService == null || walletService.Coins < cost)
                {
                    return false;
                }

                walletService.SetCoins(walletService.Coins - cost);
            }

            ownedIds.Add(normalizedId);
            equippedSkinId = normalizedId;

            Save();
            OnOwnedSkinsChanged?.Invoke();
            OnEquippedSkinChanged?.Invoke(equippedSkinId);
            return true;
        }

        public void Load()
        {
            ownedIds.Clear();
            equippedSkinId = string.Empty;

            if (saveService != null)
            {
                string json = saveService.GetString(SaveKey, string.Empty);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    try
                    {
                        var data = JsonUtility.FromJson<SkinSaveData>(json);
                        if (data != null)
                        {
                            if (data.ownedSkinIds != null)
                            {
                                for (int i = 0; i < data.ownedSkinIds.Count; i++)
                                {
                                    string id = data.ownedSkinIds[i];
                                    if (!string.IsNullOrWhiteSpace(id))
                                    {
                                        ownedIds.Add(id.Trim());
                                    }
                                }
                            }

                            if (!string.IsNullOrWhiteSpace(data.equippedSkinId))
                            {
                                equippedSkinId = data.equippedSkinId.Trim();
                            }
                        }
                    }
                    catch
                    {
                        Debug.LogWarning("[SkinService] Failed to parse saved skin data. Using defaults.");
                    }
                }
            }

            EnsureDefaultSkinState();
        }

        public void Save()
        {
            if (saveService == null) return;

            var data = new SkinSaveData
            {
                equippedSkinId = equippedSkinId ?? string.Empty,
                ownedSkinIds = new List<string>(ownedIds)
            };

            string json = JsonUtility.ToJson(data);
            saveService.SetString(SaveKey, json);
        }

        private void EnsureDefaultSkinState()
        {
            string fallbackId = GetFallbackSkinId();

            if (!string.IsNullOrWhiteSpace(fallbackId))
            {
                ownedIds.Add(fallbackId);

                if (string.IsNullOrWhiteSpace(equippedSkinId) || !IsSkinOwned(equippedSkinId))
                {
                    equippedSkinId = fallbackId;
                }
            }
        }

        private string GetFallbackSkinId()
        {
            if (catalog != null && !string.IsNullOrWhiteSpace(catalog.DefaultSkinId))
            {
                return catalog.DefaultSkinId.Trim();
            }

            if (catalog != null && catalog.Skins != null && catalog.Skins.Count > 0 && !string.IsNullOrWhiteSpace(catalog.Skins[0].id))
            {
                return catalog.Skins[0].id.Trim();
            }

            return "default";
        }
    }
}