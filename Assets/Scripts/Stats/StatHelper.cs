using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType {
    MaxHealth,
    CurrentHealth,
    Velocity,
    LocalScale,
    InvincibilityDuration,
    Damage,
    FireRate,
}

[Serializable]
public class Stat {
    public StatType type;
    public float value;

    public Stat(StatType type, float value) {
        this.type = type;
        this.value = value;
    }

    public void Apply(IoperationStrategy operation) {
        value = operation.Calculate(value);
    }
}

[Serializable]
public class StatSet : ISerializationCallbackReceiver {
    [SerializeField]
    List<Stat> statList = new List<Stat>();
    Dictionary<StatType, Stat> stats = new Dictionary<StatType, Stat>();

    public StatSet() {
        RebuildDictionary();
    }

    public StatSet(params Stat[] stats) {
        statList = new List<Stat>(stats);
        RebuildDictionary();
    }

    public Stat this[StatType type] => stats[type];

    public void AddStat(StatType type, float baseValue) {
        AddStat(new Stat(type, baseValue));
    }

    public void AddStat(Stat stat) {
        var existing = statList.Find(s=>s.type == stat.type);

        if(existing != null) existing.value = stat.value;
        else statList.Add(stat);
        RebuildDictionary();
    }

    public StatSet Clone() {
        var clone = new StatSet();

        foreach (var s in statList)
            clone.AddStat(s.type, s.value);
        return clone;
    }

    void RebuildDictionary() {
        stats = new Dictionary<StatType, Stat>();

        foreach (var s in statList)
            stats[s.type] = s;
    }

    public float GetValueOrAdd(StatType type, float defaultVal = 0f) {
        if (!stats.TryGetValue(type, out var s)) {
            s = new Stat(type, defaultVal);
            statList.Add(s);
            stats[type] = s; 
        }
        return s.value;
    }

    public bool HasStat(StatType type) => stats.ContainsKey(type);
    
    public Stat GetStatOrNull(StatType type) => stats.TryGetValue(type, out var s) ? s : null;
    
    public float GetValueOrDefault(StatType type, float defaultValue = 0f) => 
        stats.TryGetValue(type, out var s) ? s.value : defaultValue;
    public void OnAfterDeserialize() => RebuildDictionary();
    
    public void OnBeforeSerialize() { }
}

public interface IStatSetModifier {
    List<IoperationStrategy> Activate();
}

public interface IstatSetTarget {
    StatSet CurrentStats { get; }
    bool AddStatModifier(StatModifier modifier);
}

public interface IWeaponManagerTarget {
    WeaponManager WeaponManager { get; }
}

public interface IWeaponModifier {
    void Activate(WeaponManager weaponManager);
}