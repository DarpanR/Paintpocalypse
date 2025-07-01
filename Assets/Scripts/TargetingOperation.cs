using System;
using UnityEngine;
using UnityEngine.UIElements;

public enum TargetingType {
    OverMouse,
    ClosestToMouse,
    AreaAroundMouse,
    Attraction,
    AttachToTarget,
    QueueAtTarget,
    Directional
}

public struct Targeting {
    public Transform Transform;
    public TargetingType TargetingType;
    public string PickupTag;
    public Material OutlineMaterial;
    public float TargetRadius;
}

public struct TargetResult {
    public Vector3? Position;
    public BaseEntity TargetEntity;
    public bool success;
}

public static class TargetOperationFactory {
    public static DropOperation GetOperation(Targeting targeting) {
        switch (targeting.TargetingType) {
            case TargetingType.AttachToTarget:
                return new AttachToTargetDrop(targeting);
            case TargetingType.ClosestToMouse:
                return new ClosestToMouseDrop(targeting);
            case TargetingType.AreaAroundMouse:
                return new AreaAroundMouseDrop(targeting);
            case TargetingType.Attraction:
                return new AttractionDrop(targeting);
            case TargetingType.OverMouse:
            default:
                return new OverMouseDrop(targeting);
        }
    }
}

public abstract class TargetingOperation {
    protected Targeting targeting;

    public TargetingOperation(Targeting targeting) {
        this.targeting = targeting;
    }

    public abstract void Preview();
    public abstract TargetResult Execute();
}

public abstract class SelectionTargeting : TargetingOperation {
    public Action<BaseEntity> onSelected;

    public SelectionTargeting(Targeting targeting) : base(targeting) { }

    public override void Preview() {
        targeting.Transform.position = GameInputManager.Instance.MouseScreenPosition;
    }
}

//public class AttachToTargetSelection : SelectionTargeting {
//    public AttachToTargetSelection(Targeting targeting) : base(targeting) { }

//    public override TargetResult Execute() {
//        throw new NotImplementedException();
//    }
//}

public class OverMouseSelection : SelectionTargeting {

    public OverMouseSelection(Targeting targeting) : base(targeting) { }

    public override void Preview() {
        //GUI to draw here
        base.Preview();
    }

    public override TargetResult Execute() {

        Collider2D col = Physics2D.OverlapPoint(GameInputManager.Instance.MouseWorldPosition);

        if (col != null && 
            col.TryGetComponent<BaseEntity>(out var entity) && 
            entity.CompareTag(targeting.PickupTag)) {
            return new TargetResult {
                Position = GameInputManager.Instance.MouseWorldPosition,
                TargetEntity = entity
            };
        }
        return new();
    }
}

public class ClosestToMouseSelection : SelectionTargeting {
    Collider2D prevClosest;
    Material defaultMaterial;

    public ClosestToMouseSelection(Targeting targeting) : base(targeting) { }

    public override void Preview() {
        Collider2D[] hits = Physics2D.OverlapCircleAll(GameInputManager.Instance.MouseWorldPosition, targeting.TargetRadius);
        Collider2D closest = null;
        float closestDist = float.MaxValue;

        foreach (var hit in hits) 
            if (hit.CompareTag(targeting.PickupTag)) {
                float dist = Vector2.Distance(GameInputManager.Instance.MouseWorldPosition, hit.transform.position);

                if (dist < closestDist) {
                    closest = hit;
                    closestDist = dist;
                }
            }

        if (prevClosest != closest)
            SetHighlight(closest);
        base.Preview();
    }

    public override TargetResult Execute() {
        TargetResult result = new() {
            Position = GameInputManager.Instance.MouseWorldPosition,
        };

        if (prevClosest != null) {
            result.Position = prevClosest.transform.position;
            result.TargetEntity = prevClosest.GetComponent<BaseEntity>();
            
            SetHighlight(null);
        }
        return result;
    }

    void SetHighlight(Collider2D col) {
        if (prevClosest != null) {
            var sr = prevClosest.GetComponent<BaseEntity>().rend;
            sr.material = defaultMaterial;
        }

        if (col != null) {
            var sr = col.GetComponent<BaseEntity>().rend;
            defaultMaterial = sr.material;
            sr.material = targeting.OutlineMaterial;
        }
        prevClosest = col;
    }
}

//public class AreaAroundMouseSelection : SelectionOperation {
//    public AreaAroundMouseSelection(Targeting targeting) : base(targeting) { }

//    public override void Preview() {
//        /// GUI to draw here
//        base.Preview();
//    }

//    public override bool Execute() {
//        //Vector3 pos = targeting.Transform.position;
//        //Collider2D[] hits = Physics2D.OverlapCircleAll(pos, areaRadius);

//        //foreach (var hit in hits) {
//        //    if (hit.CompareTag(targeting.PickupTag)) {
//        //        BaseEntity entity = hit.GetComponent<BaseEntity>();

//        //        if (entity != null)
//        //            PickupSelectionFactory.Execute(entity, areaRadius);
//        //    }
//        //}
//        return true;
//    }
//}

//public class AttractionSelection : SelectionOperation {
//    HashSet<BaseEntity> entities = new HashSet<BaseEntity>();

//    public AttractionSelection(Targeting targeting) : base(targeting) { }

//    public override void Preview() {
//        // GUI to draw here
//        base.Preview();
//    }

//    public override bool Execute() {
//        Vector3 pos = targeting.Transform.position;
//        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, targeting.TargetRadius);

//        foreach (var hit in hits) {
//            //skips if not entity, no tag match, and already picked up
//            if (!hit.TryGetComponent<BaseEntity>(out var entity) ||
//                !entity.CompareTag(targeting.PickupTag) ||
//                entities.Contains(entity))
//                continue;

//            if (entity is IAttractable attractee) {
//                attractee.SetOverrideTarget(targeting.Transform);
//                Action<BaseEntity> handler = e => {
//                    if (e == null || e.GUID == entity.GUID)
//                        attractee.OnAttractionEnd();
//                };
//                targeting.OnPickUp += handler;
//            }
//            entities.Add(entity);
//        }
//        return base.Execute();
//    }
//}
