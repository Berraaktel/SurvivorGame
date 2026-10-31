using UnityEngine;

// Gives Brute a death-trigger: splitting into weaker basic enemies
// instead of just dying like every other regular enemy. Added directly
// to Enemy_Brute.prefab only - the same single-prefab-variant pattern
// BossAttack already uses on Enemy_Boss - so Scout/Ranged/Boss are
// unaffected. Rewards focusing a Brute down with area damage (DustNova /
// Lightning / the orbit blades) instead of always chasing whichever
// single enemy happens to be nearest, since killing one late in a fight
// just creates more things to deal with, not fewer.
public class Splitter : MonoBehaviour
{
    public int splitCount = 2;
    // Index into EnemyPool.enemyPrefabs - 0 is always the base
    // Enemy/Grunt prefab (see BuildGameUITool.BuildEnemyVariety, which
    // always puts it first with unlockLevel 1), so this is guaranteed to
    // exist and be spawnable regardless of player level.
    public int splitEnemyTypeIndex = 0;
    public float splitHealthMultiplier = 0.6f;
    public float scatterRadius = 0.4f;

    private EnemyHealth health;

    void Awake()
    {
        health = GetComponent<EnemyHealth>();
    }

    void OnEnable()
    {
        if (health != null) health.OnDied += HandleDied;
    }

    void OnDisable()
    {
        if (health != null) health.OnDied -= HandleDied;
    }

    void HandleDied()
    {
        if (EnemyPool.Instance == null) return;

        for (int i = 0; i < splitCount; i++)
        {
            Vector2 offset = Random.insideUnitCircle * scatterRadius;
            Vector3 pos = transform.position + (Vector3)offset;

            GameObject child = EnemyPool.Instance.GetEnemyOfType(splitEnemyTypeIndex, pos);
            if (child == null) continue;

            EnemyHealth childHealth = child.GetComponent<EnemyHealth>();
            if (childHealth != null) childHealth.ScaleStats(splitHealthMultiplier);
        }
    }
}
