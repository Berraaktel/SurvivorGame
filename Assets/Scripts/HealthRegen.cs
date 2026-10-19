using UnityEngine;

// Fifth passive-style ability (alongside the Rock Bomb / Dust Nova
// weapons): slowly heals the player over time instead of dealing damage.
// Every other level-up choice either hurts enemies or boosts an attack -
// without this there is no way to offset the chip damage a long run
// accumulates except the one-time Max Health bump. Starts disabled -
// unlocked as a level-up choice (see PlayerUpgrades.UnlockHealthRegen/
// UpgradeUI), same pattern as RockBombWeapon/DustNova.
public class HealthRegen : MonoBehaviour
{
    public float regenInterval = 3f;
    public int healAmount = 1;

    private float timer;
    private PlayerHealth playerHealth;

    void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
    }

    void OnEnable()
    {
        // Re-zeroed on every enable (including the moment it's unlocked
        // mid-run), same reasoning as RockBombWeapon/DustNova's timers -
        // otherwise the first tick could fire instantly.
        timer = 0f;
    }

    void Update()
    {
        if (playerHealth == null || playerHealth.IsDead) return;

        timer += Time.deltaTime;
        if (timer < regenInterval) return;

        timer = 0f;
        if (playerHealth.CurrentHealth >= playerHealth.maxHealth) return;

        playerHealth.Heal(healAmount);
        FxSpawner.Burst(transform.position, new Color(0.35f, 0.9f, 0.45f), 8, 1.8f, 0.3f, 0.12f);
    }
}
