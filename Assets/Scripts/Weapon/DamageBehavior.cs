using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageBehaviorType {
    Direct,
    Aoe,
    Dot
}

public interface IDamageBehavior {
    void ApplyDamage(GameObject target, StatSet stats, string TargetTag);
}

public class DirectDamage : IDamageBehavior {
    public void ApplyDamage(GameObject target, StatSet stats, string TargetTag) {
        if (!target.CompareTag(TargetTag)) return;

        if (target.TryGetComponent(out BaseEntity entity)) {
            float damage = stats[StatType.Damage].value;
            entity.TakeDamage(OperationFactory.GetOperation(OperationType.Addition, StatType.CurrentHealth, -damage));
        }
    }
}

public class AoeDamage : IDamageBehavior {
    public void ApplyDamage(GameObject target, StatSet stats, string TargetTag) {
        throw new System.NotImplementedException();
    }
}

public class DotDamage : IDamageBehavior {
    public void ApplyDamage(GameObject target, StatSet stats, string TargetTag) {
        throw new System.NotImplementedException();
    }
}
