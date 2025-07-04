using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBoundsClamp : MonoBehaviour {
    public Camera cam;
    public float padding = 0.5f;

    private float minX, minY, maxX, maxY;

    private void Awake() {
        cam = cam != null ? cam : Camera.main;
    }

    private void LateUpdate() {
        float vertExtent = cam.orthographicSize;
        float horizExtent = vertExtent * Screen.width / Screen.height;

        minX = cam.transform.position.x - horizExtent + padding;
        maxX = cam.transform.position.x + horizExtent - padding;
        minY = cam.transform.position.y - vertExtent + padding;
        maxY = cam.transform.position.y + vertExtent - padding;

        Vector3 clampPos = transform.position;
        clampPos.x = Mathf.Clamp(clampPos.x, minX, maxX);
        clampPos.y = Mathf.Clamp(clampPos.y, minY, maxY);
        transform.position = clampPos;
    }
}
