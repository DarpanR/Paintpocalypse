using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LinearShot : Projectile
{
    // 1) Move
    protected override void Update() {
        transform.position += stats[StatType.Speed].value * Time.deltaTime * transform.up;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!collision.CompareTag(targetTag))
            return;

        // Prevent double-hitting the same enemy
        int id = collision.gameObject.GetInstanceID();

        if (enemiesHit.ContainsKey(id))
            return;
        enemiesHit[id] = 0f;
        hits++;
        Debug.Log("hit");

        //Apply damage if enemy has a health component
        var enemy = collision.GetComponent<BaseEntity>();
        enemy?.TakeDamage(operation);

        // Check penetration limit
        if (hits >= penetration)
            Die();
    }
}
