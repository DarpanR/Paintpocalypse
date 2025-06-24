using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//[Serializable]
//public class Copyable {
//    public GameObject enemy;
//    public int amount;
//}

[Serializable]
public class EyeDropper : IMouseAbility
{
    public TargetingMode targetingMode = TargetingMode.TargetUnderMouse;
    //public List<Copyable> enemyPrefabs;

    [SerializeField]
    float cooldown;
    BaseEntity copiedEntity;

    public float CoolDown { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public TargetingMode TargetingMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void OnSelect() {
        copiedEntity = null;
    }

    public bool OnUse(Vector3 clickPos) {
        if (copiedEntity != null) {        /// Copy
            List<BaseEntity> entities = TargetingFactory.GetTargets(targetingMode, clickPos, "Enemy");

            if (entities.Count > 0)
                copiedEntity = entities[0];
        } else {                             /// Paste
            GameObject.Instantiate(copiedEntity, clickPos, Quaternion.identity);
            return true;
        }
        return false;
    }
}
