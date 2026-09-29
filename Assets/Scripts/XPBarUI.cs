using UnityEngine;
using UnityEngine.UI;

public class XPBarUI : MonoBehaviour
{
    public Image fillImage;
    public Text levelText;
    public Text xpText;
    public float fillSpeed = 2f;

    private PlayerXP playerXP;
    private float targetFill;

    void OnEnable()
    {
        TryBind();
    }

    void Update()
    {
        if (playerXP == null)
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
        PlayerXP xp = Object.FindFirstObjectByType<PlayerXP>();
        if (xp == null || xp == playerXP) return;

        playerXP = xp;
        playerXP.OnXPChanged += HandleXPChanged;
        playerXP.OnLevelUp += HandleLevelUp;
        HandleXPChanged(playerXP.currentXP, playerXP.xpToNextLevel);
        HandleLevelUp(playerXP.level);
        if (fillImage != null) fillImage.fillAmount = targetFill;
    }

    void HandleXPChanged(int current, int toNext)
    {
        targetFill = toNext > 0 ? (float)current / toNext : 0f;
        if (xpText != null) xpText.text = current + " / " + toNext;
    }

    void HandleLevelUp(int level)
    {
        if (levelText != null) levelText.text = "Lv. " + level;
    }

    void OnDisable()
    {
        if (playerXP != null)
        {
            playerXP.OnXPChanged -= HandleXPChanged;
            playerXP.OnLevelUp -= HandleLevelUp;
        }
    }
}
