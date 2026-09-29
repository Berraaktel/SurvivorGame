using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    public static ProjectilePool Instance;
    public GameObject projectilePrefab;
    public int initialSize = 16;

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

    public void SpawnProjectile(Vector3 position, Vector2 direction, int damage, float maxDistance, Transform target)
    {
        if (projectilePrefab == null) return;

        GameObject obj = GetProjectile();
        obj.transform.position = position;

        Projectile proj = obj.GetComponent<Projectile>();
        if (proj != null)
        {
            proj.direction = direction.normalized;
            proj.damage = damage;
            proj.maxDistance = maxDistance;
            proj.target = target;
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
