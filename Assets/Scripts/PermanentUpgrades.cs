using UnityEngine;

// Meta-progression purchased in the Shop with Gold (see GoldManager),
// persisted via PlayerPrefs so it survives between runs and app launches.
// ApplyPermanentUpgrades reads this at the start of every run and bumps
// the player's starting stats accordingly.
public static class PermanentUpgrades
{
    private const string HealthLevelKey = "upg_health_level";
    private const string DamageLevelKey = "upg_damage_level";
    private const string SkinUnlockedKey = "upg_skin_unlocked";

    public const int MaxHealthLevel = 5;
    public const int MaxDamageLevel = 5;
    public const int HealthLevelCost = 200;
    public const int DamageLevelCost = 300;
    public const int SkinCost = 500;

    // How much each level actually adds to the player's stats - kept in
    // step with the in-run level-up upgrades (PlayerUpgrades) so a
    // permanent level feels like one extra free pick, not a different unit.
    public const int HealthPerLevel = 2;
    public const int DamagePerLevel = 1;

    public static int GetHealthLevel()
    {
        return PlayerPrefs.GetInt(HealthLevelKey, 0);
    }

    public static int GetDamageLevel()
    {
        return PlayerPrefs.GetInt(DamageLevelKey, 0);
    }

    public static bool IsSkinUnlocked()
    {
        return PlayerPrefs.GetInt(SkinUnlockedKey, 0) == 1;
    }

    public static int GetExtraHealth()
    {
        return GetHealthLevel() * HealthPerLevel;
    }

    public static int GetExtraDamage()
    {
        return GetDamageLevel() * DamagePerLevel;
    }

    // Each returns true if the purchase went through (enough Gold, not
    // already maxed/unlocked); false otherwise, with nothing spent.
    public static bool TryBuyHealthLevel()
    {
        int level = GetHealthLevel();
        if (level >= MaxHealthLevel) return false;
        if (!GoldManager.TrySpend(HealthLevelCost)) return false;

        PlayerPrefs.SetInt(HealthLevelKey, level + 1);
        PlayerPrefs.Save();
        return true;
    }

    public static bool TryBuyDamageLevel()
    {
        int level = GetDamageLevel();
        if (level >= MaxDamageLevel) return false;
        if (!GoldManager.TrySpend(DamageLevelCost)) return false;

        PlayerPrefs.SetInt(DamageLevelKey, level + 1);
        PlayerPrefs.Save();
        return true;
    }

    public static bool TryBuySkin()
    {
        if (IsSkinUnlocked()) return false;
        if (!GoldManager.TrySpend(SkinCost)) return false;

        PlayerPrefs.SetInt(SkinUnlockedKey, 1);
        PlayerPrefs.Save();
        return true;
    }
}
