using System.Collections.Generic;
using TMPro;
using UnityEngine;
using ZENN.Core;

namespace ZENN.Systems
{
    public class SkinShopView : MonoBehaviour
    {
        [SerializeField] private SkinCatalog catalog;
        [SerializeField] private RectTransform contentContainer;
        [SerializeField] private SkinShopCard cardPrefab;
        [SerializeField] private TMP_Text coinsLabel;

        private readonly List<SkinShopCard> spawnedCards = new();

        private void OnEnable()
        {
            BuildCards();
            RefreshStates();

            if (Services.Skins != null)
            {
                Services.Skins.OnOwnedSkinsChanged += RefreshStates;
                Services.Skins.OnEquippedSkinChanged += HandleEquippedSkinChanged;
            }
        }

        private void OnDisable()
        {
            if (Services.Skins != null)
            {
                Services.Skins.OnOwnedSkinsChanged -= RefreshStates;
                Services.Skins.OnEquippedSkinChanged -= HandleEquippedSkinChanged;
            }
        }

        public void BuildCards()
        {
            if (contentContainer == null || cardPrefab == null || catalog == null) return;

            for (int i = contentContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(contentContainer.GetChild(i).gameObject);
            }
            spawnedCards.Clear();

            for (int i = 0; i < catalog.Skins.Count; i++)
            {
                SkinDefinition skin = catalog.Skins[i];
                SkinShopCard card = Instantiate(cardPrefab, contentContainer);
                card.name = $"SkinCard_{skin.id}";
                card.Bind(skin, HandleCardClicked);
                spawnedCards.Add(card);
            }
        }

        public void RefreshStates()
        {
            if (coinsLabel != null && Services.Wallet != null)
            {
                coinsLabel.text = Services.Wallet.Coins.ToString();
            }

            for (int i = 0; i < spawnedCards.Count; i++)
            {
                spawnedCards[i].Refresh(Services.Skins);
            }
        }

        private void HandleEquippedSkinChanged(string _)
        {
            RefreshStates();
        }

        private void HandleCardClicked(SkinDefinition skin)
        {
            if (Services.Skins == null) return;

            bool changed = Services.Skins.PurchaseAndEquipSkin(skin.id, skin.fallbackCost);
            if (!changed)
            {
                Debug.Log($"[SkinShop] Not enough coins or unable to equip '{skin.id}'.");
            }
            else
            {
                RefreshStates();
            }
        }
    }
}