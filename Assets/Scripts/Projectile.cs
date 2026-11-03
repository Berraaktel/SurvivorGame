using UnityEngine;

// Simple projectile: flies toward wherever it was aimed, with a gentle
// homing nudge toward the specific enemy it was thrown at (so a moving
// target doesn't just slip past a rigid straight-line throw). Checks for
// an enemy overlap every frame (Physics2D.OverlapCircleAll), deals damage
// on the first hit, then goes back to the pool. No Rigidbody2D/Collider2D
// needed on the projectile itself.
public class Projectile : MonoBehaviour
{
    public float speed = 6f;
    public float maxLifetime = 2f;
    public float hitRadius = 0.28f;

    // Enemies usually fight at point-blank range against the player, so the
    // "nearest enemy" PlayerAttack targets is often already touching the
    // player. Without this delay the knife would register a hit on its very
    // first frame - before it ever visibly moves. This guarantees a short
    // minimum flight so the throw is always visible, even at melee range.
    // Distance-based (not time-based) so it stays consistent regardless of
    // speed - a time-based delay forced the knife to travel further than
    // melee contact range, which made it overshoot and loop back around.
    public float minArmDistance = 0.15f;

    // How fast the knife can curve toward its target, in degrees/second.
    // Keeps the throw feeling like a thrown weapon (mostly straight) while
    // still being able to catch up with a target that has moved since the
    // throw was aimed.
    public float turnSpeed = 720f;

    // How far this specific throw is allowed to travel before it gives up.
    // PlayerAttack sets this to roughly the attack range at the moment of
    // firing, so a miss disappears at a sensible distance instead of
    // sailing off into empty desert for its full maxLifetime.
    [HideInInspector] public float maxDistance = 6f;

    [HideInInspector] public Vector2 direction = Vector2.right;
    [HideInInspector] public int damage = 1;

    // Set by PlayerAttack when the evolved "Bin Bicak" fires. Originally
    // this only swapped in a gold tint + bigger scale on the same Kenney
    // sprite, but that read as "same knife, slightly bigger" rather than a
    // real evolution (per direct testing feedback). Now, when evolvedSprite
    // is assigned (wired in BuildGameUITool's "Wire Evolved Knife Sprite"
    // button), the projectile swaps to that hand-tuned sprite outright
    // instead of just tinting the base one - a genuine silhouette/color
    // change, not a recolor. Falls back to the old tint+scale behavior if
    // evolvedSprite was never wired, so nothing breaks if the art is
    // missing.
    [HideInInspector] public bool evolvedKnife = false;
    public Sprite evolvedSprite;

    // The sprite this projectile normally shows (the plain Kenney knife),
    // cached once in Awake so OnEnable can always restore it when a throw
    // ISN'T the evolved one, even after a previous throw swapped the
    // sprite out.
    private Sprite baseSprite;

    // The specific enemy this knife was aimed at. May die or despawn before
    // the knife arrives - in that case it just keeps flying straight in its
    // last known direction instead of erroring out.
    [HideInInspector] public Transform target;

    private float spawnTime;
    private float distanceTraveled;
    private bool resolved;
    private SpriteRenderer sr;

    // Drives the in-flight sparkle trail on evolved throws - see Update().
    // Timer-based (not every-frame) so it reads as a handful of distinct
    // glints along the flight path rather than a solid smear of particles.
    private float sparkleTimer;
    private const float SparkleInterval = 0.07f;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) baseSprite = sr.sprite;
    }

    void OnEnable()
    {
        spawnTime = Time.time;
        distanceTraveled = 0f;
        resolved = false;
        sparkleTimer = 0f;
        // Re-applied on every throw (not just once) - tier can only change
        // from the Main Menu Shop between runs, but this keeps the knife
        // always showing whatever tier is actually owned without needing
        // a separate "weapon changed" event. evolvedKnife overrides it
        // outright so the evolution is visible regardless of tier.
        if (sr != null)
        {
            if (evolvedKnife && evolvedSprite != null)
            {
                // Real custom sprite for the evolved throw - no tint needed,
                // the art itself carries the gold/white evolved look.
                sr.sprite = evolvedSprite;
                sr.color = Color.white;
                transform.localScale = Vector3.one;
            }
            else
            {
                sr.sprite = baseSprite;
                sr.color = evolvedKnife ? new Color(1f, 0.85f, 0.25f) : KnifeTiers.GetTint();
                transform.localScale = evolvedKnife ? new Vector3(1.4f, 1.4f, 1f) : Vector3.one;
            }
        }
        ApplyRotation();
    }

    void Update()
    {
        if (resolved) return;

        if (target != null && target.gameObject.activeInHierarchy)
        {
            Vector2 toTarget = (Vector2)target.position - (Vector2)transform.position;
            if (toTarget.sqrMagnitude > 0.0001f)
            {
                Vector2 toTargetDir = toTarget.normalized;
                // Only steer while the target is still roughly ahead of us.
                // Once we've passed it (dot product goes negative), homing
                // would curve the knife backward into a visible loop - so
                // past that point we just keep flying straight instead.
                if (Vector2.Dot(direction, toTargetDir) > 0f)
                {
                    Vector3 desired = (Vector3)toTargetDir;
                    direction = Vector3.RotateTowards(direction, desired, turnSpeed * Mathf.Deg2Rad * Time.deltaTime, 0f);
                }
            }
        }

        ApplyRotation();

        float step = speed * Time.deltaTime;
        transform.position += (Vector3)(direction * step);
        distanceTraveled += step;

        // In-flight sparkle trail for the evolved throw - makes the glint
        // on the custom sprite actually visible while it's moving, not
        // just on the static art, per direct feedback ("firlatirken falan
        // o isiltilari belli olacak sekilde efekt ekleyip bunu kullanalim").
        if (evolvedKnife && evolvedSprite != null)
        {
            sparkleTimer += Time.deltaTime;
            if (sparkleTimer >= SparkleInterval)
            {
                sparkleTimer = 0f;
                FxSpawner.Burst(transform.position, new Color(1f, 0.97f, 0.78f), 3, 1.1f, 0.18f, 0.06f);
            }
        }

        bool armed = distanceTraveled >= minArmDistance;
        if (armed)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, hitRadius);
            for (int i = 0; i < hits.Length; i++)
            {
                EnemyHealth enemy = hits[i].GetComponent<EnemyHealth>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                    if (AudioManager.Instance != null) AudioManager.Instance.PlayHit();
                    FxSpawner.Burst(transform.position, new Color(0.95f, 0.88f, 0.6f), 6, 2f, 0.22f, 0.10f);
                    if (CameraFollow.Instance != null) CameraFollow.Instance.Shake(0.06f, 0.05f);
                    Deactivate();
                    return;
                }
            }
        }

        if (distanceTraveled >= maxDistance || Time.time - spawnTime >= maxLifetime)
        {
            Deactivate();
        }
    }

    void ApplyRotation()
    {
        // The knife sprite's blade points "up" (+Y) in its source art, not
        // "right" (+X), so subtract 90 degrees to line the blade up with
        // the actual travel direction.
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    void Deactivate()
    {
        resolved = true;
        target = null;
        evolvedKnife = false;
        gameObject.SetActive(false);
    }
}
