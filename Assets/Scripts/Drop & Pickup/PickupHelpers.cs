using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickupType { 
    Weapon, 
    StatModifier, 
    Currency, 
    EXP,
}

public enum DropType { 
    Counter, 
    Timer
}

public interface IPickupDefinition {
    Sprite PickupIcon { get; }
    Sprite DropIcon { get; }
    string PickupTag { get; }
    PickupType PickupType { get; }
    DropType DropType { get; }
    int DropCount { get; }
    float PickupCount { get; }
}