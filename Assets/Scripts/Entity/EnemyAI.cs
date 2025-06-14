using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyAI : BaseEntity {
    [Header("Enemy Logic")]
    public Transform spriteTransform;
    public float moveSpeed = 2f;
    public float damage = 2f;

    Transform target;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        target = GameObject.FindWithTag("Player").transform;

        if (spriteTransform == null) spriteTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update() {
        if (target != null) {
            Vector2 direction = (target.position - transform.position).normalized;
            transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
          
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
    private void LateUpdate() {
        // Reset the visual child to stay upright
        spriteTransform.rotation = quaternion.identity;
    }

    protected virtual void DoDamage(float amount) {
        throw new System.NotImplementedException();
    }

    protected void OnTriggerStay2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            PlayerController player = collision.GetComponent<PlayerController>();

            if (player != null) {
                player.TakeDamage(damage);
            }
        }
    }
}
