using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MagnifyingGlass : StatModifier<MagnifyingGlassDefinition>, IMouseAbility
{
    BaseEntity e;
    Vector3 originalSize;
    float originalSpeed;
    Dictionary<WeaponDefinition, float> originalDamages;

    public MagnifyingGlass(MagnifyingGlassDefinition definition) : base(definition) {}

    public float CoolDown { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override void Activate(BaseEntity entity) {
        if (entity == null) return;
        e = entity;

        originalSize = entity.transform.localScale;
        originalSpeed = entity.moveSpeed;

        entity.transform.localScale *= Definition.sizeMultiplier;
        entity.moveSpeed *= Definition.speedMultplier;

        WeaponManager wm = entity.GetComponent<WeaponManager>();

        foreach(var weapon in wm.allWeapons) {
            originalDamages.Add(weapon, weapon.baseDamage);
            weapon.baseDamage *= Definition.damageMultiplier;
        }
    }

    public override void Deactivate() {
        e.transform.localScale = originalSize;
        e.moveSpeed = originalSpeed;

        WeaponManager wm = e.GetComponent<WeaponManager>();

        foreach(var damage in originalDamages) {
            wm.allWeapons.Find(w => w == damage.Key).baseDamage = damage.Value;
        }
    }

    public void OnSelect() {
        //throw new System.NotImplementedException();
    }

    public bool OnUse(Vector3 clickPos) {
        List<BaseEntity> entities = TargetingFactory.GetTargets(Definition.targetingMode, clickPos, "Enemy");

        if (entities.Count == 0) return false;

        foreach (BaseEntity entity in entities)
            entity.AddStatModifier(new MagnifyingGlass(Definition));
        return true;
    }
}
