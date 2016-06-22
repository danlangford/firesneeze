using System;
using System.Collections.Generic;
using UnityEngine;

public class Cache : MonoBehaviour
{
    private static readonly int MAX_POOL_COPIES = 10;
    private static Dictionary<string, List<GameObject>> pool = new Dictionary<string, List<GameObject>>();
    private static Dictionary<string, GameObject> table = new Dictionary<string, GameObject>();

    private void Awake()
    {
        UnityEngine.Object.DontDestroyOnLoad(this);
    }

    public void Checkin(string path, GameObject go)
    {
        if (!pool.ContainsKey(path))
        {
            List<GameObject> list = new List<GameObject>(MAX_POOL_COPIES);
            pool.Add(path, list);
        }
        if (pool.ContainsKey(path) && (pool[path].Count <= MAX_POOL_COPIES))
        {
            go.transform.parent = base.transform;
            go.SetActive(false);
            pool[path].Add(go);
        }
        else
        {
            UnityEngine.Object.Destroy(go);
        }
    }

    public GameObject Checkout(string path)
    {
        if (pool.ContainsKey(path))
        {
            List<GameObject> list = pool[path];
            if (list.Count > 0)
            {
                GameObject obj2 = list[0];
                list.RemoveAt(0);
                if (obj2 != null)
                {
                    obj2.transform.parent = null;
                    obj2.SetActive(true);
                    return obj2;
                }
            }
        }
        GameObject prefab = Resources.Load<GameObject>(path);
        if (prefab == null)
        {
            return null;
        }
        GameObject obj4 = this.Create(prefab);
        if (obj4 != null)
        {
            obj4.transform.parent = null;
            obj4.SetActive(true);
        }
        return obj4;
    }

    public void Clear()
    {
        table.Clear();
        pool.Clear();
        Geometry.DestroyAllChildren(base.gameObject);
    }

    private GameObject Create(UnityEngine.Object prefab)
    {
        GameObject obj2 = UnityEngine.Object.Instantiate(prefab) as GameObject;
        if (obj2 != null)
        {
            obj2.name = prefab.name;
            obj2.transform.parent = base.transform;
            obj2.transform.position = Vector3.zero;
            obj2.SetActive(false);
        }
        return obj2;
    }

    public T Get<T>(string path) where T: Component
    {
        if (!table.ContainsKey(path))
        {
            GameObject prefab = Resources.Load<GameObject>(path);
            if (prefab != null)
            {
                GameObject obj3 = this.Create(prefab);
                if (obj3 != null)
                {
                    table.Add(path, obj3);
                }
            }
        }
        GameObject obj4 = null;
        if (table.ContainsKey(path))
        {
            obj4 = table[path];
        }
        if (obj4 != null)
        {
            return obj4.GetComponent<T>();
        }
        return null;
    }
}

