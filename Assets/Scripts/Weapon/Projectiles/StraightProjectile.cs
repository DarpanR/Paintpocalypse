using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightProjectile : Projectile
{
    Vector3 velocity;

    public override void Init(StatSet<WeaponStatType> _stats, string _targetTag, EntityStatType _affectedType, IDamageBehavior _damageBehavior) {
        base.Init(_stats, _targetTag, _affectedType, _damageBehavior);
        velocity = transform.up * _stats[WeaponStatType.Speed].value;
    }

    protected override void Update() {
        transform.position += velocity * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag(targetTag)) {
            damageBehavior?.ApplyDamage(other.gameObject, targetTag, stats, affectedType);
            hits++;

            if (hits >= stats[WeaponStatType.Penetration].value)
                Die();
        }
    }
}
