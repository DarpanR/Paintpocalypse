using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyAI : BaseEntity {
    [Header("Enemy Logic")]
    public Transform spriteTransform;

    Transform player;
    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
        player = GameObject.FindWithTag("Player").transform;

        if (spriteTransform == null) spriteTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update() {
        if (player != null) {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position += (Vector3)direction * CurrentStats[StatType.Speed].value * Time.deltaTime;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    protected override void LateUpdate() {
        base.LateUpdate();
        // Reset the visual child to stay upright
        spriteTransform.rotation = quaternion.identity;
    }

    protected override void Die() {
        DropManager.Instance.TryDrop(transform.position);
        base.Die();
    }
}
