using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : BaseEntity {
    public float moveSpeed = 2f;
    Transform target;

    public int damage = 10;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        target = GameObject.FindWithTag("Player").transform;    
    }

    // Update is called once per frame
    void Update() {
        if (target != null) {
            Vector2 direction = (target.position - transform.position).normalized;
            transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
        }
    }

    protected void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null) {
                player.TakeDamage(damage);
            }
        }
    }
}
