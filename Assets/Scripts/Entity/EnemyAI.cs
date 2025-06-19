using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyAI : BaseEntity {
    public Transform spriteTransform;

    Transform target;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        target = GameObject.FindWithTag("Player").transform;

        if (spriteTransform == null) spriteTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();

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

    protected override void Die() {
        base.Die();
        DropManager.Instance.TryDrop(transform.position);
    }
}
