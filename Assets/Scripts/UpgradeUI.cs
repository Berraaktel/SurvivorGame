using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UpgradeUI : MonoBehaviour
{
    public GameObject panel;
    public Text headerText;
    public Text[] optionTitles;
    public Text[] optionDescriptions;
    public Button[] optionButtons;

    private PlayerXP boundXP;
    private PlayerUpgrades playerUpgrades;
    private UpgradeDef[] currentChoices;

    private struct UpgradeDef
    {
        public string title;
        public string description;
        public System.Action<PlayerUpgrades> apply;
    }

    private static UpgradeDef[] BuildUpgradeList()
    {
        return new UpgradeDef[]
        {
            new UpgradeDef { title = Localization.Get("upgrade_speed_title"), description = Localization.Get("upgrade_speed_desc"), apply = (u) => u.ApplyMoveSpeed(0.5f) },
            new UpgradeDef { title = Localization.Get("upgrade_health_title"), description = Localization.Get("upgrade_health_desc"), apply = (u) => u.ApplyMaxHealth(2) },
            new UpgradeDef { title = Localization.Get("upgrade_damage_title"), description = Localization.Get("upgrade_damage_desc"), apply = (u) => u.ApplyDamage(1) },
            new UpgradeDef { title = Localization.Get("upgrade_atkspeed_title"), description = Localization.Get("upgrade_atkspeed_desc"), apply = (u) => u.ApplyAttackSpeed(0.15f) },
            new UpgradeDef { title = Localization.Get("upgrade_range_title"), description = Localization.Get("upgrade_range_desc"), apply = (u) => u.ApplyAttackRange(0.5f) },
        };
    }

    void Awake()
    {
        if (headerText != null)
        {
            headerText.text = Localization.Get("level_up");
        }

        for (int i = 0; i < optionButtons.Length; i++)
        {
            int idx = i;
            if (optionButtons[i] != null)
            {
                optionButtons[i].onClick.AddListener(() => ChooseOption(idx));
            }
        }
    }

    void OnEnable()
    {
        TryBind();
    }

    void Update()
    {
        if (boundXP == null)
        {
            TryBind();
        }
    }

    void TryBind()
    {
        PlayerXP xp = Object.FindFirstObjectByType<PlayerXP>();
        if (xp == null || xp == boundXP) return;

        boundXP = xp;
        boundXP.OnLevelUp += HandleLevelUp;

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerUpgrades = playerObj.GetComponent<PlayerUpgrades>();
        }
    }

    void HandleLevelUp(int level)
    {
        ShowChoices();
    }

    void ShowChoices()
    {
        currentChoices = PickThreeRandom();
        for (int i = 0; i < optionTitles.Length && i < currentChoices.Length; i++)
        {
            if (optionTitles[i] != null) optionTitles[i].text = currentChoices[i].title;
            if (optionDescriptions[i] != null) optionDescriptions[i].text = currentChoices[i].description;
        }

        if (panel != null) panel.SetActive(true);
        Time.timeScale = 0f;
        if (AudioManager.Instance != null) AudioManager.Instance.PlayLevelUp();
    }

    UpgradeDef[] PickThreeRandom()
    {
        List<UpgradeDef> pool = new List<UpgradeDef>(BuildUpgradeList());
        int count = Mathf.Min(3, pool.Count);
        UpgradeDef[] result = new UpgradeDef[count];

        for (int i = 0; i < count; i++)
        {
            int idx = Random.Range(0, pool.Count);
            result[i] = pool[idx];
            pool.RemoveAt(idx);
        }

        return result;
    }

    public void ChooseOption(int index)
    {
        if (currentChoices == null || index < 0 || index >= currentChoices.Length) return;

        if (playerUpgrades != null && currentChoices[index].apply != null)
        {
            currentChoices[index].apply(playerUpgrades);
        }

        if (panel != null) panel.SetActive(false);
        Time.timeScale = 1f;
    }

    void OnDisable()
    {
        if (boundXP != null)
        {
            boundXP.OnLevelUp -= HandleLevelUp;
        }
    }
}
