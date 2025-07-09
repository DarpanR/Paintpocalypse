using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightProjectile : Projectile
{
    Vector3 velocity;
    IDamageBehavior damageBehavior;

    public void Init(StatSet _stats, string _targetTag, IDamageBehavior _damageBehavior) {
        base.Init(_stats, _targetTag);
        velocity = transform.up * _stats[StatType.Speed].value;
        damageBehavior = _damageBehavior;
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
