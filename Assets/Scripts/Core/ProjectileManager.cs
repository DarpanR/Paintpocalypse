using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    public class ProjectilePool {
        readonly GameObject projPrefab;
        Transform transform;
        //public int initialSize = 10;

        Queue<GameObject> pool = new Queue<GameObject>();

        public ProjectilePool (GameObject prefab, Transform parent, int initialSize) {
            projPrefab = prefab;
            transform = parent;

            for (int i = 0; i < initialSize; i++)
                pool.Enqueue(CreateInstance());
        }

        GameObject CreateInstance() {
            var proj = Instantiate(projPrefab, transform);
            proj.gameObject.SetActive(false);

            return proj;
        }

        public GameObject Get() {
            GameObject proj = pool.Count > 0 ? pool.Dequeue() : CreateInstance();
            proj.gameObject.SetActive(true);

            return proj;
        }

        public void Return(GameObject proj) {
            proj.gameObject.SetActive(false);
            pool.Enqueue(proj);
        }

    }

    public static ProjectileManager Instance;

    Dictionary<string, ProjectilePool> pools = new();

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    public GameObject Request (GameObject prefab, int initialSize = 5) {
        Projectile proj = prefab.GetComponent<Projectile>();

        if (proj == null)
            return null;
        string key = proj.ShotName;

        if (!pools.TryGetValue(key, out var pool)) {
            pool = new ProjectilePool(prefab, transform, initialSize);
            pools.Add(key, pool);
        }
        return pool.Get();
    }

    public void Return (GameObject projectile) {
        Projectile proj = projectile.GetComponent<Projectile>();

        if (proj == null) return;
        
        string key = proj.ShotName;

        if (!pools.TryGetValue(key, out var pool)) {
            Destroy(projectile.gameObject);
            Debug.LogWarning("No pool found for prefab: " + key + ". Destroying.");
            return;
        }
        pool.Return(projectile);
    }
}
