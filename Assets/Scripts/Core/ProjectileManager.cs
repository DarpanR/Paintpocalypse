using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    public static ProjectileManager Instance;

    public Transform projectileFolder;

    public float trimInterval = 30f;
    public float maxIdleLifetime = 60f;

    Dictionary<string, ProjectilePool> pools = new();
    CountdownTimer trimTimer;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);

        if (projectileFolder == null) {
            projectileFolder = new GameObject("Projectiles").transform;
            projectileFolder.SetParent(transform);
        }
    }

    private void Start() {
        trimTimer = new CountdownTimer(trimInterval);
        trimTimer.OnTimerStop += () => {
            foreach (var pool in pools.Values)
                pool.Trim(maxIdleLifetime);
        };
        trimTimer.OnTimerStop += trimTimer.Reset;
        trimTimer.Start();
    }

    private void Update() {
        trimTimer.Tick(Time.deltaTime);
    }

    public GameObject Request(GameObject prefab, int initialSize = 5) {
        if (!prefab.TryGetComponent<Projectile>(out var proj))
            return null;
        string key = proj.ShotName;

        if (!pools.TryGetValue(key, out var pool)) {
            pool = new ProjectilePool(prefab, projectileFolder, initialSize);
            pools.Add(key, pool);
        }
        return pool.Get();
    }

    public void Return(GameObject projectile) {
        if (!projectile.TryGetComponent<Projectile>(out var proj)) return;

        string key = proj.ShotName;

        if (!pools.TryGetValue(key, out var pool)) {
            Destroy(projectile);
            Debug.LogWarning("No pool found for prefab: " + key + ". Destroying.");
            return;
        }
        pool.Return(projectile);
    }

    class ProjectilePool {
        readonly GameObject projPrefab;
        Transform transform;
        //public int initialSize = 10;

        Queue<(GameObject, float)> pool = new();

        public ProjectilePool (GameObject prefab, Transform parent, int initialSize) {
            projPrefab = prefab;
            transform = parent;

            for (int i = 0; i < initialSize; i++)
                pool.Enqueue((CreateInstance(), Time.time));
        }

        GameObject CreateInstance() {
            var proj = Instantiate(projPrefab, transform);
            proj.SetActive(false);

            return proj;
        }

        public GameObject Get() {
            GameObject proj = pool.Count > 0 ? pool.Dequeue().Item1 : CreateInstance();
            proj.SetActive(true);

            return proj;
        }

        public void Return(GameObject proj) {
            proj.SetActive(false);
            pool.Enqueue((proj, Time.time));
        }

        public void Trim(float maxIdleSeconds, int minPoolSize = 0) {
            int count = pool.Count;
            Queue<(GameObject, float)> newQueue = new();

            while(pool.Count > 0) {
                var (proj, returnedTime) = pool.Dequeue();

                if(Time.time - returnedTime > maxIdleSeconds && count > minPoolSize) {
                    Destroy(proj);
                    count--;
                } else 
                    newQueue.Enqueue((proj, returnedTime));
            }
            pool = newQueue;
        }
    }
}
