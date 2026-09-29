using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance;
    public GameObject[] enemyPrefabs;
    public int[] unlockLevels;
    public int initialSizePerType = 8;

    private List<GameObject>[] pools;
    private List<int> eligibleBuffer = new List<int>();

    void Awake()
    {
        Instance = this;

        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            pools = new List<GameObject>[0];
            return;
        }

        pools = new List<GameObject>[enemyPrefabs.Length];
        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            pools[i] = new List<GameObject>();
            if (enemyPrefabs[i] == null) continue;

            for (int j = 0; j < initialSizePerType; j++)
            {
                GameObject obj = Instantiate(enemyPrefabs[i]);
                obj.SetActive(false);
                pools[i].Add(obj);
            }
        }
    }

    public GameObject GetEnemy(Vector3 position)
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return null;

        int playerLevel = GetPlayerLevel();

        eligibleBuffer.Clear();
        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            int req = (unlockLevels != null && i < unlockLevels.Length) ? unlockLevels[i] : 1;
            if (playerLevel >= req)
            {
                eligibleBuffer.Add(i);
            }
        }

        if (eligibleBuffer.Count == 0)
        {
            eligibleBuffer.Add(0);
        }

        int typeIndex = eligibleBuffer[Random.Range(0, eligibleBuffer.Count)];
        return GetEnemyOfType(typeIndex, position);
    }

    int GetPlayerLevel()
    {
        PlayerXP xp = Object.FindFirstObjectByType<PlayerXP>();
        return xp != null ? xp.level : 1;
    }

    public GameObject GetEnemyOfType(int typeIndex, Vector3 position)
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return null;
        typeIndex = Mathf.Clamp(typeIndex, 0, enemyPrefabs.Length - 1);
        if (enemyPrefabs[typeIndex] == null) return null;

        List<GameObject> pool = pools[typeIndex];
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.transform.position = position;
                obj.SetActive(true);
                return obj;
            }
        }

        GameObject newObj = Instantiate(enemyPrefabs[typeIndex], position, Quaternion.identity);
        pool.Add(newObj);
        return newObj;
    }
}
