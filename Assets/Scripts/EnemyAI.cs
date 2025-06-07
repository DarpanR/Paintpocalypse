using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : BaseEntity {
    public float moveSpeed = 2f;
    public float damage = 2f;

    Transform target;
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

    protected virtual void DoDamage(float amount) {
        throw new System.NotImplementedException();
    }

    protected void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            PlayerController player = collision.GetComponent<PlayerController>();

            if (player != null) {
                player.TakeDamage(damage);
            }
        }
    }
}
