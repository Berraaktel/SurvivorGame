using System.Collections.Generic;
using UnityEngine;

// Separate pool from the player's ProjectilePool - keeps enemy-fired shots
// (which only ever need a straight direction + damage) from sharing a
// prefab/pool with the player's homing knife, which carries fields (target,
// maxDistance) that don't apply here.
public class EnemyProjectilePool : MonoBehaviour
{
    public static EnemyProjectilePool Instance;
    public GameObject projectilePrefab;
    public int initialSize = 12;

    private List<GameObject> pool = new List<GameObject>();

    void Awake()
    {
        Instance = this;

        if (projectilePrefab == null) return;

        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Instantiate(projectilePrefab);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public void SpawnProjectile(Vector3 position, Vector2 direction, int damage)
    {
        if (projectilePrefab == null) return;

        GameObject obj = GetProjectile();
        obj.transform.position = position;

        EnemyProjectile proj = obj.GetComponent<EnemyProjectile>();
        if (proj != null)
        {
            proj.direction = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
            proj.damage = damage;
        }

        obj.SetActive(true);
    }

    GameObject GetProjectile()
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }

        GameObject newObj = Instantiate(projectilePrefab);
        pool.Add(newObj);
        return newObj;
    }
}
