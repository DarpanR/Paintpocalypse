using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum PickupType { 
    Weapon, 
    StatModifier, 
    Currency, 
    EXP,
}

[Serializable]
public struct PickupData {
    public Sprite pickupIcon;
    public Sprite dropIcon;
    [TagMaskField]
    public string pickupTag;
    public PickupType pickupType;
    [Min(-1)]
    public float lifeTime;
    [Min(-1)]
    public int totalUsage;
    public TargetingType dropOperationType;
    public float dropRadius;
}

public interface IPickupData {
    string PickupName { get; }
    Sprite PickupIcon { get; }
    Sprite DropIcon { get; }
    string PickupTag { get; }
    PickupType PickupType { get; }
    float LifeTime { get; }
    int TotalUsage { get; }
    TargetingType TargetingType { get; }
    float TargetRadius { get; }
}

public static class PickupDropFactory {
    public static bool Execute(BaseEntity entity, IPickupData pickup) {
        if (entity == null || !entity.CompareTag(pickup.PickupTag))
            return false;

        switch (pickup.PickupType) {
            case PickupType.Weapon:
                if (pickup is not WeaponData) 
                    throw new InvalidCastException($"Pickup {pickup} is not a WeaponData!");
                return entity.WeaponManager.Equip(pickup as WeaponData);
            case PickupType.StatModifier:
                if (pickup is not StatModData)
                    throw new InvalidCastException($"Pickup {pickup} is not a StatModData!");
                entity.AddStatModifier(pickup as StatModData);
                return true;
            default:
                return false;
        }
    }
}

public abstract class DropOperation : TargetingOperation {
    public event Action<BaseEntity> OnPickUp;

    public DropOperation(Targeting targeting) :base(targeting) {}

    public void InvokeOnPickUp(BaseEntity entity) => OnPickUp?.Invoke(entity);

    public override void Preview() {
        targeting.Transform.position = GameInputManager.Instance.MouseWorldPosition;
    }

    public override TargetResult Execute() => new TargetResult {
        Position = GameInputManager.Instance.MouseWorldPosition,
        success = true,
    };
}

public class OverMouseDrop : DropOperation {
    public OverMouseDrop(Targeting targeting) : base(targeting) { }
}

public class AttachToTargetDrop : DropOperation {

    public AttachToTargetDrop(Targeting targeting) : base(targeting) { }

    public override void Preview() {
        //GUI to draw here
        base.Preview();
    }
    public override TargetResult Execute() {
        //Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Collider2D col = Physics2D.OverlapPoint(mousePos);

        //if (col != null) {
        //    targeting.Transform.SetParent(col.transform);
        //    targeting.Transform.localPosition = Vector3.zero;
        //}
        return new TargetResult();
    }
}

public class ClosestToMouseDrop : DropOperation {
    Collider2D prevClosest;
    Material defaultMaterial;

    public ClosestToMouseDrop(Targeting targeting) : base(targeting) {
    }

    public override void Preview() {
        base.Preview();

        Collider2D[] hits = Physics2D.OverlapCircleAll(GameInputManager.Instance.MouseWorldPosition, targeting.TargetRadius);
        Collider2D closest = null;
        float closestDist = float.MaxValue;

        foreach (var hit in hits) {
            if (hit.CompareTag(targeting.PickupTag)) {
                float dist = Vector2.Distance(GameInputManager.Instance.MouseWorldPosition, hit.transform.position);

                if (dist < closestDist) {
                    closest = hit;
                    closestDist = dist;
                }
            }
        }

        if (prevClosest != closest)
            SetHighlight(closest);
    }

    public override TargetResult Execute() {
        if (prevClosest == null) return new TargetResult {
            Position = GameInputManager.Instance.MouseWorldPosition,
            success =false
        };
        //targeting.Transform.position = prevClosest.transform.position;                    
        BaseEntity entity = prevClosest.GetComponent<BaseEntity>();
        
        InvokeOnPickUp(entity);
        SetHighlight(null);

        return new TargetResult {
            Position = entity.transform.position,
            TargetEntity = entity,
            success = true
        };
    }

    void SetHighlight(Collider2D col) {
        if (prevClosest != null) {
            var sr = prevClosest.GetComponent<BaseEntity>().rend;
            sr.material = defaultMaterial;
        }

        if(col != null) {
            var sr = col.GetComponent<BaseEntity>().rend;
            defaultMaterial = sr.material;
            sr.material = targeting.OutlineMaterial;
        }
        prevClosest = col;
    }
}

public class AreaAroundMouseDrop : DropOperation {
    public AreaAroundMouseDrop(Targeting targeting) : base(targeting) {}

    public override void Preview() {
        /// GUI to draw here
        base.Preview();
    }

    public override TargetResult Execute() {
        //Vector3 pos = targeting.Transform.position;
        //Collider2D[] hits = Physics2D.OverlapCircleAll(pos, areaRadius);

        //foreach (var hit in hits) {
        //    if (hit.CompareTag(targeting.PickupTag)) {
        //        BaseEntity entity = hit.GetComponent<BaseEntity>();

        //        if (entity != null)
        //            PickupDropFactory.Execute(entity, areaRadius);
        //    }
        //}
        return new TargetResult();
    }
}

public class AttractionDrop : DropOperation {
    HashSet<BaseEntity> entities = new HashSet<BaseEntity>();

    public AttractionDrop(Targeting targeting) : base(targeting) { }

    public override void Preview() {
        // GUI to draw here
        base.Preview();
    }

    public override TargetResult Execute() {
        Vector3 pos = targeting.Transform.position;
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, targeting.TargetRadius);

        foreach (var hit in hits) {
            //skips if not entity, no tag match, and already picked up
            if (!hit.TryGetComponent<BaseEntity>(out var entity) ||
                !entity.CompareTag(targeting.PickupTag) ||
                entities.Contains(entity))
                continue;

            if (entity is IAttractable attractee) {
                attractee.SetOverrideTarget(targeting.Transform);
                OnPickUp += e => {
                    if (e == null || e.GUID == entity.GUID)
                        attractee.OnAttractionEnd();
                };
            }
            entities.Add(entity);
        }
        return new TargetResult { 
            Position = GameInputManager.Instance.MouseWorldPosition,
            success = true };
    }
}