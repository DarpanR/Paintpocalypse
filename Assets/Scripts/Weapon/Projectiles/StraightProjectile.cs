using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightProjectile : Projectile
{
    Vector3 velocity;

    public override void Init(StatSet _stats, string _targetTag, IDamageBehavior _damageBehavior) {
        base.Init(_stats, _targetTag, _damageBehavior);
        velocity = transform.up * _stats[StatType.Speed].value;
    }

    protected override void Update() {
        transform.position += velocity * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag(targetTag)) {
            damageBehavior?.ApplyDamage(other.gameObject, stats, targetTag);
            hits++;

            if (hits >= stats[StatType.Penetration].value)
                Die();
        }
    }
}
