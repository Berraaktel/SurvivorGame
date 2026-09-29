using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public GameObject panel;
    public Text gameOverText;
    public Text restartButtonText;
    public Text bestTimeText;
    private PlayerHealth boundHealth;

    void Awake()
    {
        if (gameOverText != null) gameOverText.text = Localization.Get("game_over");
        if (restartButtonText != null) restartButtonText.text = Localization.Get("restart");

        Button restartButton = GetComponentInChildren<Button>(true);
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
