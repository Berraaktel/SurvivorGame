using System.Collections.Generic;
using UnityEngine;

public class XPOrbPool : MonoBehaviour
{
    public static XPOrbPool Instance;
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

    public void SpawnOrb(Vector3 position, int xpValue)
    {
        if (orbPrefab == null) return;

        GameObject obj = GetOrb();
        obj.transform.position = position;

        XPOrb orbComp = obj.GetComponent<XPOrb>();
        if (orbComp != null)
        {
            orbComp.xpValue = xpValue;
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
