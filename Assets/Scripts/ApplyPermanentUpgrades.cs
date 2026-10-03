using UnityEngine;

// Applies Shop-purchased permanent upgrades (see PermanentUpgrades) to
// this run's starting stats. [DefaultExecutionOrder(-100)] guarantees
// this Awake() runs before PlayerHealth/PlayerAttack/OrbitWeapon's own
// Awake()/Start() (default order 0), so the bonus is already in place
// before PlayerHealth captures maxHealth into currentHealth and before
// OrbitWeapon spawns its first ring of blades.
[DefaultExecutionOrder(-100)]
public class ApplyPermanentUpgrades : MonoBehaviour
{
    void Awake()
    {
        PlayerHealth ph = GetComponent<PlayerHealth>();
        if (ph != null) ph.maxHealth += PermanentUpgrades.GetExtraHealth();

        PlayerAttack pa = GetComponent<PlayerAttack>();
        if (pa != null) pa.damage += KnifeTiers.GetDamageBonus();

        OrbitWeapon ow = GetComponent<OrbitWeapon>();
        if (ow != null) ow.damage += KnifeTiers.GetDamageBonus();

        if (PermanentUpgrades.IsSkinUnlocked())
        {
            // Same child-vs-root sprite lookup EnemyHealth/PlayerMovement
            // use, since Build Player Walk Animation may have moved the
            // visible sprite onto a "SpriteVisual" child.
            Transform visual = transform.Find("SpriteVisual");
            SpriteRenderer sr = visual != null ? visual.GetComponent<SpriteRenderer>() : GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = new Color(1f, 0.84f, 0f);
        }
    }
}
