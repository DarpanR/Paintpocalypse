using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackBehavior {
    void Fire(Vector3 orgin, Quaternion rotation, StatSet stats, string targetTag, IDamageBehavior damageBehavior);
}

public class ProjectileAttack : IAttackBehavior {
    private GameObject prefab;

    public ProjectileAttack(GameObject projectilePrefab) {
        this.prefab = projectilePrefab;
    }

    public void Fire(Vector3 orgin, Quaternion rotation, StatSet stats, string targetTag, IDamageBehavior damageBehavior) {
        var go = ProjectileManager.Instance.Request(prefab);
        go.transform.SetPositionAndRotation(orgin, rotation);

        if (go.TryGetComponent(out StraightProjectile proj)) {
            proj.Init(stats, targetTag, damageBehavior);
            proj.onDestroyed = () => ProjectileManager.Instance.Return(go);

        }
    }
}
