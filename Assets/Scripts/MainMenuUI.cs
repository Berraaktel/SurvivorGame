using UnityEngine;
using UnityEngine.UI;

// Simple main-menu overlay: shows on top of the gameplay scene the moment
// it loads, freezes time so nothing (enemy spawns, timers, movement) can
// tick while it's up, and hands control back to the game on Play. No
// separate scene/Build Settings entry needed - same trick GameOverUI
// already uses for its overlay.
public class MainMenuUI : MonoBehaviour
{
    public GameObject panel;
    public Text titleText;
    public Text playButtonText;
    public Text shopButtonText;
    public Text bestTimeText;

    void Awake()
    {
        if (titleText != null) titleText.text = Localization.Get("main_menu_title");
        if (playButtonText != null) playButtonText.text = Localization.Get("play_button");
        if (shopButtonText != null) shopButtonText.text = Localization.Get("shop_button");

        if (bestTimeText != null)
        {
            float best = HighScoreManager.GetBest();
            bestTimeText.text = best > 0f ? Localization.Get("score_label") + ": " + HighScoreManager.Format(best) : "";
        }

        Button[] buttons = panel != null ? panel.GetComponentsInChildren<Button>(true) : new Button[0];
        foreach (Button b in buttons)
        {
            if (b.name == "PlayButton") b.onClick.AddListener(Play);
            else if (b.name == "ShopButton") b.onClick.AddListener(OpenShop);
        }

        if (panel != null) panel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Play()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClick();
        Time.timeScale = 1f;
        if (panel != null) panel.SetActive(false);
    }

    public void OpenShop()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClick();

        ShopUI shop = Object.FindFirstObjectByType<ShopUI>();
        if (shop != null) shop.Open();

        if (panel != null) panel.SetActive(false);
    }
}
