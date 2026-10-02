using UnityEngine;
using System;

// Run-local Gold counter, mirrors PlayerXP: GoldOrb pickups add to this
// during the run, and GameOverUI banks the total into GoldManager once
// the run ends. Kept separate from GoldManager so a run's Gold only
// becomes permanent on death, not the instant it's picked up.
public class PlayerGold : MonoBehaviour
{
    public int runGold = 0;

    public event Action<int> OnGoldChanged;

    public void AddGold(int amount)
    {
        runGold += amount;
        if (OnGoldChanged != null) OnGoldChanged(runGold);
    }
}
