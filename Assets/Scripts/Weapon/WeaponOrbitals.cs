using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public enum OrbitalRingLevels {
    FollowsRotations = 1,
    AutoTargeting = 2, 
    Orbiting = 3
}

public interface IOrbitalBehavior {
    List<FirePoint> GetFirePoints(Transform origin, List<FirePoint> firepoints, IFirePointBehavior behavior, int count);
}

public class PlayerAimRing : IOrbitalBehavior {
    public List<FirePoint> GetFirePoints(Transform origin, List<FirePoint> firePoints, IFirePointBehavior behavior, int count) {
        foreach (FirePoint firePoint in firePoints) {
            yield return behavior.GetFirePoints(origin, firePoint, count);
        }
    }
}
