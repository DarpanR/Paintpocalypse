using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Projectile : MonoBehaviour {
    // called when this projectile should die (pool will disable & recycle)
    public Action onDestroyed;

    // Runtime State
    protected Vector2 velocity;
    protected IoperationStrategy operation;
    protected float lifetime;
    protected int penetration;
    protected string target;
    protected float fireRate;

    // internal Counters
    protected int hits;
    protected Dictionary<int, float> enemiesHit = new Dictionary<int, float>();

    /// <summary>
    /// Initialize all parameters. Call immediately after Instantiate().
    /// </summary>
    public virtual void Init(Vector2 _velocity, IoperationStrategy _operation, float _lifetime, int _penetration, float _fireRate, string _target) {
        velocity = _velocity;
        operation = _operation;
        lifetime = _lifetime;
        penetration = _penetration;
        target = _target;
        fireRate = _fireRate;

        hits = 0;
        enemiesHit.Clear();
    }

    protected abstract void Update();

    protected void Die() {
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

    private void LateUpdate() {
        lifetime -= Time.deltaTime;

        if (lifetime <= 0)
            Die();
    }


    private void OnDisable() {
        // In case someone re-enables from pool without re-init
        enemiesHit.Clear();
        hits = 0;
    }
}
