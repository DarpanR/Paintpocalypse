using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MouseAbilityPanel : MonoBehaviour {
    [SerializeField]
    float globalCD = 1f;
    [SerializeField]
    float cdOffset = .1f;
    [SerializeField]
    List<AbilityConfig> abilities;
    [SerializeField]
    public Transform buttonContainer;
    [SerializeField]
    public GameObject buttonPrefab;


    public const string None = "none";
    /// AbilityName should be unique, e.g.,
    /// "Eyedropper", "MagnifyingGlass", "Pencil"
    int activeIndex = 0;
    GameObject activeObject;

    List<CooldownData> Cooldowns = new();

    //public GameObject textBomb;

    Action OnAbilityUsed;

    [Serializable]
    public struct AbilityConfig {
        public string AbilityName;
        public float Cooldown;
        public Texture2D Icon;
        public GameObject Prefab;
        public ScriptableObject PickupData;
    }

    public struct CooldownData {
        public CountdownTimer Timer;
        public Image backgroundImage;
    }

    private void Awake() {
        buttonContainer = buttonContainer != null ? buttonContainer : transform.Find("Action Button Container");

        if (buttonContainer == null) {
            buttonContainer = new GameObject("Action Button Container", typeof(RectTransform)).transform;
            buttonContainer.SetParent(transform, false);
        }
    }

    private void Start() {
        abilities.Insert(0, new AbilityConfig {
            AbilityName = None,
            Icon = null,
            Prefab = null,
            PickupData = null
        });

        // loop through to find any mistakes and create button
        // Important: Cooldowns[0] corresponds to abilities[1], etc.
        // Cooldowns[i - 1] = cooldown for abilities[i]
        for (int i = 1; i < abilities.Count; i++) {
            var ability = abilities[i];

            // If ability name is empty, try to pull from PickupData
            if (string.IsNullOrWhiteSpace(ability.AbilityName)) {
                if (ability.PickupData is IPickupData PickupData) {
                    if (!string.IsNullOrWhiteSpace(PickupData.PickupName)) {
                        ability.AbilityName = PickupData.PickupName;
                    } else {
                        throw new Exception("PickupData has no PickupName!");
                    }
                } else {
                    throw new Exception("Ability has no name and no valid IPickupData to derive a name from!");
                }
            }
            // set all names to lowercase
            ability.AbilityName = ability.AbilityName?.ToLowerInvariant();

            if (ability.Prefab == null)
                throw new Exception($"{ability.AbilityName} is missing a Prefab.");
            if (ability.PickupData != null && ability.PickupData is not IPickupData)
                throw new Exception($"Bruh this ScriptableObject in ability '{ability.AbilityName}' ain't an IPickupData!");

            /// create visuals 
            /// create ability button and assign listener
            var buttonGO = Instantiate(buttonPrefab, buttonContainer);
            buttonGO.name = ability.AbilityName;

            /// Ability Icon/Image
            var Icon = buttonGO.GetComponent<Image>();
            Debug.Log(Icon.ToString());
            if (ability.Icon != null) {
                Icon.sprite = Sprite.Create(
                    ability.Icon,
                    new Rect(0, 0, ability.Icon.width, ability.Icon.height),
                    new Vector2(0.5f, 0.5f)
                );
            }

            /// Coolddown Timer visual
            var cdBG = buttonGO.transform.Find("Fill Background").GetComponent<Image>();
            var timer = new CountdownTimer(globalCD);
            var aButton = buttonGO.GetComponent<Button>();

            Cooldowns.Add(new CooldownData { Timer = timer, backgroundImage = cdBG });

            /// add actions
            timer.OnTimerStart += () => aButton.interactable = false;
            timer.OnTimerStop += () => aButton.interactable = true;
            aButton.onClick.AddListener(() => ActivateAbility(i));
        }
        // activates defualt None
        OnAbilityUsed += () => ActivateAbility(0);
        //OnAbilityUsed?.Invoke();
    }

    private void Update() {
        foreach (var cd in Cooldowns) {
            /// ticks if Timer started
            cd.Timer.Tick(Time.deltaTime);
            cd.backgroundImage.fillAmount = cd.Timer.Progress;
        }
    }

    void ActivateAbility(int newIndex) {
        var activeAbility = abilities[activeIndex];
        var config = abilities[newIndex];


        //Deactivates old ability if clicked on the same ability, or if ability has ended (by passing in none(index = 0))
        if (config.AbilityName == activeAbility.AbilityName || config.AbilityName == None) {
            DeactivateAbility(config.AbilityName);
            // else activate new ability and deactivate all old unsed ability
        } else {
            DeactivateAbility(config.AbilityName);
            activeObject = Instantiate(config.Prefab, GameInputManager.Instance.MouseWorldPosition, Quaternion.identity);

            if (activeObject.TryGetComponent<IAbilityHandler>(out var ability)) {
                if (config.PickupData is IPickupData pData)
                    ability.Init(pData);
                ability.OnAbilityEnd += OnAbilityUsed;
                activeIndex = newIndex;

                Cursor.SetCursor(config.Icon, Vector2.zero, CursorMode.Auto);
            } else {
                Debug.LogWarning("Missing Abililtyability component!");
                activeIndex = 0;
            }

            /// start a global cooldown for all abilities except the currently active.
            for (int i = 0; i < abilities.Count; i++) {
                var cd = Cooldowns[i - 1];
                if (i != activeIndex)
                    cd.Timer.Reset(globalCD);
                cd.Timer.Start();
            }
        }

        void DeactivateAbility(string configName) {
            /// check for when the ability ended
            if (activeObject != null) {
                OnAbilityEnd();
                if (activeObject.TryGetComponent<IAbilityHandler>(out var ability))
                    ability.OnAbilityEnd -= OnAbilityUsed;
                if (configName != None)
                    Destroy(activeObject);
                activeObject = null;
                activeIndex = 0;

                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
        }

        void OnAbilityEnd() {
            if (activeObject == null || activeIndex == 0)
                return;
            var ability = activeObject.GetComponent<IAbilityHandler>();
            var updatedCD = ((ability.TotalUsage - ability.RemainingUsage) / ability.TotalUsage) * cdOffset * abilities[activeIndex].Cooldown;

            Cooldowns[activeIndex - 1].Timer.Reset(updatedCD);
        }
    }
}
