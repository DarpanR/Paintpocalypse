using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IDamageBehavior {
    void ApplyDamage(GameObject target, StatSet stats, string TargetTag);
}

public class DirectDamage : IDamageBehavior {
public void ApplyDamage(GameObject target, StatSet stats, string TargetTag) {
        if (!target.CompareTag(TargetTag)) return;

        if (target.TryGetComponent(out BaseEntity entity)) {
            float damage = stats[StatType.Damage].value;
            entity.TakeDamage(OperationFactory.GetOperation(OperationType.Addition, StatType.CurrentHealth, - damage));
        }
    }
}
