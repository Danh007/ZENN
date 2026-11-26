using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZENN.Core
{
    [Serializable]
    public struct SkinDefinition
    {
        [Tooltip("Stable unique ID used for saving & equipping (e.g., 'default', 'blue_shader').")]
        public string id;

        [Tooltip("Shown in the Shop UI.")]
        public string displayName;

        [Tooltip("Optional preview icon for horizontal scroll cards.")]
        public Sprite icon;

        [Tooltip("Material applied to the sphere's MeshRenderer (Element 0).")]
        public Material material;

        [Tooltip("Optional fallback cost if not driven by EconomyConfig.")]
        [Min(0)]
        public int fallbackCost;

        public bool IsValid => !string.IsNullOrWhiteSpace(id) && material != null;
    }

    [CreateAssetMenu(menuName = "Game/Skin Catalog", fileName = "SkinCatalog")]
    public class SkinCatalog : ScriptableObject
    {
        [SerializeField] private string defaultSkinId = "default";
        [SerializeField] private List<SkinDefinition> skins = new();

        public string DefaultSkinId => defaultSkinId;
        public IReadOnlyList<SkinDefinition> Skins => skins;

        public bool TryGetSkin(string skinId, out SkinDefinition skin)
        {
            if (!string.IsNullOrWhiteSpace(skinId))
            {
                for (int i = 0; i < skins.Count; i++)
                {
                    if (skins[i].id == skinId)
                    {
                        skin = skins[i];
                        return true;
                    }
                }
            }

            skin = default;
            return false;
        }

        public bool TryGetDefaultSkin(out SkinDefinition skin)
        {
            return TryGetSkin(defaultSkinId, out skin);
        }
    }
}