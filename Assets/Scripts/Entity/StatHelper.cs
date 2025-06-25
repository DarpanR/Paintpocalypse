using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType {
    MaxHealth,
    MoveSpeed,
    LocalScale,
    InvincibitilityDuration,
    Damage,
    FireRate,
}

[Serializable]
public class Stat {
    public StatType type;
    public float value;

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
    public Stat this[StatType type] => stats[type];


    public void AddStat(StatType type, float baseValue) {
        AddStat(new Stat { type = type, value = baseValue });
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

    public void OnAfterDeserialize() => RebuildDictionary();
    public void OnBeforeSerialize() { }
}

public interface IStatSetModifier {
    void Activate(StatSet stats);
    void Activate(WeaponManager weaponManager);
}

public interface IstatSetTarget {
    StatSet Stats { get; }
    StatBroker StatBroker { get; }
}

public interface IWeaponManagerTarget {
    WeaponManager WeaponManager { get; }
}

public interface IWeaponModifier {
    void Activate(WeaponManager weaponManager);
}