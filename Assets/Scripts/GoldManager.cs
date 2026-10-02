using UnityEngine;

// Lifetime Gold balance, persisted across play sessions via PlayerPrefs -
// same pattern as HighScoreManager. This is the "bank": Gold earned during
// a run lives on PlayerGold until the run ends, then gets added here once.
public static class GoldManager
{
    private const string GoldKey = "lifetime_gold";

    public static int GetBalance()
    {
        return PlayerPrefs.GetInt(GoldKey, 0);
    }

    public static void AddGold(int amount)
    {
        if (amount <= 0) return;
        PlayerPrefs.SetInt(GoldKey, GetBalance() + amount);
        PlayerPrefs.Save();
    }

    // Returns false (and spends nothing) if the balance can't cover it.
    public static bool TrySpend(int amount)
    {
        int balance = GetBalance();
        if (amount <= 0 || balance < amount) return false;

        PlayerPrefs.SetInt(GoldKey, balance - amount);
        PlayerPrefs.Save();
        return true;
    }
}
