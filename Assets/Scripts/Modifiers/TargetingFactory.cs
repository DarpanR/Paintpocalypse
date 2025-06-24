using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TargetingMode { TargetUnderMouse, RadiusAroundMouse, Self, NearestEnemy}
public static class TargetingFactory {
    public static List<BaseEntity>GetTargets(TargetingMode mode, Vector3 origin, string targetTag) {
        return GetTargets(mode, origin, targetTag, 1f);
    }

    public static List<BaseEntity> GetTargets(TargetingMode mode, Vector3 origin, string targetTag, float radius) {
        switch (mode) {
            case TargetingMode.TargetUnderMouse:
                return TargetUnderMouse(origin, targetTag);
            case TargetingMode.RadiusAroundMouse:
                return RadiusAroundMouse(origin, targetTag, radius); // or make radius configurable
            case TargetingMode.Self:
                return new List<BaseEntity> { GetSelf() };
            case TargetingMode.NearestEnemy:
                return NearestEnemy(origin, targetTag, radius);
            default:
                return new List<BaseEntity>();
        }
    }

    static List<BaseEntity> TargetUnderMouse(Vector3 origin, string targetTag) {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
        
        if (hit.collider != null && hit.collider.TryGetComponent(out BaseEntity entity))
            if (entity.CompareTag(targetTag))
                return new List<BaseEntity> { entity };
        return new List<BaseEntity>();
    }

    static List<BaseEntity> RadiusAroundMouse(Vector3 origin, string targetTag, float radius) {
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, radius, LayerMask.GetMask("Entity"));
        return hits.Select(h => h.GetComponent<BaseEntity>()).Where(e => e != null && e.CompareTag(targetTag)).ToList();
    }

    static List<BaseEntity> NearestEnemy(Vector3 origin, string targetTag, float radius) {
        var entities = RadiusAroundMouse(origin, targetTag, radius);
        BaseEntity nearest = null;
        float closestDist = Mathf.Infinity;

        foreach (var e in entities) {
            float dist = Vector3.Distance(origin, e.transform.position);
            if (dist < closestDist) {
                closestDist = dist;
                nearest = e;
            }
        }
        return nearest != null ? new List<BaseEntity> { nearest } : new List<BaseEntity>();
    }

    static BaseEntity GetSelf() {
        // Implement contextually: e.g., mouse player or selected entity
        return GameObject.FindWithTag("MousePlayer")?.GetComponent<BaseEntity>();
    }
}
