using UnityEngine;

// Thrown by EnemyRangedAI. Unlike the player's Projectile.cs this is a
// plain straight-line shot with no homing - homing is a player-weapon
// nicety for a knife that has to catch a dodging target, not something a
// ranged enemy needs. Flies in the direction it was fired, damages the
// Player on contact, then returns to the pool.
public class EnemyProjectile : MonoBehaviour
{
    public float speed = 5f;
    public float maxLifetime = 3f;
    public float hitRadius = 0.25f;

    [HideInInspector] public Vector2 direction = Vector2.right;
    [HideInInspector] public int damage = 1;

    private float spawnTime;
    private bool resolved;

    void OnEnable()
    {
        spawnTime = Time.time;
        resolved = false;
        ApplyRotation();
    }

    void Update()
    {
        if (resolved) return;

        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, hitRadius);
        for (int i = 0; i < hits.Length; i++)
        {
            PlayerHealth ph = hits[i].GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(damage);
                FxSpawner.Burst(transform.position, new Color(0.55f, 0.85f, 0.2f), 6, 2f, 0.2f, 0.1f);
                if (CameraFollow.Instance != null) CameraFollow.Instance.Shake(0.06f, 0.05f);
                Deactivate();
                return;
            }
        }

        if (Time.time - spawnTime >= maxLifetime)
        {
            Deactivate();
        }
    }

    void ApplyRotation()
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void Deactivate()
    {
        resolved = true;
        gameObject.SetActive(false);
    }
}
