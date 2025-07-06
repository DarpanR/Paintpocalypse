using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EyeDropper : MonoBehaviour, IAbilityHandler {
    [System.Serializable]
    public struct EyeDroppables {
        public EntityData EntityData;
        public int maxUsage;
    }

    public Material outlineMaterial;
    public TargetingType selectorType = TargetingType.ClosestToMouse;
    [TagMaskField]
    public string pickupTag = "Untagged";
    public float radius;
    public List<EyeDroppables> availableEntities = new();

    int remainingUsage = 1;
    DropOperation SelectOp;
    bool hasSelection;

    Dictionary<string, EyeDroppables> storedEntities = new();
    EyeDroppables currentSelection;

    public const string None = "None";

    public float RemainingUsage => remainingUsage;
    public float TotalUsage => currentSelection.maxUsage;

    public event Action<BaseEntity> OnPickUp;
    public event Action OnAbilityEnd;

    private void Start() {
        SelectOp = TargetOperationFactory.GetOperation(new Targeting {
            Transform = transform,
            TargetingType = selectorType,
            PickupTag = pickupTag,
            OutlineMaterial = outlineMaterial,
            TargetRadius = radius,
        });

        /// empty selection
        currentSelection = storedEntities[None] = new EyeDroppables {
            maxUsage = 1
        };

        foreach (var entry in availableEntities) {
            storedEntities[entry.EntityData.GUID] = entry;
        }
    }

    private void Update() {
        if (GameInputManager.Instance.WorldClick()) {
            if (!hasSelection) {
                var result = SelectOp.Execute();

                hasSelection = Copy(result.TargetEntity);
            } else
                Paste();
        } 
        SelectOp.Preview();
    }

    public void Init(IPickupData pickupData, bool dropIt = false) { }

    bool Copy(BaseEntity copyTarget) {
        if (copyTarget != null && storedEntities.TryGetValue(copyTarget.GUID, out var copy)) {
            currentSelection = copy;
            remainingUsage = currentSelection.maxUsage;
            return true;
        }
        currentSelection = storedEntities[None];
        remainingUsage = 1;
        return false;
    }

    void Paste() {
        var go = EntityManager.Instance.Spawn(currentSelection.EntityData.GUID, GameInputManager.Instance.MouseWorldPosition);

        OnPickUp?.Invoke(go.GetComponent<BaseEntity>());

        if (--remainingUsage <= 0) {
            OnAbilityEnd?.Invoke();
            currentSelection = storedEntities[None];
            Destroy(gameObject);
        }
    }
}
