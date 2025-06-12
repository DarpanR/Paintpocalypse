using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    // called when this projectile should die (pool will disable & recycle)
    public Action onDestroyed;

    // Runtime State
    protected Vector2 velocity;
    protected int damage;
    protected float lifetime;
    protected int penetration;
    protected string target;

    // internal Counters
    int hits;
    HashSet<int> enemiesHit = new HashSet<int>();

    Collider2D col;

    private void Awake() {
        col = GetComponent<Collider2D>();
    }

    /// <summary>
    /// Initialize all parameters. Call immediately after Instantiate().
    /// </summary>
    public virtual void Init(Vector2 _velocity, int _damage, float _lifetime, int _penetration,string _target) {
        velocity = _velocity;
        damage = _damage;
        lifetime = _lifetime;
        penetration = _penetration;
        target = _target;

        hits = 0;
        enemiesHit.Clear();
    }

    // 1) Move
    protected virtual void Update() {
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

    private void Die() {
        // Fire the pool callback if set
        if (onDestroyed != null) {
            var callBack = onDestroyed;
            onDestroyed = null;
            callBack();
        } 
        /// BUGFIX: At higher speed a race condition makes it so that ondestoryed is 
        /// null at the time of another projectile's death so instead of requeing the porjectile
        /// it gets deleted
        //else
        //    // no pool -> destory normally
        //    Destroy(gameObject);
    }

    private void OnDisable() {
        // In case someone re-enables from pool without re-init
        enemiesHit.Clear();
        hits = 0;
    }
}
