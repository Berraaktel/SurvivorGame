using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackRange = 3f;
    public float attackInterval = 1f;
    public int damage = 1;
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= attackInterval)
        {
            timer = 0f;
            TryAttack();
        }
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
            ProjectilePool.Instance.SpawnProjectile(transform.position, direction, damage, attackRange + 1f, nearest.transform);
            if (AudioManager.Instance != null) AudioManager.Instance.PlayThrow();
        }
        else
        {
            // Pool not built yet in this scene - fall back to an instant hit
            // so gameplay still works until "Build Ranged Weapon" is run.
            nearest.TakeDamage(damage);
        }
    }
}
