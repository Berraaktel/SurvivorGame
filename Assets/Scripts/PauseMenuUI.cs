using UnityEngine;
using UnityEngine.UI;

// ESC toggles a pause overlay with Resume / Settings / Quit. Settings is a
// second sub-panel (volume sliders) reached from the pause panel, not a
// separate scene - same "just toggle GameObjects + freeze timeScale" trick
// the other UI overlays (MainMenuUI, GameOverUI, UpgradeUI) already use.
public class PauseMenuUI : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject settingsPanel;

    public Text titleText;
    public Text resumeButtonText;
    public Text settingsButtonText;
    public Text quitButtonText;

    public Text settingsTitleText;
    public Text sfxLabel;
    public Text musicLabel;
    public Text backButtonText;
    public Slider sfxSlider;
    public Slider musicSlider;

    private bool isPaused;

    void Awake()
    {
        if (titleText != null) titleText.text = Localization.Get("paused_title");
        if (resumeButtonText != null) resumeButtonText.text = Localization.Get("resume_button");
        if (settingsButtonText != null) settingsButtonText.text = Localization.Get("settings_button");
        if (quitButtonText != null) quitButtonText.text = Localization.Get("quit_button");
        if (settingsTitleText != null) settingsTitleText.text = Localization.Get("settings_title");
        if (sfxLabel != null) sfxLabel.text = Localization.Get("sfx_volume");
        if (musicLabel != null) musicLabel.text = Localization.Get("music_volume");
        if (backButtonText != null) backButtonText.text = Localization.Get("back_button");

        Button[] buttons = pausePanel != null ? pausePanel.GetComponentsInChildren<Button>(true) : new Button[0];
        foreach (Button b in buttons)
        {
            if (b.name == "ResumeButton") b.onClick.AddListener(Resume);
            else if (b.name == "SettingsButton") b.onClick.AddListener(OpenSettings);
            else if (b.name == "QuitButton") b.onClick.AddListener(Quit);
        }

        Button[] settingsButtons = settingsPanel != null ? settingsPanel.GetComponentsInChildren<Button>(true) : new Button[0];
        foreach (Button b in settingsButtons)
        {
            if (b.name == "BackButton") b.onClick.AddListener(CloseSettings);
        }

        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);
        }
        if (musicSlider != null)
        {
            musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        }

        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    void OnSfxSliderChanged(float v)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.SetSfxVolume(v);
    }

    void OnMusicSliderChanged(float v)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.SetMusicVolume(v);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                // If settings sub-panel is open, ESC backs out of it first
                // rather than closing the whole pause menu in one press.
                if (settingsPanel != null && settingsPanel.activeSelf)
                {
                    CloseSettings();
                }
                else
                {
                    Resume();
                }
            }
            else if (CanPause())
            {
                Pause();
            }
        }
    }

    bool CanPause()
    {
        // Never open the pause menu on top of another full-screen overlay
        // (main menu / level-up choice / game over) - those already own
        // Time.timeScale and Resume() here would stomp on them.
        return Time.timeScale != 0f;
    }

    void Pause()
    {
        isPaused = true;
        if (pausePanel != null) pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        isPaused = false;
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OpenSettings()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            if (sfxSlider != null && AudioManager.Instance != null) sfxSlider.SetValueWithoutNotify(AudioManager.Instance.sfxVolume);
            if (musicSlider != null && AudioManager.Instance != null) musicSlider.SetValueWithoutNotify(AudioManager.Instance.musicVolume);
        }
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    public void Quit()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
