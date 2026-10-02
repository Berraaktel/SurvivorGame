using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public int xpValue = 1;
    public int goldValue = 1;
    // Marks this variant as the Boss species so a single, screen-level
    // UI (BossHealthBarUI) can track it without every regular enemy
    // paying for a health-change event nobody is listening to.
    public bool isBoss = false;

    // The currently-alive boss, if any. Set/cleared in OnEnable/OnDisable
    // rather than searched for every frame by the UI - there is at most
    // one boss alive at a time by design (BossSpawner), so a static
    // pointer is simpler and cheaper than a scene-wide find.
    public static EnemyHealth ActiveBoss;

    public event System.Action<int, int> OnHealthChanged;
    public int CurrentHealth { get { return currentHealth; } }

    private int currentHealth;
    private SpriteRenderer sr;
    private Color originalColor;
    private Coroutine flashRoutine;

    void Awake()
    {
        // The sprite/animation may live on a separate child "SpriteVisual"
        // object instead of this root (kept separate so cosmetic animation
        // never disturbs the Rigidbody2D/Collider2D on the root) - fall
        // back to this object's own SpriteRenderer if there's no such
        // child, so this keeps working on enemies that don't use it.
        Transform visual = transform.Find("SpriteVisual");
        sr = visual != null ? visual.GetComponent<SpriteRenderer>() : GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    void OnEnable()
    {
        currentHealth = maxHealth;

        // A pooled enemy can be reactivated mid-flash: SetActive(false) kills
        // the FlashRed coroutine below immediately, before it gets a chance
        // to restore the original color. That's why a recycled enemy could
        // pop back up stuck red. Always start a fresh spawn with a clean
        // color and no leftover coroutine reference.
        flashRoutine = null;
        if (sr != null) sr.color = originalColor;

        if (isBoss) ActiveBoss = this;
        if (OnHealthChanged != null) OnHealthChanged(currentHealth, maxHealth);
    }

    void OnDisable()
    {
        if (isBoss && ActiveBoss == this) ActiveBoss = null;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (OnHealthChanged != null) OnHealthChanged(currentHealth, maxHealth);

        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            if (XPOrbPool.Instance != null)
            {
                XPOrbPool.Instance.SpawnOrb(transform.position, xpValue);
            }
            if (GoldOrbPool.Instance != null && goldValue > 0)
            {
                GoldOrbPool.Instance.SpawnOrb(transform.position, goldValue);
            }
            if (AudioManager.Instance != null) AudioManager.Instance.PlayEnemyDeath();
            FxSpawner.Burst(transform.position, new Color(0.75f, 0.15f, 0.12f), 12, 3f, 0.4f, 0.14f);
            if (CameraFollow.Instance != null) CameraFollow.Instance.Shake(0.10f, 0.08f);
            gameObject.SetActive(false);
        }
    }

    IEnumerator FlashRed()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        if (gameObject.activeInHierarchy)
        {
            sr.color = originalColor;
        }
        flashRoutine = null;
    }
}
