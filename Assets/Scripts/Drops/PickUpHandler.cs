using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class PickupHandler : MonoBehaviour, IVisitable {
    [Tooltip("Degrees per second")]
    public float rotationSpeed = 45f;
    protected virtual IPickupDefinition Definition { get; private set; }
    protected SpriteRenderer sr;
    protected int remainingUsage;

    bool dropped;
    
    // Event here
    public event Action Dropped = delegate { };

    public virtual void Init() {
        Init(Definition);
    }

    public virtual void Init(IPickupDefinition definition) {
        Definition = definition;
        dropped = false;
        remainingUsage = definition.Amount;
    }

    private void Awake() {
        sr = GetComponent<SpriteRenderer>();
        SetVisual();
    }

    void SetVisual() {
        sr.sprite = Definition.PickupIcon;
        sr.sortingLayerName = "Pickups";
        sr.sortingOrder = 0;
    }

    void Update() {
        if (!dropped) {
            if (Input.GetMouseButtonDown(0)) {
                dropped = true;
                Dropped.Invoke();
            } else {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0f;
                transform.position = mousePos;
            }
        } else
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0f);
    }

    protected abstract void PickUp(BaseEntity entity);
    
    public void Accept<T>(T visitor) where T : Component, IVisitor {
        if (visitor is BaseEntity entity)
            PickUp(entity);
    }

    public void OnTriggerEnter2D(Collider2D other) {
        other.GetComponent<IVisitor>()?.Visit(this);
    }

    // (Optional) Draw a little gizmo in the editor so you can see
    // which definition is assigned without running the game.
#if UNITY_EDITOR
    void OnDrawGizmosSelected() {
        if (Definition != null && Definition.PickupIcon) {
            Gizmos.DrawIcon(transform.position,
                AssetDatabase.GetAssetPath(Definition.PickupIcon), true);
        }
    }
#endif
}
