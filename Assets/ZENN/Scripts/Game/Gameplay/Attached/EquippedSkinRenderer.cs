using UnityEngine;
using ZENN.Core;
using ZENN.Systems;

namespace ZENN.Gameplay
{
    [RequireComponent(typeof(MeshRenderer))]
    public class EquippedSkinRenderer : MonoBehaviour
    {
        [Tooltip("Optional fallback catalog if Services.Skins isn't available yet (e.g., direct scene testing).")]
        [SerializeField] private SkinCatalog fallbackCatalog;

        [Tooltip("Material slot index on the MeshRenderer (Element 0 by default).")]
        [SerializeField] private int materialIndex = 0;

        private MeshRenderer targetRenderer;

        private void Awake()
        {
            targetRenderer = GetComponent<MeshRenderer>();
        }

        private void OnEnable()
        {
            if (Services.Skins != null)
            {
                Services.Skins.OnEquippedSkinChanged += HandleEquippedSkinChanged;
            }

            RefreshSkin();
        }

        private void OnDisable()
        {
            if (Services.Skins != null)
            {
                Services.Skins.OnEquippedSkinChanged -= HandleEquippedSkinChanged;
            }
        }

        private void Start()
        {
            // Ensures sync if Services.Skins was initialized after Awake/OnEnable
            RefreshSkin();
        }

        private void HandleEquippedSkinChanged(string _)
        {
            RefreshSkin();
        }

        public void RefreshSkin()
        {
            if (targetRenderer == null)
            {
                targetRenderer = GetComponent<MeshRenderer>();
            }

            if (targetRenderer == null)
                return;

            Material skinMaterial = ResolveEquippedMaterial();
            if (skinMaterial == null)
                return;

            Material[] materials = targetRenderer.sharedMaterials;
            if (materialIndex < 0 || materialIndex >= materials.Length)
                return;

            if (materials[materialIndex] == skinMaterial)
                return;

            materials[materialIndex] = skinMaterial;
            targetRenderer.sharedMaterials = materials;
        }

       private Material ResolveEquippedMaterial()
        {
            if (Services.Skins != null && Services.Skins.TryGetEquippedSkin(out SkinDefinition equippedSkin) && equippedSkin.material != null)
            {
                return equippedSkin.material;
            }

            if (fallbackCatalog != null)
            {
                string targetId = Services.Skins != null && !string.IsNullOrWhiteSpace(Services.Skins.EquippedSkinId)
                    ? Services.Skins.EquippedSkinId
                    : fallbackCatalog.DefaultSkinId;

                if (fallbackCatalog.TryGetSkin(targetId, out SkinDefinition fallbackMatch) && fallbackMatch.material != null)
                {
                    return fallbackMatch.material;
                }

                if (fallbackCatalog.TryGetDefaultSkin(out SkinDefinition fallbackDefault) && fallbackDefault.material != null)
                {
                    return fallbackDefault.material;
                }
            }

            return null;
        }
    }
}