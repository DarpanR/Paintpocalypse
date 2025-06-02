using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnRate = 2f;
    public float spawnRange = 10f;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnEnemy", 2f, spawnRate);    
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 SpawnPos = (Vector2)transform.position + Random.insideUnitCircle.normalized * spawnRange;
        Instantiate(enemyPrefab,SpawnPos,Quaternion.identity);
    }
}
