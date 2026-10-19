using UnityEngine;

// Every enemy that currently spawns is a plain reskin of one of a few
// stat spreads (Grunt/Scout/Brute/Ranged) - no run ever produces a
// stand-out, "watch out for this one" encounter outside the Boss. This
// gives any regular spawn a small, time-scaling chance to come in as an
// Elite instead: visibly bigger and gold-tinted, noticeably tougher, and
// worth noticeably more XP/gold - a cheap source of replay variety that
// doesn't touch EnemyPool's level-gated roster at all. Attached to the
// Grunt/Scout/Brute/Ranged prefabs (see BuildGameUITool's "Build Elite
// Enemies"), never to the Boss - the Boss is already the singular
// scripted threat (BossSpawner), not part of the random spawn pool this
// rolls against.
public class EliteEnemy : MonoBehaviour
{
    public float minChance = 0.05f;
    public float maxChance = 0.35f;
    public float rampDuration = 240f;
    public float statMultiplier = 2.2f;
    public float scaleMultiplier = 1.35f;
    public Color eliteTint = new Color(1f, 0.85f, 0.2f, 1f);

    public bool IsElite { get; private set; }

    private EnemyHealth health;
    private EnemyAI ai;
    private EnemyRangedAI rangedAi;
    private Vector3 baseScale;
    private bool pendingRoll;

    void Awake()
    {
        health = GetComponent<EnemyHealth>();
        ai = GetComponent<EnemyAI>();
        rangedAi = GetComponent<EnemyRangedAI>();
        baseScale = transform.localScale;
    }

    void OnEnable()
    {
        IsElite = false;
        transform.localScale = baseScale;
        // Deferred to Update - see BoostElite's comment on EnemyHealth for
        // why this can't roll/apply here in OnEnable: EnemyHealth's and
        // EnemyAI's own OnEnable (which re-derive their stats from
        // DifficultyManager) need to run first, and Unity does not
        // guarantee component OnEnable order on the same GameObject.
        pendingRoll = true;
    }

    void Update()
    {
        if (!pendingRoll) return;
        pendingRoll = false;

        if (health != null && health.isBoss) return;

        float elapsed = DifficultyManager.GetElapsedSeconds();
        float t = Mathf.Clamp01(elapsed / rampDuration);
        float chance = Mathf.Lerp(minChance, maxChance, t);

        if (Random.value <= chance)
        {
            BecomeElite();
        }
    }

    void BecomeElite()
    {
        IsElite = true;

        if (health != null)
        {
            health.BoostElite(statMultiplier);
            health.SetBaseColor(eliteTint);
        }
        if (ai != null) ai.contactDamage = Mathf.Max(1, Mathf.RoundToInt(ai.contactDamage * statMultiplier));
        if (rangedAi != null) rangedAi.projectileDamage = Mathf.Max(1, Mathf.RoundToInt(rangedAi.projectileDamage * statMultiplier));

        transform.localScale = baseScale * scaleMultiplier;

        FxSpawner.Burst(transform.position, eliteTint, 12, 2f, 0.3f, 0.14f);
    }
}
