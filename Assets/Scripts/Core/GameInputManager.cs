using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameInputManager : MonoBehaviour {
    public static GameInputManager Instance { get; private set; }
    public bool InputLocked { get; set; }

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update() {
        if (IsKeyDown(KeyCode.Escape)) GameEvents.RaisePausePressed();
    }

    /// Returns true if pointer over any UI
    public bool IsPointerOverUI() {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    /// Returns true if left click in world (ignores UI)
    public bool WorldClick() {
        return !InputLocked && Input.GetMouseButtonDown(0) && !IsPointerOverUI();
    }

    /// Right click in world
    public bool WorldRightClick() {
        return !InputLocked && Input.GetMouseButtonDown(1) && !IsPointerOverUI();
    }

    /// Any key pressed this frame
    public bool IsKeyDown(KeyCode key) {
        return !InputLocked && Input.GetKeyDown(key);
    }

    /// Any key held
    public bool IsKeyPressed(KeyCode key) {
        return !InputLocked && Input.GetKey(key);
    }

    /// Any key released this frame
    public bool IsKeyReleased(KeyCode key) {
        return !InputLocked && Input.GetKeyUp(key);
    }

    /// Mouse position in world space
    public Vector3 MouseWorldPosition {
        get {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0f;
            return pos;
        }
    }

    /// Mouse position in screen space
    public Vector2 MouseScreenPosition => Input.mousePosition;

    /// Scroll delta
    public float ScrollDelta => Input.mouseScrollDelta.y;
}