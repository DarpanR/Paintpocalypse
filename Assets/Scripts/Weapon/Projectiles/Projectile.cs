using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Projectile : MonoBehaviour {
    // called when this projectile should die (pool will disable & recycle)
    public Action onDestroyed;

    // Runtime State
    protected StatSet stats;
    protected IoperationStrategy operation;
    protected int penetration;
    protected string targetTag;
    protected CountdownTimer lifetime;

    // internal Counters
    protected int hits;
    protected Dictionary<int, float> enemiesHit = new Dictionary<int, float>();

    /// <summary>
    /// Initialize all parameters. Call immediately after Instantiate().
    /// </summary>
    public virtual void Init(StatSet _stats, string _targetTag, IoperationStrategy _operation, int _penetration) {
        stats = _stats;
        operation = _operation;
        penetration = _penetration;
        targetTag = _targetTag;
        hits = 0;

        lifetime = new CountdownTimer(stats[StatType.Lifetime].value);
        lifetime.Start();
        lifetime.OnTimerStop += Die;

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
        /// BUGFIX: At higher velocity a race condition makes it so that ondestoryed is 
        /// null at the time of another projectile's death so instead of requeing the porjectile
        /// it gets deleted
        //else
        //    // no pool -> destory normally
        //    Destroy(gameObject);
    }

    private void LateUpdate() {
        lifetime.Tick(Time.deltaTime);
    }


    private void OnDisable() {
        // In case someone re-enables from pool without re-init
        enemiesHit.Clear();
        hits = 0;
    }
}
