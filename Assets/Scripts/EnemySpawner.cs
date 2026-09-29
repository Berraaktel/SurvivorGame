using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public float spawnInterval = 2f;
    public float minSpawnInterval = 0.5f;
    // How many seconds of survival it takes for the spawn rate to ramp from
    // spawnInterval down to minSpawnInterval. Without this the game never
    // gets harder over time, which is the core pacing loop this genre needs.
    public float rampDuration = 180f;
    public float spawnRadius = 8f;

    private float timer;
    private float elapsed;
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
        elapsed += Time.deltaTime;
        timer += Time.deltaTime;

        if (timer >= GetCurrentSpawnInterval())
        {
            timer = 0f;
            SpawnEnemy();
        }
    }

    float GetCurrentSpawnInterval()
    {
        float t = Mathf.Clamp01(elapsed / rampDuration);
        return Mathf.Lerp(spawnInterval, minSpawnInterval, t);
    }

    void SpawnEnemy()
    {
        if (player == null) return;

        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = player.position + (Vector3)(randomDirection * spawnRadius);

        EnemyPool.Instance.GetEnemy(spawnPos);
    }
}
