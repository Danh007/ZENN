using System;

namespace ZENN.Core
{
    public interface IWalletService
    {
        int Coins { get; }
        event Action<int> OnCoinsChanged; // Placeholder / clean hook for UI & other systems
        void SetCoins(int amount);
        void AddCoins(int amount);
    }
}