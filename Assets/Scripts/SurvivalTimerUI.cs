using UnityEngine;
using UnityEngine.UI;

// Shows elapsed survival time as MM:SS. Uses Time.deltaTime, so it
// automatically pauses whenever Time.timeScale is 0 (Game Over screen,
// level-up choice screen) exactly like the rest of the game's pause logic.
public class SurvivalTimerUI : MonoBehaviour
{
    public Text timerText;
    private float elapsed;

    void Update()
    {
        elapsed += Time.deltaTime;

        if (timerText == null) return;

        int totalSeconds = Mathf.FloorToInt(elapsed);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
