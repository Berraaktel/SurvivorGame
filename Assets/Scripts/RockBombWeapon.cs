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

    // Evolution: max level + Can Yenileme (Health Regen) turns this into
    // "Meteor Yagmuru" - every throw lobs a second bomb instead of one.
    // Same shape as every other weapon here, see PlayerAttack for the
    // full explanation of the pattern.
    public int level = 1;
    public int maxLevel = 5;
    public bool evolved = false;

    private float timer;

    void OnEnable()
    {
        // Re-zeroed every time this gets enabled (including the moment
        // it's unlocked mid-run), so the first throw doesn't fire
        // instantly off whatever time happened to accumulate while it
        // sat disabled since Awake.
        timer = 0f;
    }

    public bool CanLevelUp()
    {
        return enabled && !evolved && level < maxLevel;
    }

    public void LevelUp()
    {
        level++;
        damage += 1;
        throwInterval = Mathf.Max(1.2f, throwInterval - 0.2f);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= throwInterval)
        {
            timer = 0f;
            TryThrow();
        }
        CheckEvolution();
    }

    void CheckEvolution()
    {
        if (evolved || level < maxLevel) return;

        PlayerUpgrades pu = GetComponent<PlayerUpgrades>();
        if (pu == null || !pu.HasHealthRegen()) return;

        evolved = true;
        damage += 1;

        if (EvolutionBanner.Instance != null) EvolutionBanner.Instance.Show(Localization.Get("evo_rockbomb"));
        FxSpawner.Burst(transform.position, new Color(1f, 0.85f, 0.35f), 32, 3.2f, 0.65f, 0.22f);
        if (CameraFollow.Instance != null) CameraFollow.Instance.Shake(0.28f, 0.2f);
        if (AudioManager.Instance != null) AudioManager.Instance.PlayLevelUp();
    }

    void TryThrow()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, throwRange);

        Transform nearest = null;
        Transform second = null;
        float nearestDist = float.MaxValue;
        float secondDist = float.MaxValue;
        foreach (Collider2D hit in hits)
        {
            if (hit.GetComponent<EnemyHealth>() == null) continue;

            float dist = (hit.transform.position - transform.position).sqrMagnitude;
            if (dist < nearestDist)
            {
                secondDist = nearestDist;
                second = nearest;
                nearestDist = dist;
                nearest = hit.transform;
            }
            else if (dist < secondDist)
            {
                secondDist = dist;
                second = hit.transform;
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

            if (evolved)
            {
                // Evolved "Meteor Yagmuru": a second bomb goes out at the
                // same time, toward the next-closest enemy if there is one
                // (otherwise a slight angle off the first) - twice the
                // blast coverage per throw instead of raising one bomb's
                // own numbers.
                Vector2 secondDir = second != null
                    ? ((Vector2)second.position - (Vector2)transform.position).normalized
                    : RotateDir(direction, 25f);
                RockBombPool.Instance.SpawnBomb(transform.position, secondDir, damage, throwRange + 1f);
            }
        }
    }

    Vector2 RotateDir(Vector2 dir, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(dir.x * cos - dir.y * sin, dir.x * sin + dir.y * cos);
    }
}
