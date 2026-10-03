using UnityEngine;

// Replaces the old flat "+1 damage" permanent upgrade with a tiered
// weapon-skin ladder: each tier is both a recolor of the thrown knife and
// a bigger damage bonus, so one Shop purchase pays off visually and
// mechanically at the same time (same idea as the gold-tinted "Altin
// Kahraman" character skin, just applied to the weapon and stacked into
// levels instead of a single unlock). Persisted via PlayerPrefs, same
// pattern as PermanentUpgrades/GoldManager.
public static class KnifeTiers
{
    private const string TierKey = "upg_knife_tier";

    public const int TierCount = 4; // 0=Bronz, 1=Gumus, 2=Altin, 3=Elmas

    private static readonly string[] NamesTr = { "Bronz Bicak", "Gumus Bicak", "Altin Bicak", "Elmas Bicak" };
    private static readonly string[] NamesEn = { "Bronze Knife", "Silver Knife", "Golden Knife", "Diamond Knife" };

    // Tier 0 is the knife every run already starts with, so it has no
    // price - the Shop only ever sells tiers 1-3.
    private static readonly int[] Prices = { 0, 300, 700, 1400 };
    private static readonly int[] DamageBonusByTier = { 0, 1, 3, 6 };

    // Multiplicative SpriteRenderer tints, so each needs to be a fairly
    // saturated, distinct hue - a near-white "silver" barely shows up
    // against the knife art's own light colors once multiplied in.
    private static readonly Color[] TierTints =
    {
        new Color(0.72f, 0.45f, 0.20f),        // Bronz - copper/bronze
        new Color(0.60f, 0.68f, 0.80f),        // Gumus - cool steel-blue silver
        new Color(1f, 0.84f, 0f),              // Altin - gold, matches the Gold pickups/skin
        new Color(0.45f, 0.85f, 1f),           // Elmas - bright diamond-blue
    };

    public static int GetTier()
    {
        return Mathf.Clamp(PlayerPrefs.GetInt(TierKey, 0), 0, TierCount - 1);
    }

    public static bool IsMaxTier()
    {
        return GetTier() >= TierCount - 1;
    }

    public static string GetTierName(int tier)
    {
        tier = Mathf.Clamp(tier, 0, TierCount - 1);
        return Localization.CurrentLanguage == "tr" ? NamesTr[tier] : NamesEn[tier];
    }

    public static string GetCurrentTierName()
    {
        return GetTierName(GetTier());
    }

    public static Color GetTint()
    {
        return TierTints[GetTier()];
    }

    public static int GetDamageBonus()
    {
        return DamageBonusByTier[GetTier()];
    }

    public static int GetNextTierPrice()
    {
        int tier = GetTier();
        if (tier >= TierCount - 1) return 0;
        return Prices[tier + 1];
    }

    public static int GetNextDamageBonus()
    {
        int tier = GetTier();
        int next = Mathf.Min(tier + 1, TierCount - 1);
        return DamageBonusByTier[next];
    }

    // Returns true if the purchase went through (enough Gold, not already
    // at the top tier); false otherwise, with nothing spent.
    public static bool TryBuyNextTier()
    {
        int tier = GetTier();
        if (tier >= TierCount - 1) return false;

        int price = Prices[tier + 1];
        if (!GoldManager.TrySpend(price)) return false;

        PlayerPrefs.SetInt(TierKey, tier + 1);
        PlayerPrefs.Save();
        return true;
    }
}
