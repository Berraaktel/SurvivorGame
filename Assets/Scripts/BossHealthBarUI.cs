using UnityEngine;
using UnityEngine.UI;

// Shows a health bar across the top of the screen while the Boss is alive,
// and hides it otherwise. Lives on the same always-active Canvas as
// HealthBarUI/XPBarUI (so Update keeps running even while the bar itself
// is hidden) and tracks EnemyHealth.ActiveBoss rather than searching the
// scene every frame - there is at most one boss alive at a time.
public class BossHealthBarUI : MonoBehaviour
{
    public GameObject panelRoot;
    public Image fillImage;
    public float fillSpeed = 3f;

    private EnemyHealth boundBoss;
    private float targetFill = 1f;

    void Update()
    {
        EnemyHealth current = EnemyHealth.ActiveBoss;

        if (current != boundBoss)
        {
            if (boundBoss != null)
            {
                boundBoss.OnHealthChanged -= HandleHealthChanged;
            }

            boundBoss = current;

            if (boundBoss != null)
            {
                boundBoss.OnHealthChanged += HandleHealthChanged;
                HandleHealthChanged(boundBoss.CurrentHealth, boundBoss.maxHealth);
                if (fillImage != null) fillImage.fillAmount = targetFill;
            }

            if (panelRoot != null) panelRoot.SetActive(boundBoss != null);
        }

        if (fillImage != null)
        {
            fillImage.fillAmount = Mathf.MoveTowards(fillImage.fillAmount, targetFill, fillSpeed * Time.deltaTime);
        }
    }

    void HandleHealthChanged(int current, int max)
    {
        targetFill = max > 0 ? (float)current / max : 0f;
    }

    void OnDisable()
    {
        if (boundBoss != null)
        {
            boundBoss.OnHealthChanged -= HandleHealthChanged;
            boundBoss = null;
        }
    }
}
