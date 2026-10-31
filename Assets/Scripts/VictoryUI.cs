using UnityEngine;
using UnityEngine.UI;

// Marks a run's "win" moment. Vampire-Survivors-style games are built
// around surviving a target duration, not lasting forever until death -
// without this the game had no win state at all, only Game Over. Shows
// once, when SurvivalTimerUI crosses targetSeconds (the same 10-minute
// mark DifficultyManager's own scaling already caps out at, so hitting
// it means the player weathered the run's hardest sustained difficulty).
public class VictoryUI : MonoBehaviour
{
    public float targetSeconds = 600f; // 10 minutes
    public GameObject panel;
    public Text victoryText;
    public Text timeText;
    public Text goldEarnedText;
    public Text continueButtonText;

    private bool triggered;
    private SurvivalTimerUI timer;

    void Awake()
    {
        if (victoryText != null) victoryText.text = Localization.Get("victory_title");
        if (continueButtonText != null) continueButtonText.text = Localization.Get("resume_button");

        Button continueButton = panel != null ? panel.GetComponentInChildren<Button>(true) : GetComponentInChildren<Button>(true);
        if (continueButton != null) continueButton.onClick.AddListener(Continue);

        if (panel != null) panel.SetActive(false);
    }

    void Update()
    {
        if (triggered) return;

        if (timer == null)
        {
            timer = Object.FindFirstObjectByType<SurvivalTimerUI>();
            if (timer == null) return;
        }

        if (timer.Elapsed >= targetSeconds)
        {
            triggered = true;
            ShowVictory();
        }
    }

    void ShowVictory()
    {
        if (panel != null) panel.SetActive(true);
        Time.timeScale = 0f;
        if (AudioManager.Instance != null) AudioManager.Instance.PlayLevelUp();

        // Bank the run's gold so far and zero PlayerGold's own counter -
        // otherwise GameOverUI would bank this same gold a second time
        // when the player eventually dies later in the same run. Only
        // gold earned AFTER this point gets banked again at that death.
        PlayerGold gold = Object.FindFirstObjectByType<PlayerGold>();
        int earned = gold != null ? gold.runGold : 0;
        GoldManager.AddGold(earned);
        if (gold != null) gold.runGold = 0;

        HighScoreManager.ReportRun(timer.Elapsed);

        if (timeText != null) timeText.text = HighScoreManager.Format(timer.Elapsed);
        if (goldEarnedText != null)
        {
            goldEarnedText.text = Localization.Get("gold_earned_label") + ": +" + earned
                + "  (" + Localization.Get("total_gold_label") + ": " + GoldManager.GetBalance() + ")";
        }
    }

    // Closes the panel and lets the run keep going past the win point -
    // DifficultyManager is already capped, so nothing special needs to
    // happen here beyond unpausing.
    public void Continue()
    {
        if (panel != null) panel.SetActive(false);
        Time.timeScale = 1f;
    }
}
