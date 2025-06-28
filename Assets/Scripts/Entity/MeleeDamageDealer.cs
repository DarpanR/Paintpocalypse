using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CollisionAttack : MonoBehaviour {
    public BaseEntity ownerEntity;
    public int damage = 0;
    [TagMaskField] 
    public string targetTag = "Untagged";
   
    public OperationType operationType;
    public StatType affectedType;

    StatBroker statBroker;

    private void Awake() {
        var collider = GetComponent<Collider2D>();
        collider.isTrigger = true;
    }

    private void Start() {
        targetTag = targetTag == "Untagged" ? ownerEntity.eData.targetTag : targetTag;
        damage = (damage > 0) ? damage : (int)ownerEntity.CurrentStats.GetValueOrDefault(
            StatType.Damage, 0f);
        statBroker = new StatBroker(
            new StatSet(
                new Stat(StatType.Damage, damage)
            )
        );
        ownerEntity.OnAddStatModifier += statBroker.Add;
    }

    public void LateUpdate() {
        statBroker.Tick(Time.deltaTime);
    }

    protected void OnTriggerStay2D(Collider2D collision) {
        if (collision.CompareTag(targetTag)) {
            collision.GetComponent<BaseEntity>().TakeDamage(
                OperationFactory.GetOperation(
                    operationType,
                    affectedType,
                    -statBroker.CurrentStats[StatType.Damage].value
            ));
        }
    }
}
