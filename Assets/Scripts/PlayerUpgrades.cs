using UnityEngine;

public class PlayerUpgrades : MonoBehaviour
{
    public void ApplyMoveSpeed(float amount)
    {
        PlayerMovement pm = GetComponent<PlayerMovement>();
        if (pm != null) pm.moveSpeed += amount;
    }

    public void ApplyMaxHealth(int amount)
    {
        PlayerHealth ph = GetComponent<PlayerHealth>();
        if (ph != null)
        {
            ph.maxHealth += amount;
            ph.Heal(amount);
        }
    }

    public void ApplyDamage(int amount)
    {
        PlayerAttack pa = GetComponent<PlayerAttack>();
        if (pa != null) pa.damage += amount;

        // Keep both weapons scaling together - without this the orbit
        // blades quietly fall behind every level, since this is the only
        // "more damage" choice the level-up screen offers.
        OrbitWeapon ow = GetComponent<OrbitWeapon>();
        if (ow != null) ow.SetDamage(ow.damage + amount);
    }

    public void ApplyAttackSpeed(float reduceSeconds)
    {
        PlayerAttack pa = GetComponent<PlayerAttack>();
        if (pa != null) pa.attackInterval = Mathf.Max(0.15f, pa.attackInterval - reduceSeconds);
    }

    public void ApplyAttackRange(float amount)
    {
        PlayerAttack pa = GetComponent<PlayerAttack>();
        if (pa != null) pa.attackRange += amount;
    }
}
