using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZENN.Core;

namespace ZENN.Systems
{
    public class SkinShopCard : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text statusText;

        private SkinDefinition boundSkin;
        private Action<SkinDefinition> onClicked;

        public void Bind(SkinDefinition skin, Action<SkinDefinition> clickCallback)
        {
            boundSkin = skin;
            onClicked = clickCallback;

            if (nameText != null)
            {
                nameText.text = string.IsNullOrWhiteSpace(skin.displayName) ? skin.id : skin.displayName;
            }

            if (iconImage != null)
            {
                iconImage.sprite = skin.icon;
                iconImage.enabled = skin.icon != null;
            }

            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(HandleClick);
            }
        }

        public void Refresh(ISkinService skinService)
        {
            if (skinService == null || statusText == null) return;

            bool equipped = skinService.IsSkinEquipped(boundSkin.id);
            bool owned = skinService.IsSkinOwned(boundSkin.id);

            if (equipped)
            {
                statusText.text = "Equipped";
                if (button != null) button.interactable = false;
            }
            else if (owned)
            {
                statusText.text = "Equip";
                if (button != null) button.interactable = true;
            }
            else
            {
                int cost = skinService.GetSkinCost(boundSkin.id, boundSkin.fallbackCost);
                statusText.text = cost > 0 ? $"{cost} Coins" : "Free";
                if (button != null) button.interactable = true;
            }
        }

        private void HandleClick()
        {
            onClicked?.Invoke(boundSkin);
        }
    }
}