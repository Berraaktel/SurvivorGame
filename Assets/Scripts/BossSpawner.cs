using UnityEngine;

// Spawns exactly one Boss-type enemy on its own schedule, independent from
// EnemySpawner's ambient random spawning. The boss's type index sits
// outside the normal level-gated rotation (see BuildEnemyVariety /
// EnemyPool.unlockLevels), so it only ever enters play through this script
// calling EnemyPool.GetEnemyOfType directly - never through the random
// EnemyPool.GetEnemy(position) draw the ambient spawner uses.
public class BossSpawner : MonoBehaviour
{
    public int bossTypeIndex = -1;
    public float firstSpawnDelay = 45f;
    public float repeatInterval = 75f;
    public float spawnRadius = 9f;

    private float timer;
    private bool firstSpawnDone;
    private Transform player;

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (bossTypeIndex < 0 || player == null) return;

        timer += Time.deltaTime;
        float threshold = firstSpawnDone ? repeatInterval : firstSpawnDelay;

        if (timer >= threshold)
        {
            timer = 0f;
            firstSpawnDone = true;
            SpawnBoss();
        }
    }

    void SpawnBoss()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = player.position + (Vector3)(randomDirection * spawnRadius);
        EnemyPool.Instance.GetEnemyOfType(bossTypeIndex, spawnPos);
        Debug.Log("Boss spawned at " + spawnPos);
    }
}
