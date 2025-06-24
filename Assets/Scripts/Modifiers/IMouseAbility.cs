using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMouseAbility
{
    float CoolDown { get; set; }
    TargetingMode TargetingMode { get; set; }

    void OnSelect();              // called when chosen
    bool OnUse(Vector3 position); // called when clicked
}
