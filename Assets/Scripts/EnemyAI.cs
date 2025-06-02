using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 2f;
    Transform target;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindWithTag("Player").transform;    
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null) { 
            Vector2 direction = (target.position - transform.position).normalized;
            transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
        } 
    }
}
