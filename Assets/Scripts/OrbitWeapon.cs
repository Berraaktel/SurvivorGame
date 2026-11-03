using System.Collections.Generic;
using UnityEngine;

// Second weapon type: a small ring of blades that continuously spins
// around the player, independent from the thrown-knife auto-targeting
// weapon (PlayerAttack). No aiming needed - it just punishes anything
// that gets close, which rewards fighting defensively instead of only
// kiting and relying on the ranged throw.
public class OrbitWeapon : MonoBehaviour
{
    public GameObject orbiterPrefab;
    public int orbiterCount = 2;
    public float radius = 1.3f;
    public float rotationSpeed = 140f; // degrees per second, around the player
    public int damage = 1;

    // Evolution: max level + Miknatis (Magnet) turns this into "Olum
    // Cemberi" - two extra blades plus a bigger, faster ring. Same
    // level/evolve shape as every other weapon here, see PlayerAttack for
    // the first one with the full explanation.
    public int level = 1;
    public int maxLevel = 5;
    public bool evolved = false;

    private List<Transform> orbiters = new List<Transform>();
    private float angle;

    void Start()
    {
        if (orbiterPrefab == null) return;

        for (int i = 0; i < orbiterCount; i++)
        {
            GameObject obj = Instantiate(orbiterPrefab);
            OrbitBlade blade = obj.GetComponent<OrbitBlade>();
            if (blade != null) blade.damage = damage;
            orbiters.Add(obj.transform);
        }
    }

    // Re-propagates a new damage value to every already-spawned orbiter.
    // OrbitWeapon.damage alone only reaches a blade at spawn time (see
    // Start()), so without this, the "Saldiri Hasari" level-up upgrade
    // would silently stop affecting this weapon the moment the ring is
    // first spawned - it would keep buffing the thrown knife only.
    public void SetDamage(int newDamage)
    {
        damage = newDamage;
        foreach (Transform t in orbiters)
        {
            if (t == null) continue;
            OrbitBlade blade = t.GetComponent<OrbitBlade>();
            if (blade != null) blade.damage = damage;
        }
    }

    public bool CanLevelUp()
    {
        return enabled && !evolved && level < maxLevel;
    }

    // Leveling this weapon used to only speed it up, which barely read as
    // a change at all - now every level also adds one more blade to the
    // ring (2 blades at level 1 -> 4 by level 3, matching how the base
    // game's own weapons visibly grow), plus a little extra room (radius)
    // so the growing ring does not start overlapping itself.
    public void LevelUp()
    {
        level++;
        SetDamage(damage + 1);
        rotationSpeed += 15f;
        radius += 0.1f;
        AddBlade();
    }

    void AddBlade()
    {
        if (orbiterPrefab == null) return;

        GameObject obj = Instantiate(orbiterPrefab);
        OrbitBlade blade = obj.GetComponent<OrbitBlade>();
        if (blade != null)
        {
            blade.damage = damage;
            blade.evolved = evolved;
        }
        orbiters.Add(obj.transform);
        orbiterCount = orbiters.Count;
    }

    void Update()
    {
        if (orbiters.Count == 0) return;

        angle += rotationSpeed * Time.deltaTime;
        float step = 360f / orbiters.Count;

        for (int i = 0; i < orbiters.Count; i++)
        {
            float a = (angle + step * i) * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0f) * radius;
            orbiters[i].position = transform.position + offset;
            // Extra self-spin on top of the orbit itself, so each blade
            // visibly whirls rather than just gliding around face-first.
            orbiters[i].Rotate(0f, 0f, rotationSpeed * 4f * Time.deltaTime);
        }

        CheckEvolution();
    }

    void CheckEvolution()
    {
        if (evolved || level < maxLevel) return;

        PlayerUpgrades pu = GetComponent<PlayerUpgrades>();
        if (pu == null || !pu.HasMagnet()) return;

        Evolve();
    }

    void Evolve()
    {
        evolved = true;
        radius += 0.3f;
        SetDamage(damage + 1);

        AddBlade();
        AddBlade();

        foreach (Transform t in orbiters)
        {
            if (t == null) continue;
            OrbitBlade blade = t.GetComponent<OrbitBlade>();
            if (blade != null) blade.evolved = true;
        }

        if (EvolutionBanner.Instance != null) EvolutionBanner.Instance.Show(Localization.Get("evo_orbit"));
        FxSpawner.Burst(transform.position, new Color(1f, 0.85f, 0.35f), 32, 3.2f, 0.65f, 0.22f);
        if (CameraFollow.Instance != null) CameraFollow.Instance.Shake(0.28f, 0.2f);
        if (AudioManager.Instance != null) AudioManager.Instance.PlayLevelUp();
    }

    void OnDestroy()
    {
        // The orbiters are free-floating GameObjects, not children, so they
        // survive their owner's destruction unless cleaned up explicitly -
        // matters on scene reload where a fresh OrbitWeapon.Start() would
        // otherwise stack a new ring on top of the old one.
        foreach (Transform t in orbiters)
        {
            if (t != null) Destroy(t.gameObject);
        }
    }
}
