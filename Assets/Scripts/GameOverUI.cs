using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public GameObject panel;
    public Text gameOverText;
    public Text restartButtonText;
    public Text bestTimeText;
    public Text goldEarnedText;
    private PlayerHealth boundHealth;

    void Awake()
    {
        if (gameOverText != null) gameOverText.text = Localization.Get("game_over");
        if (restartButtonText != null) restartButtonText.text = Localization.Get("restart");

        Button restartButton = panel != null ? panel.GetComponentInChildren<Button>(true) : GetComponentInChildren<Button>(true);
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(Restart);
        }
    }

    void OnEnable()
    {
        TryBind();
    }

    void Update()
    {
        if (boundHealth == null)
        {
            TryBind();
        }
    }

    void TryBind()
    {
        PlayerHealth ph = Object.FindFirstObjectByType<PlayerHealth>();
        if (ph == null || ph == boundHealth) return;

        boundHealth = ph;
        boundHealth.OnDied += HandleDied;
    }

    void HandleDied()
    {
        if (panel != null) panel.SetActive(true);
        Time.timeScale = 0f;
        if (AudioManager.Instance != null) AudioManager.Instance.PlayGameOver();

        if (bestTimeText != null)
        {
            SurvivalTimerUI timer = Object.FindFirstObjectByType<SurvivalTimerUI>();
            float survived = timer != null ? timer.Elapsed : 0f;
            bool isRecord = HighScoreManager.ReportRun(survived);
            string prefix = isRecord ? Localization.Get("record_score_label") : Localization.Get("score_label");
            bestTimeText.text = prefix + ": " + HighScoreManager.Format(HighScoreManager.GetBest());
        }

        if (goldEarnedText != null)
        {
            // Gold only becomes permanent here, on death - this is the one
            // and only place PlayerGold's run-local total gets banked into
            // GoldManager's persistent lifetime balance.
            PlayerGold gold = Object.FindFirstObjectByType<PlayerGold>();
            int earned = gold != null ? gold.runGold : 0;
            GoldManager.AddGold(earned);
            goldEarnedText.text = Localization.Get("gold_earned_label") + ": +" + earned
                + "  (" + Localization.Get("total_gold_label") + ": " + GoldManager.GetBalance() + ")";
        }
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnDisable()
    {
        if (boundHealth != null)
        {
            boundHealth.OnDied -= HandleDied;
        }
    }
}
