using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEShot : Projectile
{
    public float minRadius = 0f;
    public float maxRadius = 3f;
    public float expansionRate = 0.5f;

    float timer = 0f;
    float currentRadius = 0f;

    public override void Init(Vector2 velocity, int damage, float lifetime, int penetration, float fireRate, string target) {
        base.Init(velocity, damage, lifetime, penetration, fireRate, target);
        
        transform.localScale = Vector3.one * minRadius;
    }

    protected override void Update() {
        timer += Time.deltaTime;
        currentRadius = Mathf.Min(currentRadius + expansionRate * Time.deltaTime, maxRadius);
        transform.localScale = Vector3.one * currentRadius * 2f;

        if (timer >= fireRate) {
            timer -= fireRate;
            DoDamage();
        }
    }

    void DoDamage() {
        var hits = Physics2D.OverlapCircleAll(transform.position, currentRadius);

        foreach (var hit in hits) {
            if (!hit.CompareTag(target)) {
                var enemy = hit.GetComponent<EnemyAI>();

                if (enemy != null)
                    enemy.TakeDamage(damage);
            }
        }
    }
}
