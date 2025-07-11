using UnityEngine;

public enum AttackBehaviorType {
    Projectile,
    Raycast,
    Beam
}

public interface IAttackBehavior {
    void Fire(FirePoint firePoint, StatSet<WeaponStatType> stats, string targetTag, EntityStatType affectedType, IDamageBehavior damageBehavior);
}

public class ProjectileAttack : IAttackBehavior {
    private GameObject prefab;

    public ProjectileAttack(GameObject projectilePrefab) {
        this.prefab = projectilePrefab;
    }

    public void Fire(FirePoint firePoint, StatSet<WeaponStatType> stats, string targetTag, EntityStatType affectedType, IDamageBehavior damageBehavior) {
        var go = ProjectileManager.Instance.Request(prefab);
        go.transform.SetPositionAndRotation(firePoint.position, Quaternion.Euler(0f,0f,firePoint.angle));

        if (go.TryGetComponent(out Projectile proj)) {
            proj.Init(stats, targetTag, affectedType, damageBehavior);
            proj.onDestroyed = () => ProjectileManager.Instance.Return(go);
        }
    }
}

public class RaycastAttack : IAttackBehavior {
    private GameObject prefab;

    public void Fire(FirePoint firePoint, StatSet<WeaponStatType> stats, string targetTag, EntityStatType affectedType, IDamageBehavior damageBehavior) {
        throw new System.NotImplementedException();
    }
} 

public class BeamAttack : IAttackBehavior {
    private GameObject prefab;

    public void Fire(FirePoint firePoint, StatSet<WeaponStatType> stats, string targetTag, EntityStatType affectedType, IDamageBehavior damageBehavior) {
        throw new System.NotImplementedException();
    }
}
