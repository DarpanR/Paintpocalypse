using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Projectile : MonoBehaviour {

    [SerializeField]
    string shotName;
    public string ShotName => shotName;
    // called when this projectile should die (pool will disable & recycle)
    public Action onDestroyed;

    // Runtime State
    protected StatSet<WeaponStatType> stats;
    protected string targetTag;
    protected EntityStatType affectedType;
    protected CountdownTimer lifetime;
    protected IDamageBehavior damageBehavior;

    // internal Counters
    protected int hits;
    protected Dictionary<int, float> enemiesHit = new Dictionary<int, float>();

    /// <summary>
    /// Initialize all parameters. Call immediately after Instantiate().
    /// </summary>
    public virtual void Init(StatSet<WeaponStatType> _stats, string _targetTag, EntityStatType _affectedType, IDamageBehavior damageBehavior) {
        stats = _stats;
        affectedType = _affectedType;
        targetTag = _targetTag;

        hits = 0;
        enemiesHit.Clear();

        lifetime = new CountdownTimer(stats[WeaponStatType.Lifetime].value);
        lifetime.Start();
        lifetime.OnTimerStop += Die;

        enemiesHit.Clear();
    }

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

    protected abstract void Update();

    private void LateUpdate() {
        lifetime.Tick(Time.deltaTime);
    }

    private void OnDisable() {
        // In case someone re-enables from pool without re-init
        enemiesHit.Clear();
        hits = 0;
    }
}
