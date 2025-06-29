using System;
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
    OverMouse,
    ClosestToDrop,
    Area,
    Attraction,
    AttachToTarget,
    QueueAtTarget,
    Directional
}

public interface IPickupData {
    string DisplayName { get; }
    Sprite PickupIcon { get; }
    Sprite DropIcon { get; }
    string PickupTag { get; }
    PickupType PickupType { get; }
    float LifeTime { get; }
    int TotalUsage { get; }
    float PickupCount { get; }
    DropType DropType { get; }
    float DropRadius{ get; }
    float DropForce { get; }
}

public static class DropOperationFactory {
    public static DropOperation GetOperation(PickupHandler handler, IPickupData pickup) {
        switch (pickup.DropType) {
            case DropType.AttachToTarget:
                return new AttachToTargetOperation(handler);
            case DropType.ClosestToDrop:
                return new ClosestToMouseOperation(handler, pickup.DropRadius);
            case DropType.Area:
                return new AreaOperation(handler, pickup.DropRadius);
            case DropType.Attraction:
                return new AttractionOperation(handler, pickup.DropRadius, pickup.DropForce);
            case DropType.OverMouse:
            default:
                return new OverMouseOperation(handler);
        }
    }
}

public static class PickupOperationFactory {
    public static bool Execute(BaseEntity entity, IPickupData pickup) {
        if (entity == null || pickup.PickupTag != entity.tag)
            return false;

        switch (pickup.PickupType) {
            case PickupType.Weapon:
                if (pickup is not WeaponData) 
                    throw new InvalidCastException($"Pickup {pickup} is not a WeaponData!");
                entity.WeaponManager.Equip(pickup as WeaponData);
                return true;
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

public abstract class DropOperation {
    protected PickupHandler pickupHandler;
    public float rotationSpeed = 45f;
    public virtual bool ShouldDestroyAfterExecute => false;

    public DropOperation(PickupHandler pickupHandler) {
        this.pickupHandler = pickupHandler;
    }

    public virtual void Preview() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        pickupHandler.transform.position = mousePos;
    }

    public virtual void Execute() {
        pickupHandler.transform.Rotate(0, rotationSpeed * Time.deltaTime, 0f);
    }
}

public class OverMouseOperation : DropOperation {
    public OverMouseOperation(PickupHandler pickupHandler) : base(pickupHandler) { }

    public override void Execute() { }
}

public class AttachToTargetOperation : DropOperation {
    public override bool ShouldDestroyAfterExecute => true;

    public AttachToTargetOperation(PickupHandler pickupHandler) : base(pickupHandler) { }

    public override void Preview() {
        //GUI to draw here
        base.Preview();
    }
    public override void Execute() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D col = Physics2D.OverlapPoint(mousePos);

        if (col != null) {
            pickupHandler.transform.SetParent(col.transform);
            pickupHandler.transform.localPosition = Vector3.zero;
        }
    }
}

public class ClosestToMouseOperation : DropOperation {
    float searchRadius = 3f;
    public override bool ShouldDestroyAfterExecute => true;

    public ClosestToMouseOperation(PickupHandler pickupHandler, float searchRadius = 3f) : base(pickupHandler) {
        this.searchRadius = searchRadius;
    }

    public override void Preview() {
        /// GUI to draw here
        base.Preview();
    }

    public override void Execute() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        Collider2D[] hits = Physics2D.OverlapCircleAll(mousePos, searchRadius);
        Collider2D closest = null;
        float closestDist = float.MaxValue;

        foreach (var hit in hits) {
            if (hit.CompareTag(pickupHandler.PickupTag)) {
                float dist = Vector2.Distance(mousePos, hit.transform.position);

                if (dist < closestDist) {
                    closest = hit;
                    closestDist = dist;
                }
            }
        }

        if (closest != null)
            pickupHandler.transform.position = closest.transform.position;
        else
            pickupHandler.transform.position = mousePos;
    }
}

public class AreaOperation : DropOperation {
    float areaRadius = 3f;
    public override bool ShouldDestroyAfterExecute => false;

    public AreaOperation(PickupHandler pickupHandler, float areaRadius = 3f) : base(pickupHandler) {
        this.areaRadius = areaRadius;
    }

    public override void Preview() {
        /// GUI to draw here
        base.Preview();
    }

    public override void Execute() {
        Vector3 pos = pickupHandler.transform.position;
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, areaRadius);

        foreach (var hit in hits) {
            if (hit.CompareTag(pickupHandler.PickupTag)) {
                BaseEntity entity = hit.GetComponent<BaseEntity>();

                if (entity != null)
                    PickupOperationFactory.Execute(entity, pickupHandler.Data);
            }
        }
    }
}

public class AttractionOperation : DropOperation {
    float AttractionRadius = 4f;
    float pullForce = 5f;
    public override bool ShouldDestroyAfterExecute => false;

    public AttractionOperation(PickupHandler pickupHandler, float radius = 4f, float force = 5f) : base(pickupHandler) {
        AttractionRadius = radius;
        pullForce = force;
    }

    public override void Preview() {
        // GUI to draw here
        base.Preview();
    }

    public override void Execute() {
        Vector3 pos = pickupHandler.transform.position;
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, AttractionRadius);

        foreach (var hit in hits) {
            if (hit.CompareTag(pickupHandler.PickupTag)) {
                Rigidbody2D rb = hit.attachedRigidbody;

                if (rb != null) {
                    Vector2 dir = (pos - rb.transform.position).normalized;
                    rb.AddForce(dir * pullForce, ForceMode2D.Impulse);
                }
            }
        }
    }
}