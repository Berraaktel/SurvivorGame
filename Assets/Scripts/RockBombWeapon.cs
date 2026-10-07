using UnityEngine;

// Third weapon: periodically lobs an exploding rock toward the nearest
// enemy (same targeting as PlayerAttack), but unlike the knife's single
// hit this damages every enemy caught in the blast once it lands -
// rewards throwing into a cluster instead of always hitting whichever
// enemy happens to be closest. Starts disabled - unlocked as a level-up
// choice (see PlayerUpgrades.UnlockRockBomb/UpgradeUI).
public class RockBombWeapon : MonoBehaviour
{
    public float throwInterval = 2.5f;
    public float throwRange = 5f;
    public int damage = 2;

    private float timer;

    void OnEnable()
    {
        // Re-zeroed every time this gets enabled (including the moment
        // it's unlocked mid-run), so the first throw doesn't fire
        // instantly off whatever time happened to accumulate while it
        // sat disabled since Awake.
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= throwInterval)
        {
            timer = 0f;
            TryThrow();
        }
    }

    void TryThrow()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, throwRange);

        Transform nearest = null;
        float nearestDist = float.MaxValue;
        foreach (Collider2D hit in hits)
        {
            if (hit.GetComponent<EnemyHealth>() == null) continue;

            float dist = (hit.transform.position - transform.position).sqrMagnitude;
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = hit.transform;
            }
        }

        if (nearest == null) return;

        Vector2 direction = ((Vector2)nearest.position - (Vector2)transform.position).normalized;
        if (RockBombPool.Instance != null)
        {
            RockBombPool.Instance.SpawnBomb(transform.position, direction, damage, throwRange + 1f);
            if (AudioManager.Instance != null) AudioManager.Instance.PlayThrow();
            // Small launch puff at the player's feet so the throw itself
            // is visible, not just the (easy to miss) impact moments later.
            FxSpawner.Burst(transform.position, new Color(0.6f, 0.45f, 0.3f), 6, 2.5f, 0.2f, 0.1f);
        }
    }
}
