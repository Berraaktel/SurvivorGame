using UnityEngine;
using System;

public class PlayerXP : MonoBehaviour
{
    public int level = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 5;
    public float xpGrowth = 1.3f;

    public event Action<int, int> OnXPChanged;
    public event Action<int> OnLevelUp;

    void Start()
    {
        if (OnXPChanged != null) OnXPChanged(currentXP, xpToNextLevel);
    }

    public void AddXP(int amount)
    {
        currentXP += amount;

        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            level++;
            xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * xpGrowth);
            Debug.Log("Level up! Yeni seviye: " + level);
            if (OnLevelUp != null) OnLevelUp(level);
        }

        if (OnXPChanged != null) OnXPChanged(currentXP, xpToNextLevel);
    }
}
