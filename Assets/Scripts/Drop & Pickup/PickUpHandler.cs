using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class PickupHandler : MonoBehaviour {
    protected SpriteRenderer rend;
    protected CountdownTimer lifeTimer;
    protected int remainingUsage;
    protected DropOperation dropOperation;
    protected bool dropped;

    public PickupType PickupType { get; protected set; }
    public abstract IPickupData Data { get; }
    public string PickupTag => Data.PickupTag;

    // Event here
    public event Action Dropped = delegate { };

    protected virtual void Awake() {
        rend = GetComponent<SpriteRenderer>();

        /// for any prebuilt pickup object. it will be existed as a dropped objcet instead.
        /// allows for placing pickup object scene! ^-^
        if (Data != null)
            Init(Data, true);
        else
            Debug.LogWarning($"{name} has no pickup data in Start(). Visuals will not be set until Init() is called.");
    }

    private void Start() {
        dropOperation = DropOperationFactory.GetOperation(this, Data);
        lifeTimer.Start();
    }

    void Update() {
        if (!dropped) {
            if (Input.GetMouseButtonDown(0)) {
                dropped = true;
                Dropped.Invoke();
            } else
                dropOperation.Preview();
        } else
            dropOperation.Execute();
        lifeTimer.Tick(Time.deltaTime);
    }

    public virtual void Init(IPickupData data, bool dropIt = false) {
        if (data == null)
            throw new NullReferenceException("Missing data here dawg!");
        if (data.PickupType != PickupType)
            throw new Exception("Mismatched Drop Item and Dropper");
        /// Drop lifespan behavior:
        /// - If LifeTime <0: defaults to 5 seconds.
        /// - If TotalUsage <0: defaults to 1 usage.
        /// - Both >=0: expires when either time runs out or usage is exhausted.
        float lifetime = data.LifeTime < 0 ? 5f : data.LifeTime;
        int usage = data.TotalUsage < 0 ? 1 : data.TotalUsage;
  
        lifeTimer = new CountdownTimer(lifetime);
        remainingUsage = usage;

        lifeTimer.OnTimerStop += () => Destroy(gameObject);

        dropped = dropIt;

        rend.sprite = data.PickupIcon;
        rend.sortingLayerName = "Pickups";
        rend.sortingOrder = 0;
    }

    public void OnTriggerEnter2D(Collider2D other) {
        if (!dropped) return;
        if (other != null && other.CompareTag(Data.PickupTag))
            if (PickupOperationFactory.Execute(other.GetComponent<BaseEntity>(), Data))
                if (--remainingUsage <= 0) Destroy(gameObject);
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
            gameObject.name = $"Pickup_{Data.DisplayName}";
    }
#endif
}
