using System;
using System.Collections.Generic;

namespace ZENN.Core
{
    public interface ISkinService
    {
        string EquippedSkinId { get; }
        IReadOnlyCollection<string> OwnedSkinIds { get; }

        event Action<string> OnEquippedSkinChanged;
        event Action OnOwnedSkinsChanged;

        bool IsSkinOwned(string skinId);
        bool IsSkinEquipped(string skinId);
        int GetSkinCost(string skinId, int fallbackCost = 0);
        bool TryGetEquippedSkin(out SkinDefinition skin);

        bool EquipSkin(string skinId);
        bool PurchaseAndEquipSkin(string skinId, int fallbackCost = 0);
        void Load();
        void Save();
    }
}