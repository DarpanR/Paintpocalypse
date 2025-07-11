using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageBehaviorType {
    Direct,
    Aoe,
    Dot
}

public interface IDamageBehavior {
    void ApplyDamage(GameObject target, string TargetTag, StatSet<WeaponStatType> stats, EntityStatType affectedType);
}

public class DirectDamage : IDamageBehavior {
    public void ApplyDamage(GameObject target, string TargetTag, StatSet<WeaponStatType> stats, EntityStatType affectedType) {
        if (!target.CompareTag(TargetTag)) return;

        if (target.TryGetComponent(out BaseEntity entity)) {
            float damage = stats[WeaponStatType.Damage].value;
            entity.TakeDamage(OperationFactory<EntityStatType>.GetOperation(OperationType.Addition, affectedType, -damage));
        }
    }
}

public class AoeDamage : IDamageBehavior {
    public void ApplyDamage(GameObject target, string TargetTag, StatSet<WeaponStatType> stats, EntityStatType affectedType) {
        throw new System.NotImplementedException();
    }
}

public class DotDamage : IDamageBehavior {
    public void ApplyDamage(GameObject target, string TargetTag, StatSet<WeaponStatType> stats, EntityStatType affectedType) {
        throw new System.NotImplementedException();
    }
}
