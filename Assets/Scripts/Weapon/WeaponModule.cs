using System.Collections.Generic;
using UnityEngine;

public class WeaponModule
{
    //Current statBroker
    [SerializeField]
    int level = 1;

    readonly WeaponData data;
    readonly string targetTag;
    readonly Transform parentOrigin;

    readonly WeaponBehavior style;
    readonly StatBroker<WeaponStatType> statBroker;
    readonly List<FirePoint> firePoints;

    public int Level { get; private set; }
    public StatSet<WeaponStatType> CurrentStats => statBroker.CurrentStats;
    // explicit interface impl so consumer sees only the base type
    
    public WeaponModule(Transform parentOrigin, WeaponData data, string targetTag) {
        this.parentOrigin = parentOrigin;
        this.data = data;
        this.targetTag = targetTag;
        

        Level = level;
        statBroker = new StatBroker<WeaponStatType>(data.baseStats);
        firePoints = data.firePoints;
        style = WeaponFactory.CreateStyle(data);
    }

    public void TryFire() {
        float deltaTime = Time.deltaTime;

        style.fireMode.Tick(deltaTime);
        style.firePointBehavior.Tick(deltaTime);
        statBroker.Tick(deltaTime);

        if (!style.fireMode.ShouldFire()) return;

        var distribution = style.firingPattern.GetShotDistribution(firePoints.Count,
            (int)CurrentStats[WeaponStatType.ProjectileCount].value);

        for (int i = 0; i < firePoints.Count; i++) {
            int shots = distribution[i];

            if (shots <= 0) continue;
            var spawnPoints = style.firePointBehavior.GetFirePoints(parentOrigin, firePoints[i], shots);

            foreach (var fp in spawnPoints) {
                style.attack.Fire(fp, CurrentStats, targetTag, data.affectedType, style.damage);
            }
        }
    }

    public virtual bool Upgrade() {
        if (Level >= data.maxLevel) return false;
        Level++;
        
        statBroker.UpdateBaseStat(WeaponStatType.Damage, GetComputedProperties(WeaponStatType.Damage));
        statBroker.UpdateBaseStat(WeaponStatType.FireRate, GetComputedProperties(WeaponStatType.FireRate));

        //fireTimer.Reset(CurrentstatBroker[WeaponStatType.FireRate].value);
        // Recalculate with new base statBroker while preserving modifiers
        return true;
    }

    public bool AddStatModifier(StatModifier<WeaponStatType> mod, SettableType setType) {
        return statBroker.Add(mod, setType);
    }

    float GetComputedProperties (WeaponStatType type) {
        return CurrentStats[type].value + data.LevelStats.GetValueOrDefault(type, 0f);
    }

    // -- Computed Properties (base + level up stat) --
    //public int Penetration =>
    //    data.basePenetration + data.luPenetration * (Level - 1);
    //public int ProjectileCount =>
    //    data.baseProjectileCount + data.luProjectileCount * (Level - 1);
}
