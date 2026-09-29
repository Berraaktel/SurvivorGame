using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float stoppingDistance = 0.25f;
    private Rigidbody2D rb;
    private Transform player;

    public int contactDamage = 1;
    public float damageInterval = 1f;

    // How close (world units, center-to-center) counts as "touching" the
    // player for contact damage. This is a direct distance check instead
    // of relying on the trigger collider's Physics2D overlap - these
    // colliders are tiny and several enemies can be stacked in the same
    // spot, which made the old OnTriggerStay2D-based damage unreliable
    // (worked, then stopped working, depending on exact collider sizing).
    // A plain distance compare is deterministic regardless of collider
    // size or how many enemies are overlapping.
    public float contactRange = 0.3f;
    private float damageTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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

        if (dist > stoppingDistance)
        {
            Vector2 direction = toPlayer / dist;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
        }

        if (dist <= contactRange)
        {
            damageTimer += Time.fixedDeltaTime;
            if (damageTimer >= damageInterval)
            {
                damageTimer = 0f;
                PlayerHealth ph = player.GetComponent<PlayerHealth>();
                if (ph != null)
                {
                    ph.TakeDamage(contactDamage);
                }
            }
        }
        else
        {
            damageTimer = 0f;
        }
    }
}
