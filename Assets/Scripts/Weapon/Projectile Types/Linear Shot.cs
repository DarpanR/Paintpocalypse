using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LinearShot : Projectile
{
    // 1) Move
    protected override void Update() {
        transform.position += (Vector3)velocity * Time.deltaTime;
    }

    // 2) Age & expire
    private void LateUpdate() {
        lifetime -= Time.deltaTime;

        if (lifetime <= 0)
            Die();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!collision.CompareTag(target))
            return;

        // Prevent double-hitting the same enemy
        int id = collision.gameObject.GetInstanceID();

        if (enemiesHit.Contains(id))
            return;
        enemiesHit.Add(id);
        hits++;

        //Apply damage if enemy has a health component
        var enemy = collision.GetComponent<BaseEntity>();
        enemy?.TakeDamage(damage);

        // Check penetration limit
        if (hits >= penetration)
            Die();
    }
}
