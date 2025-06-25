using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickupType { 
    Weapon, 
    StatModifier, 
    Currency, 
    EXP,
}

public interface IPickupDefinition {
    PickupType PickupType { get; }
    int Amount { get; }
    Sprite PickupIcon { get; }
}
