using UnityEngine;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]

public abstract class PickupHandler : MonoBehaviour, IAbilityHandler {
    public Material outlineMaterial;
    public float rotationalSpeed = 45f;
    
    protected SpriteRenderer rend;
    protected CountdownTimer lifeTimer;
    protected int remainingUsage;
    protected DropOperation dropOp;
    protected bool dropped;
    protected HashSet<BaseEntity> AlreadyTriggered = new(); 

    public abstract IPickupData Data { get; }
    public PickupType PickupType { get; protected set; }

    public float RemainingUsage => remainingUsage;
    public float TotalUsage => Data.TotalUsage;

    // Event here
    public event Action OnDropped = delegate { };
    public event Action OnAbilityEnd;

    protected virtual void Awake() {
        rend = GetComponent<SpriteRenderer>();

        /// for any prebuilt pickup object. it will be existed as a dropped objcet instead.
        /// allows for manually placing pickup object scene! ^-^
        if (Data != null)
            Init(Data, true);
    }

    private void Start() {
        if (Data == null)
            Debug.LogWarning($"{name} has no pickup data in Start(). Visuals will not be set until Init() is called.");
        dropOp = TargetOperationFactory.GetOperation(new Targeting {
            Transform = transform,
            TargetingType = Data.TargetingType,
            PickupTag = Data.PickupTag,
            OutlineMaterial = outlineMaterial,
            TargetRadius = Data.TargetRadius,
        });

        lifeTimer.OnTimerStop += () => Destroy(gameObject);
        OnDropped += lifeTimer.Start;
    }

    void Update() {
        if (!dropped) {
            if (GameInputManager.Instance.WorldClick()) {
                /// get results and do things with it?
                var result = dropOp.Execute();

                dropped = result.success;
                transform.position = result.Position ?? transform.position;
                TriggerPickUp(result.TargetEntity);

                SetVisuals();
                FinalizeDrop();
            } else 
                dropOp.Preview();
        } else {
            lifeTimer.Tick(Time.deltaTime);
            dropOp.Execute();
        }
        UpdateVisuals();
    }

    public void OnTriggerEnter2D(Collider2D other) {
        if (!dropped) return;
        if (other != null && other.TryGetComponent<BaseEntity>(out var entity)) {
            TriggerPickUp(entity);
        }
    }

    public void OnTriggerExit2D(Collider2D other) {
        if (other.TryGetComponent<BaseEntity>(out var entity))
            AlreadyTriggered.Remove(entity);
    }

    public void OnDestroy() {
        dropOp.InvokeOnPickUp(null);
    }

    public virtual void Init(IPickupData data, bool dropIt = false) {
        if (data == null)
            throw new NullReferenceException("Missing data here dawg!");
        if (data.PickupType != PickupType)
            throw new Exception("Mismatched Drop Item and Dropper");
        dropped = dropIt;
        /// Drop lifespan behavior:
        /// - If LifeTime <0: defaults to 5 seconds.
        /// - If TotalUsage <0: defaults to 1 usage.
        /// - Both >=0: expires when either time runs out or usage is exhausted.
        float lifetime = Data.LifeTime < 0 ? 5f : Data.LifeTime;
        int usage = Data.TotalUsage < 0 ? 1 : Data.TotalUsage;

        lifeTimer = new CountdownTimer(lifetime);
        remainingUsage = usage;

        rend.sortingLayerName = "Pickups";
        rend.sortingOrder = 0;
        SetVisuals();
        FinalizeDrop();
    }

    void SetVisuals() => rend.sprite = dropped ? Data.DropIcon : Data.PickupIcon;
    
    protected virtual void UpdateVisuals() {
        if (dropped)
            transform.Rotate(Vector3.up, rotationalSpeed * Time.deltaTime);  
    }

    void TriggerPickUp(BaseEntity entity) {
        if (entity == null || AlreadyTriggered.Contains(entity)) return;
        AlreadyTriggered.Add(entity);
        dropOp.InvokeOnPickUp(entity);

        if (remainingUsage > 0 && PickupDropFactory.Execute(entity, Data))
            if (--remainingUsage <= 0)
                Destroy(gameObject);
    }

    void FinalizeDrop() {
        if (!dropped) return;
        OnDropped?.Invoke();
        OnAbilityEnd?.Invoke();
    }

    // (Optional) Draw a little gizmo in the editor so you can see
    // which data is assigned without running the game.
#if UNITY_EDITOR
    void OnDrawGizmosSelected() {
        if (Data != null && Data.PickupIcon) {
            Gizmos.DrawIcon(transform.position,
                AssetDatabase.GetAssetPath(Data.PickupIcon), true);
        }
    }

    private void OnValidate() {
        if (Data != null)
            gameObject.name = $"Pickup_{Data.PickupName}";
    }
#endif
}