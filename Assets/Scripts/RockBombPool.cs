using System.Collections.Generic;
using UnityEngine;

// Pooling for RockBomb, same pattern as ProjectilePool/EnemyProjectilePool -
// kept as its own pool since a bomb's fields (explosionRadius) don't
// belong on the knife's Projectile class.
public class RockBombPool : MonoBehaviour
{
    public static RockBombPool Instance;
    public GameObject bombPrefab;
    public int initialSize = 6;

    private List<GameObject> pool = new List<GameObject>();

    void Awake()
    {
        Instance = this;

        if (bombPrefab == null) return;

        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Instantiate(bombPrefab);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public void SpawnBomb(Vector3 position, Vector2 direction, int damage, float maxDistance)
    {
        if (bombPrefab == null) return;

        GameObject obj = GetBomb();
        obj.transform.position = position;

        RockBomb bomb = obj.GetComponent<RockBomb>();
        if (bomb != null)
        {
            bomb.direction = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
            bomb.damage = damage;
            bomb.maxDistance = maxDistance;
        }

        obj.SetActive(true);
    }

    GameObject GetBomb()
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }

        GameObject newObj = Instantiate(bombPrefab);
        pool.Add(newObj);
        return newObj;
    }
}
