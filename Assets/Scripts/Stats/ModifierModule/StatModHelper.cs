using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ModCapabilities {
    None = 0,
    Entity = 1 << 0,
    Weapon = 1 << 1,
}

public interface IStatOperationProvider<T> {
    IoperationStrategy<T> ToOperation();
}

[Serializable]
public abstract class StatModBehavior {
    public float value;
    public OperationType operation;
    public abstract ModCapabilities GetModCapabilities { get; }
}

[Serializable]
public class EntityModBehavior : StatModBehavior, IStatOperationProvider<EntityStatType> {
    public EntityStatType statType;
    public override ModCapabilities GetModCapabilities => ModCapabilities.Entity;
    public IoperationStrategy<EntityStatType> ToOperation() =>
        OperationFactory<EntityStatType>.GetOperation(operation, statType, value);
}

[Serializable]
public class WeaponModBehavior : StatModBehavior {
    public WeaponStatType statType;
    public override ModCapabilities GetModCapabilities => ModCapabilities.Weapon;
    public IoperationStrategy<WeaponStatType> ToOperation() =>
     OperationFactory<WeaponStatType>.GetOperation(operation, statType, value);
}