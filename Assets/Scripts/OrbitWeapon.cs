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
