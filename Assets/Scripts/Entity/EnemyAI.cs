using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyAI : BaseEntity, IAttractable {

    Transform player;
    Transform overrideTarget = null;
    CountdownTimer hitTimer;
    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
        player = GameObject.FindWithTag("Player").transform;
    }

    public override void Init(EntityData entityData, string guid) {
        base.Init(entityData, guid);
        hitTimer = new CountdownTimer(CurrentStats.GetValueOrDefault(EntityStatType.InvincibilityDuration, 0.05f));
    }

    // Update is called once per frame
    void Update() {
        if (hitTimer.IsRunning) {
            hitTimer.Tick(Time.deltaTime);
            return;
        }
        var target = overrideTarget != null ? overrideTarget : player;

        if (target != null) {
            Vector2 direction = (target.position - transform.position).normalized;
            transform.position += (Vector3)direction * CurrentStats[EntityStatType.Speed].value * Time.deltaTime;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    protected override void LateUpdate() {
        if (hitTimer.IsRunning) return;
        base.LateUpdate();
    }

    public override void TakeDamage(IoperationStrategy<EntityStatType> operation) {
        base.TakeDamage(operation);
        hitTimer.Reset();
    }

    protected override void OnStatUpdated() {
        var newScale = CurrentStats.GetStatOrNull(EntityStatType.LocalScale).value;

        if (!Mathf.Approximately(newScale, transform.localScale.x))
            transform.localScale = new Vector3(newScale, newScale);
        base.OnStatUpdated();
    }

    public void SetOverrideTarget(Transform overrideTarget) {
        this.overrideTarget = overrideTarget;
    }

    public void OnAttractionEnd() {
        overrideTarget = null;
    }

    protected override void Die() {
        GameEvents.RaiseEntityDeath(GUID, transform.position);
        //DropManager.Instance.TryDrop(transform.position);
        base.Die();
    }
}
