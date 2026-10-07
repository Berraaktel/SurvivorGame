using UnityEngine;

// Fourth weapon archetype: a periodic pulse centered on the player,
// damaging everything within radius at once. Unlike every other weapon
// here it needs no target and no throw - it rewards standing your ground
// in a crowd instead of always kiting or aiming at the single nearest
// enemy. Starts disabled - unlocked as a level-up choice (see
// PlayerUpgrades.UnlockDustNova/UpgradeUI).
public class DustNova : MonoBehaviour
{
    public float pulseInterval = 2f;
    public float radius = 2.2f;
    public int damage = 1;

    private float timer;

    void OnEnable()
    {
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= pulseInterval)
        {
            timer = 0f;
            Pulse();
        }
    }

    void Pulse()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        bool hitAny = false;
        for (int i = 0; i < hits.Length; i++)
        {
            EnemyHealth enemy = hits[i].GetComponent<EnemyHealth>();
            if (enemy == null) continue;

            enemy.TakeDamage(damage);
            hitAny = true;
        }

        // Particles start at the player and travel outward fast enough to
        // reach 'radius' by the time their lifetime ends, so the burst
        // itself reads as an expanding dust ring instead of a small pop.
        // Count/size/lifetime bumped up from the first pass - at 18
        // particles over 0.3s the ring faded before it was noticeable in
        // a crowded screen.
        float lifetime = 0.45f;
        FxSpawner.Burst(transform.position, new Color(0.85f, 0.72f, 0.45f, 0.85f), 28, radius / lifetime, lifetime, 0.2f);

        // Always audible, even on a pulse that hits nothing - otherwise
        // the only sign the ability fired at all is the particle ring,
        // which is easy to miss mid-fight.
        if (AudioManager.Instance != null) AudioManager.Instance.PlayThrow();

        if (hitAny)
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlayHit();
            if (CameraFollow.Instance != null) CameraFollow.Instance.Shake(0.1f, 0.08f);
        }
    }
}
