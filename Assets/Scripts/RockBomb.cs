using UnityEngine;

// Third weapon archetype: a lobbed rock that explodes in an area on
// impact (or once it runs out of flight distance/time), rewarding a
// throw into a cluster of enemies instead of only ever hitting whichever
// one happens to be nearest - unlike the thrown knife's single-target
// homing hit or the orbit blades' continuous melee ring.
public class RockBomb : MonoBehaviour
{
    public float speed = 4.5f;
    public float maxLifetime = 2.5f;
    public float explosionRadius = 1.6f;
    public float spinSpeed = 480f;

    [HideInInspector] public Vector2 direction = Vector2.right;
    [HideInInspector] public int damage = 2;
    [HideInInspector] public float maxDistance = 5f;

    private float spawnTime;
    private float distanceTraveled;
    private bool resolved;

    void OnEnable()
    {
        spawnTime = Time.time;
        distanceTraveled = 0f;
        resolved = false;
        transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        if (resolved) return;

        float step = speed * Time.deltaTime;
        transform.position += (Vector3)(direction * step);
        distanceTraveled += step;
        // Tumbles in flight so a thrown rock reads as thrown, not just
        // gliding - the sprite has no animation of its own.
        transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);

        if (HasEnemyAt(transform.position, 0.25f) || distanceTraveled >= maxDistance || Time.time - spawnTime >= maxLifetime)
        {
            Explode();
        }
    }

    bool HasEnemyAt(Vector3 pos, float radius)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, radius);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].GetComponent<EnemyHealth>() != null) return true;
        }
        return false;
    }

    void Explode()
    {
        resolved = true;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        for (int i = 0; i < hits.Length; i++)
        {
            EnemyHealth enemy = hits[i].GetComponent<EnemyHealth>();
            if (enemy != null) enemy.TakeDamage(damage);
        }

        if (AudioManager.Instance != null) AudioManager.Instance.PlayHit();
        FxSpawner.Burst(transform.position, new Color(0.82f, 0.52f, 0.22f), 14, 3.5f, 0.35f, 0.16f);
        if (CameraFollow.Instance != null) CameraFollow.Instance.Shake(0.12f, 0.09f);

        gameObject.SetActive(false);
    }
}
