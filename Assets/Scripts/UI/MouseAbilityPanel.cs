using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseAbilityPanel : MonoBehaviour {
    [SerializeField]
    float globalCD = 1f;
    [SerializeField]
    float cdOffset = 0.2f;
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
    List<Button> actionButtons = new();

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
                    } else
                        throw new Exception("PickupData has no PickupName!");
                } else
                    throw new Exception("Ability has no name and no valid IPickupData to derive a name from!");
            }
            // set all names to lowercase
            ability.AbilityName = ability.AbilityName?.ToLowerInvariant();

            if (ability.Prefab == null)
                throw new Exception($"{ability.AbilityName} is missing a Prefab.");
            if (ability.PickupData != null && ability.PickupData is not IPickupData)
                throw new Exception($"Bruh this ScriptableObject in ability '{ability.AbilityName}' ain't an IPickupData!");

            /// create visuals 
            /// create ability button and assign listener
            var aButton = Instantiate(buttonPrefab, buttonContainer).GetComponent<Button>();
            actionButtons.Add(aButton);
            aButton.gameObject.name = ability.AbilityName;
            
            /// Ability Icon/Image
            var Icon = aButton.transform.Find("Icon").GetComponent<Image>();

            if (ability.Icon != null) {
                Icon.sprite = Sprite.Create(
                    ability.Icon,
                    new Rect(0, 0, ability.Icon.width, ability.Icon.height),
                    new Vector2(0.5f, 0.5f)
                );
            }

            /// Coolddown Timer visual
            var cdBG = aButton.transform.Find("Background").GetComponent<Image>();
            var timer = new CountdownTimer(globalCD);

            Cooldowns.Add(new CooldownData { Timer = timer, backgroundImage = cdBG });

            /// interactable again when timer runs out
            timer.OnTimerStop += () => aButton.interactable = true;

            var index = i;
            aButton.onClick.AddListener(() => ActivateAbility(index));
        }
        // activates defualt None
        OnAbilityUsed += () => ActivateAbility(0);
        //OnAbilityUsed?.Invoke();
    }

    private void Update() {
        for (int i = 0; i < Cooldowns.Count; i++) {
            var cd = Cooldowns[i];
            //foreach (var cd in Cooldowns) {
            /// ticks if Timer started
            cd.Timer.Tick(Time.deltaTime);
            cd.backgroundImage.fillAmount = cd.Timer.Progress;
        }
    }

    void ActivateAbility(int newIndex) {
        var oldAbility = abilities[activeIndex];
        var newAbility = abilities[newIndex];
        //Debug.Log("before active:" + oldAbility.AbilityName + ", new: " + newAbility.AbilityName);

        //Deactivates old ability if clicked on the same ability, or if ability has ended (by passing in none(index = 0))
        if (newAbility.AbilityName == oldAbility.AbilityName || newAbility.AbilityName == None) {
            DeactivateAbility();
            // else activate new ability and deactivate all old unsed ability
        } else {
            DeactivateAbility();
            activeObject = Instantiate(newAbility.Prefab, GameInputManager.Instance.MouseWorldPosition, Quaternion.identity);

            if (activeObject.TryGetComponent<IAbilityHandler>(out var ability)) {
                if (newAbility.PickupData is IPickupData pData)
                    ability.Init(pData);
                ability.OnAbilityEnd += () => actionButtons[newIndex - 1].interactable = false;
                activeIndex = newIndex;
                Cooldowns[activeIndex - 1].Timer.Reset(newAbility.Cooldown);

                //Cursor.SetCursor(newAbility.Icon, Vector2.zero, CursorMode.Auto);
            } else {
                Debug.LogWarning("Missing Abililtyability component!");
                activeIndex = 0;
            }

            /// start a global cooldown for all abilities except the currently active.
            for (int i = 1; i < abilities.Count || i == activeIndex; i++) {
                var cd = Cooldowns[i - 1];

                if (!cd.Timer.IsRunning) {
                    //Debug.Log("cd reseted for : " + i);
                    /// if ability isn't currently in cooldown, if cooldown = active.cooldown else global cd
                    cd.Timer.Reset(globalCD);
                }
            }
            //Debug.Log("after active:" + abilities[activeIndex].AbilityName + ", new: " + newAbility.AbilityName);
        }

        void DeactivateAbility() {
            /// check for when the ability ended
            if (activeObject == null) return;
            if (activeObject.TryGetComponent<IAbilityHandler>(out var ability)) {
                if (newAbility.AbilityName != None) {
                    AbilityEnded(ability); 
                    Destroy(activeObject);
                }
                ability.OnAbilityEnd -= OnAbilityUsed;
            }
            activeObject = null;
            activeIndex = 0;

            //Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        void AbilityEnded(IAbilityHandler ability) {
            var cd = Cooldowns[activeIndex - 1].Timer;
            var usage = ((ability.TotalUsage - ability.RemainingUsage) / ability.TotalUsage);
            var remainingTime = abilities[activeIndex].Cooldown - cd.Progress;
            var reduction = (1 - usage) * cdOffset;
            var finalValue = Mathf.Max(globalCD, remainingTime * reduction);

            Debug.Log($"Usage: {usage}, RemainingTime: {remainingTime}, Reduction: {reduction}, FinalValue: {finalValue}");

            cd.Reset(finalValue);
        }
    }
}