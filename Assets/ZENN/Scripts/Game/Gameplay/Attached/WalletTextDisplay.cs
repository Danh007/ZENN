using TMPro;
using UnityEngine;
using ZENN.Systems;

[RequireComponent(typeof(TextMeshProUGUI))]
public class WalletTextDisplay : MonoBehaviour
{
    [SerializeField] private string prefix = "";
    private TextMeshProUGUI label;

    void Awake()
    {
        label = GetComponent<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        Refresh(Services.Wallet?.Coins ?? 0);

        if (Services.Wallet != null)
            Services.Wallet.OnCoinsChanged += Refresh;
    }

    void OnDisable()
    {
        if (Services.Wallet != null)
            Services.Wallet.OnCoinsChanged -= Refresh;
    }

    private void Refresh(int currentCoins)
    {
        label.text = $"{prefix}{currentCoins}";
    }
}