using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFirePointStrategy {
    IEnumerable<(Vector3 pos, Quaternion rot)> GetFirePoints(Transform firePoints);
}

public class SinglePointFire : IFirePointStrategy {
    public IEnumerable<(Vector3 pos, Quaternion rot)> GetFirePoints(Transform firePoints) {
        yield return (firePoints.position, firePoints.rotation);
    }
}
