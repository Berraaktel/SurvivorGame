using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackRange = 3f;
    public float attackInterval = 1f;
    public int damage = 1;
    private float timer;

    // Evolution: once this weapon hits maxLevel (via repeated "Bicagi
    // Guclendir" level-up picks) AND the player also has Armor, it
    // auto-evolves into "Bin Bicak" - same paired weapon+passive idea
    // every weapon below reuses, just with Armor as the knife's catalyst.
    public int level = 1;
    public int maxLevel = 5;
    public bool evolved = false;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= attackInterval)
        {
            timer = 0f;
            TryAttack();
        }
        CheckEvolution();
    }

    public bool CanLevelUp()
    {
        return enabled && !evolved && level < maxLevel;
    }

    public void LevelUp()
    {
        level++;
        damage += 1;
        attackInterval = Mathf.Max(0.35f, attackInterval - 0.05f);
    }

    void CheckEvolution()
    {
        if (evolved || level < maxLevel) return;

        PlayerUpgrades pu = GetComponent<PlayerUpgrades>();
        if (pu == null || !pu.HasArmor()) return;

        evolved = true;
        damage += 2;

        if (EvolutionBanner.Instance != null) EvolutionBanner.Instance.Show(Localization.Get("evo_knife"));
        FxSpawner.Burst(transform.position, new Color(1f, 0.92f, 0.45f), 32, 3.2f, 0.65f, 0.22f);
        if (CameraFollow.Instance != null) CameraFollow.Instance.Shake(0.28f, 0.2f);
        if (AudioManager.Instance != null) AudioManager.Instance.PlayLevelUp();
    }

    void TryAttack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);

        EnemyHealth nearest = null;
        float nearestDist = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
            if (enemy == null) continue;
            // Range can grow a lot over a run (level-up picks keep adding
            // to it) - without this, a throw aimed at something completely
            // off-screen would land and kill it with no visible cause, so
            // only enemies the player can actually see are valid targets.
            if (CameraFollow.Instance != null && !CameraFollow.Instance.IsOnScreen(hit.transform.position)) continue;

            float dist = (hit.transform.position - transform.position).sqrMagnitude;
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = enemy;
            }
        }

        if (nearest == null) return;

        Vector2 direction = ((Vector2)nearest.transform.position - (Vector2)transform.position).normalized;

        if (ProjectilePool.Instance != null)
        {
            // A little slack past attackRange so a throw aimed at a
            // target that then steps away still has a fair chance to land,
            // instead of stopping short and looking like it fizzled early.
            if (evolved)
            {
                // Evolved "Bin Bicak": the single homing throw becomes a
                // 3-way spread so one attack threatens a cluster instead
                // of only ever the nearest enemy - the outer two are aimed
                // off to the sides and don't home onto nearest.transform.
                // evolvedKnife=true on every one of the three so all of
                // them show the bright evolved tint/size, not just a mix.
                ProjectilePool.Instance.SpawnProjectile(transform.position, direction, damage, attackRange + 1f, nearest.transform, true);
                ProjectilePool.Instance.SpawnProjectile(transform.position, RotateDir(direction, 18f), damage, attackRange + 1f, null, true);
                ProjectilePool.Instance.SpawnProjectile(transform.position, RotateDir(direction, -18f), damage, attackRange + 1f, null, true);
            }
            else
            {
                ProjectilePool.Instance.SpawnProjectile(transform.position, direction, damage, attackRange + 1f, nearest.transform);
            }
            if (AudioManager.Instance != null) AudioManager.Instance.PlayThrow();
        }
        else
        {
            // Pool not built yet in this scene - fall back to an instant hit
            // so gameplay still works until "Build Ranged Weapon" is run.
            nearest.TakeDamage(damage);
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
