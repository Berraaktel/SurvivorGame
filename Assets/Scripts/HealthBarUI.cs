using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Image fillImage;
    public float fillSpeed = 3f;

    private PlayerHealth playerHealth;
    private float targetFill = 1f;

    void OnEnable()
    {
        TryBind();
    }

    void Update()
    {
        if (playerHealth == null)
        {
            TryBind();
        }

        if (fillImage != null)
        {
            fillImage.fillAmount = Mathf.MoveTowards(fillImage.fillAmount, targetFill, fillSpeed * Time.deltaTime);
        }
    }

    void TryBind()
    {
        PlayerHealth ph = Object.FindFirstObjectByType<PlayerHealth>();
        if (ph == null || ph == playerHealth) return;

        playerHealth = ph;
        playerHealth.OnHealthChanged += HandleHealthChanged;
        HandleHealthChanged(playerHealth.CurrentHealth, playerHealth.maxHealth);
        if (fillImage != null) fillImage.fillAmount = targetFill;
    }

    void HandleHealthChanged(int current, int max)
    {
        targetFill = max > 0 ? (float)current / max : 0f;
    }

    void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= HandleHealthChanged;
        }
    }
}
