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

    // Passive items (Can Yenileme / Miknatis / Zirh) - same level-up
    // unlock pattern as the weapons above: HasX() gates the UpgradeUI
    // pool entry, UnlockX() flips it on. Unlike the weapons these don't
    // all live on a single disabled/enabled component, since Magnet and
    // Armor are single-value tweaks rather than their own behaviour.
    public bool HasHealthRegen()
    {
        HealthRegen hr = GetComponent<HealthRegen>();
        return hr != null && hr.enabled;
    }

    public void UnlockHealthRegen()
    {
        HealthRegen hr = GetComponent<HealthRegen>();
        if (hr != null) hr.enabled = true;
        PlayPassiveUnlockFx(new Color(1f, 0.92f, 0.55f));
    }

    public bool HasMagnet()
    {
        return XPOrb.magnetMultiplier > 1f;
    }

    public void UnlockMagnet()
    {
        XPOrb.magnetMultiplier = 1.8f;
        GoldOrb.magnetMultiplier = 1.8f;
        PlayPassiveUnlockFx(new Color(0.55f, 0.75f, 1f));
    }

    public bool HasArmor()
    {
        PlayerHealth ph = GetComponent<PlayerHealth>();
        return ph != null && ph.armor > 0;
    }

    public void UnlockArmor()
    {
        PlayerHealth ph = GetComponent<PlayerHealth>();
        if (ph != null) ph.armor += 1;
        PlayPassiveUnlockFx(new Color(0.6f, 0.65f, 0.75f));
    }

    // The 3 passives above have no ongoing effect a player can clearly
    // SEE the moment they're picked (Armor's damage reduction is silent,
    // Magnet's wider pickup range only shows up later on a distant orb,
    // Health Regen's first tick is seconds away) - without this, picking
    // one looked and sounded exactly like picking nothing. Each gets its
    // own tint so the three stay visually distinct from each other too.
    void PlayPassiveUnlockFx(Color tint)
    {
        FxSpawner.Burst(transform.position, tint, 18, 2.6f, 0.45f, 0.16f);
        if (AudioManager.Instance != null) AudioManager.Instance.PlayPickup();
    }
}
