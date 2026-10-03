using UnityEngine;
using UnityEngine.UI;

// Permanent-upgrade shop, opened from the Main Menu. Spends lifetime Gold
// (GoldManager) on permanent run-start bonuses (PermanentUpgrades) - those
// bonuses are then applied at the start of every run by
// ApplyPermanentUpgrades. No real-money IAP yet: this is just the economy
// and the spending UI around it.
public class ShopUI : MonoBehaviour
{
    public GameObject panel;
    public Text headerText;
    public Text goldBalanceText;

    // Index 0 = health, 1 = knife tier, 2 = skin - same order everywhere.
    public Text[] optionTitles;
    public Text[] optionDescriptions;
    public Text[] optionButtonLabels;
    public Button[] optionButtons;
    public Image[] optionIcons;
    public Button backButton;

    void Awake()
    {
        if (headerText != null) headerText.text = Localization.Get("shop_title");

        if (optionButtons != null)
        {
            if (optionButtons.Length > 0 && optionButtons[0] != null) optionButtons[0].onClick.AddListener(BuyHealth);
            if (optionButtons.Length > 1 && optionButtons[1] != null) optionButtons[1].onClick.AddListener(BuyKnifeTier);
            if (optionButtons.Length > 2 && optionButtons[2] != null) optionButtons[2].onClick.AddListener(BuySkin);
        }

        if (backButton != null) backButton.onClick.AddListener(Close);
    }

    public void Open()
    {
        if (panel != null) panel.SetActive(true);
        Refresh();
    }

    public void Close()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClick();
        if (panel != null) panel.SetActive(false);

        MainMenuUI menu = Object.FindFirstObjectByType<MainMenuUI>();
        if (menu != null && menu.panel != null) menu.panel.SetActive(true);
    }

    void Refresh()
    {
        if (goldBalanceText != null)
        {
            goldBalanceText.text = Localization.Get("gold_balance_label") + ": " + GoldManager.GetBalance();
        }

        SetRowText(0, Localization.Get("shop_health_title"), Localization.Get("shop_health_desc"));
        SetLevelStatus(0, PermanentUpgrades.GetHealthLevel(), PermanentUpgrades.MaxHealthLevel, PermanentUpgrades.HealthLevelCost);

        bool knifeMaxed = KnifeTiers.IsMaxTier();
        string knifeDesc = knifeMaxed
            ? Localization.Get("shop_knife_maxed_desc")
            : string.Format(Localization.Get("shop_knife_desc_format"), KnifeTiers.GetTierName(KnifeTiers.GetTier() + 1), KnifeTiers.GetNextDamageBonus());
        SetRowText(1, KnifeTiers.GetCurrentTierName(), knifeDesc);
        SetButtonState(1, knifeMaxed, knifeMaxed ? Localization.Get("maxed_label") : Localization.Get("buy_button") + "\n(" + KnifeTiers.GetNextTierPrice() + ")");
        if (optionIcons != null && optionIcons.Length > 1 && optionIcons[1] != null) optionIcons[1].color = KnifeTiers.GetTint();

        SetRowText(2, Localization.Get("shop_skin_title"), Localization.Get("shop_skin_desc"));
        SetOwnedStatus(2, PermanentUpgrades.IsSkinUnlocked(), PermanentUpgrades.SkinCost);
    }

    void SetRowText(int index, string title, string desc)
    {
        if (optionTitles != null && index < optionTitles.Length && optionTitles[index] != null) optionTitles[index].text = title;
        if (optionDescriptions != null && index < optionDescriptions.Length && optionDescriptions[index] != null) optionDescriptions[index].text = desc;
    }

    void SetLevelStatus(int index, int level, int maxLevel, int cost)
    {
        bool maxed = level >= maxLevel;
        SetButtonState(index, maxed, maxed ? Localization.Get("maxed_label") : Localization.Get("buy_button") + "\n(" + cost + ")");
    }

    void SetOwnedStatus(int index, bool owned, int cost)
    {
        SetButtonState(index, owned, owned ? Localization.Get("owned_label") : Localization.Get("buy_button") + "\n(" + cost + ")");
    }

    void SetButtonState(int index, bool disabled, string label)
    {
        if (optionButtonLabels != null && index < optionButtonLabels.Length && optionButtonLabels[index] != null)
        {
            optionButtonLabels[index].text = label;
        }
        if (optionButtons != null && index < optionButtons.Length && optionButtons[index] != null)
        {
            optionButtons[index].interactable = !disabled;
        }
    }

    void BuyHealth()
    {
        if (PermanentUpgrades.TryBuyHealthLevel() && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayClick();
        }
        Refresh();
    }

    void BuyKnifeTier()
    {
        if (KnifeTiers.TryBuyNextTier() && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayClick();
        }
        Refresh();
    }

    void BuySkin()
    {
        if (PermanentUpgrades.TryBuySkin() && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayClick();
        }
        Refresh();
    }
}
