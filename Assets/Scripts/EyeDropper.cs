using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EyeDropper : MonoBehaviour, IAbilityHandler {
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

    Dictionary<string, EyeDroppables> storedEntities = new();
    EyeDroppables currentSelection;

    public float RemainingUsage => remainingUsage;
    public float TotalUsage => currentSelection.maxUsage;

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
                storedEntities[entity.GUID] = entry;
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
            remainingUsage = currentSelection.maxUsage;
            return true;
        }
        return false;
    }

    void Paste() {
        var obj = Instantiate(currentSelection.prefab, GameInputManager.Instance.MouseWorldPosition, Quaternion.identity);

        OnPickUp?.Invoke(obj.GetComponent<BaseEntity>());

        if (--remainingUsage <= 0) {
            OnAbilityEnd?.Invoke();
            currentSelection = null;
            Destroy(gameObject);
        }
    }
}
