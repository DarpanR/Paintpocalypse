using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum EntityStatType {
    MaxHealth,
    CurrentHealth,
    MaxShield,
    CurrentShield,
    LocalScale,
    InvincibilityDuration,
    Speed,
}

public enum WeaponStatType { 
    Speed,
    Damage,
    FireRate,
    Lifetime,
    Penetration,
    ProjectileCount,
    Cooldown,
    FirePointAngle,
    FirePointSweepSpeed,
}

public enum SettableType { 
    Single, 
    Timer, 
    Multi
}

public enum ModifierCapabilities {
    None = 0,
    Weapon = 1,
    Stat = 2,
}

[Serializable]
public class Stat<T> {
    public T type;
    public float value;

    public Stat(T type, float value) {
        this.type = type;
        this.value = value;
    }

    public void Apply(IoperationStrategy<T> operation) {
        value = operation.Calculate(value);
    }
}

[Serializable]
public class StatSet<T> : ISerializationCallbackReceiver {
    [SerializeField]
    List<Stat<T>> statList = new ();
    Dictionary<T, Stat<T>> stats = new ();

    public List<Stat<T>> StatList => statList;

    public StatSet() {
        RebuildDictionary();
    }

    public StatSet(params Stat<T>[] stats) {
        foreach (var s in stats)
            AddStat(s); 
        RebuildDictionary();
    }

    public Stat<T> this[T type] => stats[type];

    public void AddStat(T type, float baseValue) {
        AddStat(new Stat<T>(type, baseValue));
    }

    public void AddStat(Stat<T> stat) {
        statList.RemoveAll(s => s.type.Equals(stat.type));
        statList.Add(stat);
        stats[stat.type] = stat;
        RebuildDictionary();
    }

    public StatSet<T> Clone() {
        StatSet<T> clone = new ();

        foreach (var s in statList)
            clone.AddStat(s.type, s.value);
        return clone;
    }

    void RebuildDictionary() {
        stats = new Dictionary<T, Stat<T>>();

        foreach (var s in statList)
            stats[s.type] = s;
    }

    public float GetValueOrAdd(T type, float defaultVal = 0f) {
        if (!stats.TryGetValue(type, out var s)) {
            s = new Stat<T>(type, defaultVal);
            statList.Add(s);
            stats[type] = s; 
        }
        return s.value;
    }

    public bool HasStat(T type) => stats.ContainsKey(type);
    
    public Stat<T> GetStatOrNull(T type) => stats.TryGetValue(type, out var s) ? s : null;
    
    public float GetValueOrDefault(T type, float defaultValue = 0f) => 
        stats.TryGetValue(type, out var s) ? s.value : defaultValue;
    public void OnAfterDeserialize() => RebuildDictionary();
    
    public void OnBeforeSerialize() { }
}

public interface IstatSetTarget<T> {
    StatSet<T> CurrentStats { get; }
    bool AddStatModifier(StatModData modifier);
}

public interface IWeaponManagerTarget {
    WeaponManager WeaponManager { get; }
}

public interface IWeaponModifier {
    void Activate(WeaponManager weaponManager);
}