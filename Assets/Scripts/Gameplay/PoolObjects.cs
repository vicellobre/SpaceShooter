using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Object pool of french fries
/// </summary>
public static class PoolObjects {
    static GameObject prefabUFO;
    static List<GameObject> poolUFOs;
    static GameObject prefabBullet;
    static List<GameObject> poolBullets;

    public static GameObject PrefabUFO { get { return prefabUFO; } }
    public static GameObject PrefabBullet { get { return prefabBullet; } }
    public static List<GameObject> PoolUFOs { get { return poolUFOs; } }
    public static List<GameObject> PoolBullets { get { return poolBullets; } }

    /// <summary>
    /// Initializes the pool
    /// </summary>
    ///
    public static void InitializeUFOPool() {
        // create and fill pool
        prefabUFO = Resources.Load<GameObject>("UFO");
        poolUFOs = new List<GameObject>(20);
        for (int i = 0; i < poolUFOs.Capacity; i++) {
            poolUFOs.Add(GetNew_Object(prefabUFO));
        }
    }

    public static void InitializeBulletPool() {
        // create and fill pool
        prefabBullet = Resources.Load<GameObject>("Bullet");
        poolBullets = new List<GameObject>(50);
        for (int i = 0; i < poolBullets.Capacity; i++) {
            poolBullets.Add(GetNew_Object(prefabBullet));
        }
    }

    /// <summary>
    /// Gets a french fries object from the pool
    /// </summary>
    /// <returns>french fries</returns>
    public static GameObject Get_Object(List<GameObject> pool, GameObject prefab) {
        // check for available object in pool
        if (pool.Count > 0) {
            GameObject ufo = pool[pool.Count - 1];
            pool.RemoveAt(pool.Count - 1);
            return ufo;
        } else {
            // pool empty, so expand pool and return new object
            pool.Capacity++;
            return GetNew_Object(prefab);
        }
    }

    /// <summary>
    /// Returns a french fries object to the pool
    /// </summary>
    /// <param name="frenchFries">french fries</param>
    public static void Return_Object(List<GameObject> pool, GameObject prefab) {
        
        if (prefab.CompareTag(TagName.Bullet.ToString())) {
            prefab.GetComponent<Bullet>().StopMoving();
        } else if (prefab.CompareTag(TagName.UFO.ToString())) {
            prefab.GetComponent<UFO>().StopMoving();
        }
        prefab.SetActive(false);

        if(!pool.Contains(prefab))
            pool.Add(prefab);
    }

    /// <summary>
    /// Gets a new french fries object
    /// </summary>
    /// <returns>french fries</returns>
    static GameObject GetNew_Object(GameObject prefab) {
        GameObject go = GameObject.Instantiate(prefab);
        if (go.CompareTag(TagName.Bullet.ToString())) {
            go.GetComponent<Bullet>().Initialize();
        } else if (go.CompareTag(TagName.UFO.ToString())) {
            go.GetComponent<UFO>().Initialize();
        }
        go.SetActive(false);
        GameObject.DontDestroyOnLoad(go);
        return go;
    }

    public static void ReturnPoolObjects(List<GameObject> pool, string tag) {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag(tag))
        {
            Return_Object(pool, go);
        }
        
    }
}