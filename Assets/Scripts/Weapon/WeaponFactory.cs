using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[Serializable]
public struct WeaponBehavior {
    public IAttackBehavior attack;
    public IDamageBehavior damage;
    public IFireMode fireMode;
    public IFirePointBehavior firePointBehavior;
    public IFiringPattern firingPattern;
}

public static class WeaponFactory {
    public static readonly Dictionary<FireModeType, Func<WeaponData, IFireMode>> fireModeMap = new() { 
        { FireModeType.Burst, data => new BurstFire(
            data.baseStats.GetValueOrDefault(StatType.FireRate, 1f),
            (int)data.baseStats.GetValueOrDefault(StatType.ProjectileCount, 3f),
            data.baseStats.GetValueOrDefault(StatType.Cooldown, 3f))
        },
        { FireModeType.RateFire, data => new RateFire(
            data.baseStats.GetValueOrDefault(StatType.FireRate, 1f))
        },
        { FireModeType.FullAuto, data => new FullAutoFire(
            data.baseStats.GetValueOrDefault(StatType.FireRate, 1f))
        }
    };

    public static readonly Dictionary<AttackBehaviorType, Func<WeaponData, IAttackBehavior>> attackMap = new() { 
        { AttackBehaviorType.Projectile, data => new ProjectileAttack(data.projectile) },
        { AttackBehaviorType.Raycast, data => new RaycastAttack() },
        { AttackBehaviorType.Beam, data => new BeamAttack() }
    };

    public static readonly Dictionary<DamageBehaviorType, Func<IDamageBehavior>> damageMap = new() { 
        { DamageBehaviorType.Direct, () => new DirectDamage() },
        { DamageBehaviorType.Aoe, () => new AoeDamage() },
        { DamageBehaviorType.Dot, () => new DotDamage() }
    };

    public static readonly Dictionary<FirePointBehaviorType, Func<WeaponData, IFirePointBehavior>> firePointMap = new() {
        { FirePointBehaviorType.Straight, data => new StraightFirePoint() 
        },
        { FirePointBehaviorType.Arc,data => new ArcFirePoint(
            data.baseStats.GetValueOrDefault(StatType.FirePointAngle, 45f)) 
        },
        { FirePointBehaviorType.Revolving,data => new RevolvingFirePoint(
            data.baseStats.GetValueOrDefault(StatType.FirePointAngle, 90f)) 
        },
        { FirePointBehaviorType.RandomArc, data => new RandomArcFirePoint(
            data.baseStats.GetValueOrDefault(StatType.FirePointAngle, 30f)) }
    };

    public static readonly Dictionary<FiringPatternType, Func<IFiringPattern>> firingPatternMap = new() {
        { FiringPatternType.One, () => new OnePerFirePointPattern() },
        { FiringPatternType.All, () => new AllPerFirePointParttern() },
        { FiringPatternType.Even, () => new EvenDistributionPattern() }
    };

    public static WeaponBehavior CreateStyle(WeaponData data) {
        WeaponBehavior weapon = new();

        if (!fireModeMap.TryGetValue(data.fireMode, out var fireModeFactory)) {
            Debug.LogWarning($"FireMode not implemented: {data.fireMode}");
        } else {
            weapon.fireMode = fireModeFactory(data);
        }

        if (!attackMap.TryGetValue(data.attackBehavior, out var attackFactory)) {
            Debug.LogWarning($"AttackBehaviorType not implemented: {data.attackBehavior}");
        } else {
            weapon.attack = attackFactory(data);
        }

        if (!damageMap.TryGetValue(data.damageBehavior, out var damageFactory)) {
            Debug.LogWarning($"DamageBehaviorType not implemented: {data.damageBehavior}");
        } else {
            weapon.damage = damageFactory();
        }

        if (!firePointMap.TryGetValue(data.fireBehavior, out var firePointFactory)) {
            Debug.LogWarning($"FirePointType not implemented: {data.firePoints}");
        } else {
            weapon.firePointBehavior = firePointFactory(data);
        }

        if (!firingPatternMap.TryGetValue(data.firingPattern, out var firingPatternfactory)) {
            Debug.LogWarning($"FiringPattern not implemented: {data.firingPattern}");
        } else {
            weapon.firingPattern = firingPatternfactory();
        }
        return weapon;
    }
}