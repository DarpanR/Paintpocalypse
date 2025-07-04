using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalTransformLock : MonoBehaviour {
    public bool lockPosition;
    public bool lockRotation;
    public bool lockScale;

    Transform parent;
    Vector3 position;
    Quaternion rotation;
    Vector3 scale;

    void Awake() {
        parent = transform.parent;
        position = transform.localPosition;
        rotation = transform.localRotation;
        scale = transform.localScale;

        LateUpdate();
    }

    void LateUpdate() {
        if (lockPosition) LockPosition();
        if (lockRotation) LockRotation();
        if (lockScale) LockScale();
    }

    void LockPosition() {
        transform.position = parent.position + position;
    }

    void LockRotation() {
        transform.rotation = rotation;
    }

    void LockScale() {
        transform.localScale = scale;
    }
}