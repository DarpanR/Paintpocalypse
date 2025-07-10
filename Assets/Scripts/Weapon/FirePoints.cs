using System;
using System.Collections.Generic;
using UnityEngine;

public enum FirePointBehaviorType {
    Straight,
    RandomArc,
    Revolving,
    Arc,
}

[Serializable]
public struct FirePoint {
    public Vector3 position;
    public float angle;
}

public interface IFirePointBehavior {
    void Tick(float deltaTime);
    IEnumerable<FirePoint> GetFirePoints(Transform origin, FirePoint firePoint, int count);
}

public class StraightFirePoint : IFirePointBehavior {
    public IEnumerable<FirePoint> GetFirePoints(Transform origin, FirePoint firePoint, int count) {
        for (int i = 0; i < count; i++) {
            yield return new FirePoint {
                position = firePoint.position + origin.position,
                angle = firePoint.angle + origin.eulerAngles.z
            };
        }
    }

    public void Tick(float deltaTime) { }
    
}

public class RandomArcFirePoint : IFirePointBehavior {
    float arcAngle;

    public RandomArcFirePoint(float arcAngle) {
        this.arcAngle = arcAngle;
    }

    public IEnumerable<FirePoint> GetFirePoints(Transform origin, FirePoint firePoint, int count) {
        for (int i = 0; i < count; i++) {
            float spread = UnityEngine.Random.Range(-arcAngle / 2f, arcAngle / 2f);
            
            yield return new FirePoint {
                position = firePoint.position + origin.position,
                angle = firePoint.angle + origin.eulerAngles.z + spread
            };
        }
    }

    public void Tick(float deltaTime) {}
}

/// <summary>
/// Dont use this one yet.
/// </summary>
public class ArcFirePoint : IFirePointBehavior {
    float arcAngle;
    float projectileCount;
    int direction = 0;
    int swingStep = 0;

    public ArcFirePoint(float arcAngle) {
        this.arcAngle = arcAngle;
    }

    public IEnumerable<FirePoint> GetFirePoints(Transform origin,FirePoint firePoint, int count) {
        int totalSteps = Mathf.Max(count, 2);
        float stepSize = arcAngle / (totalSteps - 1);
        float sweepOffset = -arcAngle / 2f + stepSize * swingStep;

        for (int i = 0; i < count; i++) {
            float localAngle = -arcAngle / 2f + stepSize * i;
            float totalAngle = sweepOffset + localAngle;

            yield return new FirePoint {
                position = origin.position + firePoint.position,
                angle = origin.eulerAngles.z + firePoint.angle + totalAngle
            };
        }
        // Advance swing step
        swingStep += direction;
        if (swingStep >= totalSteps - 1) {
            swingStep = totalSteps - 1;
            direction = -1;
        } else if (swingStep <= 0) {
            swingStep = 0;
            direction = 1;
        }
    }

    public void Tick(float deltaTime) { }
}

public class RevolvingFirePoint : IFirePointBehavior {
    float rotationSpeed;
    float curAngle;

    public RevolvingFirePoint(float rotationalSpeed) {
        this.rotationSpeed = rotationalSpeed;
    }

    public IEnumerable<FirePoint> GetFirePoints(Transform origin, FirePoint firePoint, int count) {
        for (int i = 0; i < count; i++) {
            float radius = firePoint.position.magnitude;
            float baseAngle = Mathf.Atan2(firePoint.position.y, firePoint.position.x) * Mathf.Rad2Deg;
            float totalAngle = baseAngle + curAngle;

            Vector2 localOffset = Quaternion.Euler(0, 0, totalAngle) * Vector2.right * radius;

            yield return new FirePoint {
                position = origin.position + (Vector3)localOffset,
                angle = totalAngle + firePoint.angle + origin.eulerAngles.z
            };
        }
    }

    public void Tick(float deltaTime) {
        curAngle += rotationSpeed * deltaTime;
        curAngle %= 360;
    }
}
