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
    public EntityStatType affectedType;

    StatBroker<WeaponStatType> statBroker;

    private void Awake() {
        var collider = GetComponent<Collider2D>();
        collider.isTrigger = true;
    }

    private void Start() {
        targetTag = targetTag == "Untagged" ? ownerEntity.TargetTag : targetTag;
        damage = (damage > 0) ? damage : 10;
        statBroker = new StatBroker<WeaponStatType>(
            new StatSet<WeaponStatType>(
                new Stat<WeaponStatType>(WeaponStatType.Damage, damage)
            )
        );
        ownerEntity.OnAddStatModifier += AddStatModifier;
    }

    public void LateUpdate() {
        statBroker.Tick(Time.deltaTime);
    }

    public void AddStatModifier(StatModData def) {
        var wMod = def.statMods.FindAll(a => a.GetModCapabilities == ModCapabilities.Weapon);

        if (wMod.Count > 0) {
            var wStatMod = new StatModifier<WeaponStatType>(wMod, def.GUID, def.duration);
            statBroker.Add(wStatMod, def.settable);
        }
    }

    protected void OnTriggerStay2D(Collider2D collision) {
        if (collision.CompareTag(targetTag)) {
            collision.GetComponent<BaseEntity>().TakeDamage(
                OperationFactory<EntityStatType>.GetOperation(
                    operationType,
                    affectedType,
                    -statBroker.CurrentStats[WeaponStatType.Damage].value
            ));
        }
    }
}
