using UnityEngine;

// Gives the Boss its own attack pattern, separate from EnemyAI's ordinary
// melee chase+contact damage that every enemy (Scout/Brute/Ranged/Boss)
// already shares. Added directly to Enemy_Boss.prefab only - never to the
// shared base Enemy.prefab - so Scout/Brute/Ranged never inherit it the
// way EliteEnemy's component does. Without this the boss was mechanically
// identical to a normal chaser with more HP; a real encounter needs a
// pattern the player has to actually react to.
//
// Pattern: every attackInterval seconds the boss telegraphs (color pulse +
// FX burst + cast sound) for telegraphDuration, then fires a full ring of
// projectileCount shots outward from its own position using the same
// pooled EnemyProjectilePool every Ranged enemy already fires through -
// aimed in a circle instead of at the player, so dodging means moving out
// of the ring during the telegraph rather than reacting to the shots
// themselves.
public class BossAttack : MonoBehaviour
{
    public float attackInterval = 6f;
    public float telegraphDuration = 0.6f;
    public int projectileCount = 10;
    public int baseProjectileDamage = 2;
    public Color telegraphColor = new Color(1f, 0.25f, 0.25f, 1f);

    private float timer;
    private bool telegraphing;
    private float telegraphTimer;
    private int scaledDamage;
    private SpriteRenderer sr;
    private Color baseColor;

    void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    void OnEnable()
    {
        // Same "snapshot base, rescale on enable" pattern as
        // EnemyAI.contactDamage - this object is pooled and reused many
        // times across a run as DifficultyManager's multiplier keeps
        // rising, so the damage has to be recomputed each time, not once.
        timer = 0f;
        telegraphing = false;
        telegraphTimer = 0f;
        scaledDamage = Mathf.Max(1, Mathf.RoundToInt(baseProjectileDamage * DifficultyManager.GetMultiplier()));
    }

    void Update()
    {
        if (telegraphing)
        {
            telegraphTimer += Time.deltaTime;
            // Pulse between the boss's current color and telegraphColor so
            // the warning still reads even while EnemyHealth's own
            // hit-flash or EliteEnemy's tint is also driving sr.color.
            float t = Mathf.PingPong(telegraphTimer * 10f, 1f);
            if (sr != null) sr.color = Color.Lerp(baseColor, telegraphColor, t);

            if (telegraphTimer >= telegraphDuration)
            {
                telegraphing = false;
                if (sr != null) sr.color = baseColor;
                FireRing();
            }
            return;
        }

        timer += Time.deltaTime;
        if (timer >= attackInterval)
        {
            timer = 0f;
            StartTelegraph();
        }
    }

    void StartTelegraph()
    {
        telegraphing = true;
        telegraphTimer = 0f;
        if (sr != null) baseColor = sr.color;
        FxSpawner.Burst(transform.position, telegraphColor, 16, 1.5f, telegraphDuration, 0.12f);
        if (AudioManager.Instance != null) AudioManager.Instance.PlayThrow();
    }

    void FireRing()
    {
        if (EnemyProjectilePool.Instance == null) return;

        for (int i = 0; i < projectileCount; i++)
        {
            float angle = (360f / projectileCount) * i * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            EnemyProjectilePool.Instance.SpawnProjectile(transform.position, dir, scaledDamage);
        }

        if (CameraFollow.Instance != null) CameraFollow.Instance.Shake(0.15f, 0.12f);
        if (AudioManager.Instance != null) AudioManager.Instance.PlayHit();
    }
}
