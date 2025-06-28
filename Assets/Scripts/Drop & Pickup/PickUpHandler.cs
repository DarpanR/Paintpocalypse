using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;


#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class PickupHandler : MonoBehaviour {
    [Tooltip("Degrees per second")]
    public float rotationSpeed = 45f;

    protected SpriteRenderer rend;
    protected int remainingUsage;
    protected bool dropped;

    public PickupType PickupType { get; protected set; }
    protected abstract IPickupDefinition Definition { get; }

    // Event here
    public event Action Dropped = delegate { };

    protected virtual void Awake() {
        rend = GetComponent<SpriteRenderer>();
       
    }

    private void Start() {
        /// for any prebuilt pickup object. it will be existed as a dropped objcet instead.
        /// allows for placing pickup object scene! ^-^
        if (Definition != null)
            Init(Definition, true);
        else
            Debug.LogWarning($"{name} has no pickup definition in Start(). Visuals will not be set until Init() is called.");
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

    public virtual void Init(IPickupDefinition definition, bool dropIt = false) {
        if (definition == null)
            throw new NullReferenceException("Missing definition here dawg!");
        if (definition.PickupType != PickupType)
            throw new Exception("Mismatched Drop Item and Dropper");
        remainingUsage = definition.DropCount;
        dropped = dropIt;

        rend.sprite = definition.PickupIcon;
        rend.sortingLayerName = "Pickups";
        rend.sortingOrder = 0;
    }

    protected virtual void PickUp(BaseEntity entity) {
        if (remainingUsage <= 0) Destroy(gameObject);
    }

    public void OnTriggerEnter2D(Collider2D other) {
        if (!dropped) return;
        if (other != null && other.CompareTag(Definition.PickupTag)) 
            PickUp(other.GetComponent<BaseEntity>());
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
    private void OnValidate() {
        if (Definition != null)
            gameObject.name = $"Pickup_{Definition.DisplayName}";
    }
#endif
}
