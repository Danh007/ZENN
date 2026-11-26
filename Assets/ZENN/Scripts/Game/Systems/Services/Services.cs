using ZENN.Core;

namespace ZENN.Systems
{
    public static class Services
    {
        public static ISaveService Save { get; set; }
        public static ISettingsService Settings { get; set; }
        public static IWalletService Wallet { get; set; }
        public static IConfigService Config { get; set; }
    }
}
