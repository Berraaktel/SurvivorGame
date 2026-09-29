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
    public Text quitButtonText;

    void Awake()
    {
        if (titleText != null) titleText.text = Localization.Get("main_menu_title");
        if (playButtonText != null) playButtonText.text = Localization.Get("play_button");
        if (quitButtonText != null) quitButtonText.text = Localization.Get("quit_button");

        Button[] buttons = panel != null ? panel.GetComponentsInChildren<Button>(true) : new Button[0];
        foreach (Button b in buttons)
        {
            if (b.name == "PlayButton") b.onClick.AddListener(Play);
            else if (b.name == "QuitButton") b.onClick.AddListener(Quit);
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

    public void Quit()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClick();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
