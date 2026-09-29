using System.Collections.Generic;
using UnityEngine;

// A single spinning blade orbiting the player, positioned every frame by
// OrbitWeapon. Deals damage to anything it touches, with a short per-enemy
// cooldown so a blade sitting in contact with a slow enemy ticks like a
// lawn-mower blade instead of melting it in one frame.
public class OrbitBlade : MonoBehaviour
{
    public float hitRadius = 0.3f;
    public float hitCooldown = 0.5f;
    [HideInInspector] public int damage = 1;

    private Dictionary<EnemyHealth, float> lastHitTime = new Dictionary<EnemyHealth, float>();

    void Update()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, hitRadius);
        for (int i = 0; i < hits.Length; i++)
        {
            EnemyHealth enemy = hits[i].GetComponent<EnemyHealth>();
            if (enemy == null) continue;

            float last;
            if (lastHitTime.TryGetValue(enemy, out last) && Time.time - last < hitCooldown) continue;

            lastHitTime[enemy] = Time.time;
            enemy.TakeDamage(damage);
            if (AudioManager.Instance != null) AudioManager.Instance.PlayHit();
            FxSpawner.Burst(transform.position, new Color(0.55f, 0.85f, 0.85f), 5, 1.8f, 0.18f, 0.09f);
        }
    }
}
