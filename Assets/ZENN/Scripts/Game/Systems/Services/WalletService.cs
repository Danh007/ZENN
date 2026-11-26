using System;
using ZENN.Core;

namespace ZENN.Systems
{
    public class WalletService : IWalletService
    {
        private const string WalletCoinsKey = "wallet_coins";
        private const string WalletInitializedKey = "wallet_initialized";

        private readonly ISaveService saveService;
        private readonly IConfigService configService;

        public int Coins { get; private set; }
        public event Action<int> OnCoinsChanged;

        public WalletService(ISaveService saveService, IConfigService configService)
        {
            this.saveService = saveService;
            this.configService = configService;
        }

        public void Load()
        {
            if (saveService == null)
            {
                Coins = 0;
                OnCoinsChanged?.Invoke(Coins);
                return;
            }

            bool isInitialized = saveService.GetString(WalletInitializedKey, "") == "1";

            if (!isInitialized)
            {
                int starting = configService?.Economy != null
                    ? Math.Max(0, configService.Economy.startingCoins)
                    : 0;

                Coins = starting;
                saveService.SetString(WalletCoinsKey, Coins.ToString());
                saveService.SetString(WalletInitializedKey, "1");
            }
            else
            {
                string raw = saveService.GetString(WalletCoinsKey, "0");
                Coins = int.TryParse(raw, out int parsedCoins) && parsedCoins >= 0
                    ? parsedCoins
                    : 0;
            }

            OnCoinsChanged?.Invoke(Coins);
        }

        public void AddCoins(int amount)
        {
            if (amount <= 0) return;

            Coins += amount;
            Save();
            OnCoinsChanged?.Invoke(Coins);
        }

        public void SetCoins(int amount)
        {
            Coins = Math.Max(0, amount);
            Save();
            OnCoinsChanged?.Invoke(Coins);
        }

        private void Save()
        {
            saveService?.SetString(WalletCoinsKey, Coins.ToString());
            saveService?.SetString(WalletInitializedKey, "1");
        }
    }
}