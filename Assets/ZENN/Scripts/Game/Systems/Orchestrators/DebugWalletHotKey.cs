using UnityEngine;
using UnityEngine.InputSystem;
using ZENN.Systems;

#if UNITY_EDITOR
public class DebugWalletHotkey : MonoBehaviour
{
    [SerializeField] private Key resetKey = Key.F9;
    [SerializeField] private Key addTestCoinsKey = Key.F10;
    [SerializeField] private int testCoinAmount = 10;

    void Update()
    {
        if (Services.Wallet == null) return;

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard[resetKey].wasPressedThisFrame)
        {
            Services.Wallet.SetCoins(0);
            Debug.Log("[Debug] Wallet reset to 0.");
        }

        if (keyboard[addTestCoinsKey].wasPressedThisFrame)
        {
            Services.Wallet.AddCoins(testCoinAmount);
            Debug.Log($"[Debug] Added {testCoinAmount} coins. Total: {Services.Wallet.Coins}");
        }
    }
}
#endif