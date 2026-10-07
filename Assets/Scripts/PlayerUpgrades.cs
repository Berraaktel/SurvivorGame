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

        // Keep every weapon scaling together - without this, whichever
        // weapon isn't the thrown knife quietly falls behind every level,
        // since this is the only "more damage" choice the level-up screen
        // offers. Each new weapon added here needs the same line.
        OrbitWeapon ow = GetComponent<OrbitWeapon>();
        if (ow != null) ow.SetDamage(ow.damage + amount);

        RockBombWeapon rb = GetComponent<RockBombWeapon>();
        if (rb != null) rb.damage += amount;

        DustNova dn = GetComponent<DustNova>();
        if (dn != null) dn.damage += amount;
    }

    // Weapon-unlock level-up choices (see UpgradeUI.BuildUpgradeList) read
    // these to decide whether a weapon is still worth offering, and call
    // the matching UnlockX() to enable it - the component already sits on
    // the Player GameObject (added by BuildGameUITool's "Build Rock Bomb
    // Weapon" / "Build Dust Nova Weapon" buttons) but starts disabled, so
    // unlocking is just flipping it on.
    public bool HasRockBomb()
    {
        RockBombWeapon rb = GetComponent<RockBombWeapon>();
        return rb != null && rb.enabled;
    }

    public bool HasDustNova()
    {
        DustNova dn = GetComponent<DustNova>();
        return dn != null && dn.enabled;
    }

    public void UnlockRockBomb()
    {
        RockBombWeapon rb = GetComponent<RockBombWeapon>();
        if (rb != null) rb.enabled = true;
    }

    public void UnlockDustNova()
    {
        DustNova dn = GetComponent<DustNova>();
        if (dn != null) dn.enabled = true;
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
