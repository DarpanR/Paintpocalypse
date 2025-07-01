using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EyeDropper : MonoBehaviour, IAbililtyHandler {
    [System.Serializable]
    public class EyeDroppables {
        public GameObject prefab;
        public int maxUsage;
    }

    public Material outlineMaterial;
    public TargetingType selectorType = TargetingType.ClosestToMouse;
    [TagMaskField]
    public string pickupTag = "Untagged";
    public float radius;
    public List<EyeDroppables> availableEntities = new();

    int remainingUsage = 5;
    DropOperation SelectOp;
    bool hasSelection;

    Dictionary<string, GameObject> storedEntities = new();
    Dictionary<string, int> maxUsages = new();
    GameObject currentSelection;

    public event Action<BaseEntity> OnPickUp;
    public event Action OnAbilityEnd;

    Targeting targeting;

    private void Start() {
        SelectOp = TargetOperationFactory.GetOperation(new Targeting {
            Transform = transform,
            TargetingType = selectorType,
            PickupTag = pickupTag,
            OutlineMaterial = outlineMaterial,
            TargetRadius = radius,
        });

        foreach (var entry in availableEntities) {
            if (entry.prefab.TryGetComponent<BaseEntity>(out var entity)) {
                storedEntities[entity.GUID] = entry.prefab;
                maxUsages[entity.GUID] = entry.maxUsage;
            }
        }
    }

    private void Update() {
        if (GameInputManager.Instance.WorldClick()) {
            if (!hasSelection) {
                var result = SelectOp.Execute();

                hasSelection = Copy(result.TargetEntity);
            } else
                Paste();
        } else if (!hasSelection)
            SelectOp.Preview();
    }

    public void Init(IPickupData pickupData, bool dropIt = false) { }

    bool Copy(BaseEntity copyTarget) {
        if (copyTarget != null && storedEntities.TryGetValue(copyTarget.GUID, out var copy)) {
            currentSelection = copy;
            remainingUsage = maxUsages[copyTarget.GUID];
            return true;
        }
        return false;
    }

    void Paste() {
        var obj = Instantiate(currentSelection, GameInputManager.Instance.MouseWorldPosition, Quaternion.identity);

        OnPickUp?.Invoke(obj.GetComponent<BaseEntity>());

        if (--remainingUsage <= 0) {
            OnAbilityEnd?.Invoke();
            Destroy(gameObject);
        }
    }
}
