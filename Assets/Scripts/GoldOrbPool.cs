using System.Collections.Generic;
using UnityEngine;

// Pooled Gold orbs, mirrors XPOrbPool exactly.
public class GoldOrbPool : MonoBehaviour
{
    public static GoldOrbPool Instance;
    public GameObject orbPrefab;
    public int initialSize = 20;

    private List<GameObject> pool = new List<GameObject>();

    void Awake()
    {
        Instance = this;

        if (orbPrefab == null) return;

        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Instantiate(orbPrefab);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public void SpawnOrb(Vector3 position, int goldValue)
    {
        if (orbPrefab == null) return;

        GameObject obj = GetOrb();
        obj.transform.position = position;

        GoldOrb orbComp = obj.GetComponent<GoldOrb>();
        if (orbComp != null)
        {
            orbComp.goldValue = goldValue;
        }

        obj.SetActive(true);
    }

    GameObject GetOrb()
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }

        GameObject newObj = Instantiate(orbPrefab);
        pool.Add(newObj);
        return newObj;
    }
}
