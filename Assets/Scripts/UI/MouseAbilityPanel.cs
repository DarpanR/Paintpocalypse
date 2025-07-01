using System;
using System.Collections.Generic;
using UnityEngine;

public class MouseAbilityPanel : MonoBehaviour {
    [SerializeField]
    List<AbilityConfig> abilities;

    public const string None = "none";

    /// abilityName should be unique, e.g.,
    /// "Eyedropper", "MagnifyingGlass", "Pencil"
    string activeName = None;
    GameObject activeObject;
    //public GameObject textBomb;

    Action OnDropHandler;

    [Serializable]
    public struct AbilityConfig {
        public string abilityName;
        public Texture2D icon;
        public GameObject prefab;
        public ScriptableObject pickupData;
    }

    private void Start() {
        abilities.Insert(0, new AbilityConfig {
            abilityName = None,
            icon = null,
            prefab = null,
            pickupData = null
        });

        for (int i = 1; i < abilities.Count; i++) {
            var ability = abilities[i];

            // If ability name is empty, try to pull from pickupData
            if (string.IsNullOrWhiteSpace(ability.abilityName)) {
                if (ability.pickupData is IPickupData pickupData) {
                    if (!string.IsNullOrWhiteSpace(pickupData.PickupName)) {
                        ability.abilityName = pickupData.PickupName;
                    } else {
                        throw new Exception("PickupData has no PickupName!");
                    }
                } else {
                    throw new Exception("Ability has no name and no valid IPickupData to derive a name from!");
                }
            }
            ability.abilityName = ability.abilityName?.ToLowerInvariant();

            if (ability.prefab == null)
                throw new Exception($"{ability.abilityName} is missing a prefab.");
            if (ability.pickupData != null && ability.pickupData is not IPickupData)
                throw new Exception($"Bruh this ScriptableObject in ability '{ability.abilityName}' ain't an IPickupData!");
        }
        OnDropHandler += () => ActivateAbility(abilities[0]);
        OnDropHandler?.Invoke();
    }

    void ActivateAbility(AbilityConfig config) {
        if (config.abilityName == activeName || config.abilityName == None) {
            //Deactivate Current
            DeactivateAbility(config.abilityName);
        } else if (config.abilityName != None) {
            DeactivateAbility(config.abilityName);
            activeObject = Instantiate(config.prefab, GameInputManager.Instance.MouseWorldPosition, Quaternion.identity);

            if (activeObject.TryGetComponent<IAbililtyHandler>(out var handler)) {
                if (config.pickupData is IPickupData pData)
                    handler.Init(pData);
                handler.OnAbilityEnd += OnDropHandler;
                activeName = config.abilityName;

                Cursor.SetCursor(config.icon, Vector2.zero, CursorMode.Auto);
            } else {
                Debug.LogWarning("Missing AbililtyHandler component!");
                activeName = None;
            }
        }
    }

    void DeactivateAbility(string configName) {
        if (activeObject != null) {
            if (activeObject.TryGetComponent<IAbililtyHandler>(out var handler))
                handler.OnAbilityEnd -= OnDropHandler;
            if (configName != None) Destroy(activeObject);
            activeObject = null;
            activeName = None;

            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }

    public void OnAbilityClicked(int index) {
        ActivateAbility(abilities[index]);
    }
}
