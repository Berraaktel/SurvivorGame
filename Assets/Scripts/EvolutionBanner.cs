using UnityEngine;
using UnityEngine.UI;

// Shows a short "X -> Y!" banner when a weapon evolves (see each weapon's
// CheckEvolution()/Evolve()). Uses unscaled time so it still finishes its
// fade even if a level-up choice happens to freeze Time.timeScale at the
// same moment - same reasoning as FxSpawner's particles using unscaled
// time (see FxSpawner.Burst).
public class EvolutionBanner : MonoBehaviour
{
    public static EvolutionBanner Instance;

    public GameObject panel;
    public Text label;
    public float holdDuration = 1.6f;
    public float fadeDuration = 0.5f;

    private float timer;
    private bool active;
    private CanvasGroup group;

    void Awake()
    {
        Instance = this;
        if (panel != null)
        {
            group = panel.GetComponent<CanvasGroup>();
            if (group == null) group = panel.AddComponent<CanvasGroup>();
            panel.SetActive(false);
        }
    }

    public void Show(string message)
    {
        if (panel == null || label == null) return;

        label.text = message;
        panel.SetActive(true);
        if (group != null) group.alpha = 1f;
        timer = 0f;
        active = true;
    }

    void Update()
    {
        if (!active) return;

        timer += Time.unscaledDeltaTime;

        if (timer >= holdDuration + fadeDuration)
        {
            active = false;
            panel.SetActive(false);
            return;
        }

        if (timer >= holdDuration && group != null)
        {
            float t = (timer - holdDuration) / fadeDuration;
            group.alpha = Mathf.Clamp01(1f - t);
        }
    }
}
