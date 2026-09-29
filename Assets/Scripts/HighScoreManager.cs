using UnityEngine;

// Tracks the player's best survival time across play sessions/builds via
// PlayerPrefs. Static helper (no scene object needed) so any UI script
// can just call HighScoreManager.GetBest() / ReportRun(...) directly.
public static class HighScoreManager
{
    private const string BestTimeKey = "best_survival_time";

    public static float GetBest()
    {
        return PlayerPrefs.GetFloat(BestTimeKey, 0f);
    }

    // Call once when a run ends. Returns true (and saves) if this run beat
    // the previous best, false otherwise.
    public static bool ReportRun(float survivedSeconds)
    {
        float best = GetBest();
        if (survivedSeconds > best)
        {
            PlayerPrefs.SetFloat(BestTimeKey, survivedSeconds);
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }

    public static string Format(float seconds)
    {
        int totalSeconds = Mathf.FloorToInt(seconds);
        int minutes = totalSeconds / 60;
        int secs = totalSeconds % 60;
        return string.Format("{0:00}:{1:00}", minutes, secs);
    }
}
