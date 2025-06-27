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

    protected SpriteRenderer rend;
    protected int remainingUsage;

    bool dropped;

    public PickupType PickupType { get; protected set; }
    protected virtual IPickupDefinition Definition { get; private set; }

    // Event here
    public event Action Dropped = delegate { };

    protected abstract void Awake();

    private void Start() {
        rend = GetComponent<SpriteRenderer>();
        SetVisual();

        /// for any prebuilt pickup object. it will be existed as a dropped objcet instead.
        /// allows for placing pickup object scene! ^-^
        if(Definition != null) dropped = true;
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

    public virtual void Init(bool dropIt = false) {
        Init(Definition, dropIt);
    }

    public virtual void Init(IPickupDefinition definition, bool dropIt = false) {
        if (definition == null)
            throw new NullReferenceException("Missing definition here dawg!");
        if (definition.PickupType != PickupType)
            throw new Exception("Mismatched Drop Item and Dropper");
        this.Definition = definition;
        remainingUsage = definition.DropCount;
    }

    void SetVisual() {
        rend.sprite = Definition.PickupIcon;
        rend.sortingLayerName = "Pickups";
        rend.sortingOrder = 0;
    }

    protected abstract void PickUp(BaseEntity entity);
    
    public void Accept<T>(T visitor) where T : Component, IVisitor {
        if (visitor is BaseEntity entity)
            PickUp(entity);
    }

    public void OnTriggerEnter2D(Collider2D other) {
        if (!dropped) return;
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
