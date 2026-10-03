using UnityEngine;

// Third enemy archetype, alongside EnemyAI's melee chasers: this one keeps
// its distance and attacks with a thrown projectile instead of contact
// damage. Without this every threat in the game rewarded the exact same
// response (stand still, let the knife/orbit blades handle it) - a Ranged
// enemy punishes that passivity from outside contact range, giving the
// player an actual reason to reposition during a run.
public class EnemyRangedAI : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float preferredRange = 3.5f;

    // How far from preferredRange the enemy tolerates before bothering to
    // reposition. Without this slack it jitters back and forth every
    // FixedUpdate as it overshoots the exact distance in both directions.
    public float rangeSlack = 0.5f;

    public float fireInterval = 1.8f;
    public int projectileDamage = 1;

    private Rigidbody2D rb;
    private Transform player;
    private float fireTimer;
    private int baseProjectileDamage;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // See EnemyHealth.Awake for why this is snapshotted once instead of
        // scaled in place - this object is pooled and reactivated many
        // times across a run as DifficultyManager's multiplier keeps rising.
        baseProjectileDamage = projectileDamage;
    }

    void OnEnable()
    {
        projectileDamage = Mathf.Max(1, Mathf.RoundToInt(baseProjectileDamage * DifficultyManager.GetMultiplier()));
        fireTimer = 0f;
    }

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        Vector2 toPlayer = (Vector2)player.position - rb.position;
        float dist = toPlayer.magnitude;
        Vector2 dir = dist > 0.0001f ? toPlayer / dist : Vector2.zero;

        if (dist > preferredRange + rangeSlack)
        {
            // Too far - close in like a normal chaser.
            rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
        }
        else if (dist < preferredRange - rangeSlack)
        {
            // Too close - back off to keep fighting at range instead of
            // getting pulled into contact range like a melee enemy would.
            rb.MovePosition(rb.position - dir * moveSpeed * Time.fixedDeltaTime);
        }

        fireTimer += Time.fixedDeltaTime;
        if (fireTimer >= fireInterval)
        {
            fireTimer = 0f;
            if (EnemyProjectilePool.Instance != null && dist > 0.0001f)
            {
                EnemyProjectilePool.Instance.SpawnProjectile(rb.position, dir, projectileDamage);
            }
        }
    }
}
