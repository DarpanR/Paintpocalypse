using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseAbilityPanel : MonoBehaviour {
    [SerializeField]
    float globalCD = 1f;
    [SerializeField, Range(0,1)]
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
    public class AbilityButton {
        public Button Button;
        public CountdownTimer Timer;
        public Image CooldownImage;
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
        for (int i = 1; i < abilities.Count; i++)
            InitializeAbility(abilities[i], i);
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

            if (cd.Timer.IsRunning) 
                cd.backgroundImage.fillAmount = cd.Timer.Progress;
        }
    }

    private void InitializeAbility(AbilityConfig ability, int index) {
        // validate
        if (string.IsNullOrWhiteSpace(ability.AbilityName))
            ResolveAbilityName(ref ability);

        ability.AbilityName = ability.AbilityName.ToLowerInvariant();
        if (ability.Prefab == null)
            throw new Exception($"{ability.AbilityName} is missing a Prefab.");
        if (ability.PickupData != null && ability.PickupData is not IPickupData)
            throw new Exception($"Ability '{ability.AbilityName}' has invalid PickupData.");

        // create UI
        var buttonGO = Instantiate(buttonPrefab, buttonContainer);
        buttonGO.name = ability.AbilityName;

        var aButton = buttonGO.GetComponent<Button>();
        actionButtons.Add(aButton);

        SetupButtonVisuals(aButton, ability);

        // cooldown
        var timer = new CountdownTimer(globalCD);
        Cooldowns.Add(new CooldownData { Timer = timer, backgroundImage = GetBackgroundImage(aButton) });
        timer.OnTimerStop += () => aButton.interactable = true;

        // click
        aButton.onClick.AddListener(() => ActivateAbility(index));
    }

    private void SetupButtonVisuals(Button button, AbilityConfig ability) {
        var iconImage = button.transform.Find("Icon").GetComponent<Image>();
        if (ability.Icon != null) {
            iconImage.sprite = Sprite.Create(
                ability.Icon,
                new Rect(0, 0, ability.Icon.width, ability.Icon.height),
                new Vector2(0.5f, 0.5f)
            );
        }
    }

    private Image GetBackgroundImage(Button button) {
        var background = button.transform.Find("Background");
        if (background == null)
            throw new Exception($"Button '{button.gameObject.name}' is missing a 'Background' child.");

        var image = background.GetComponent<Image>();
        if (image == null)
            throw new Exception($"'Background' under button '{button.gameObject.name}' has no Image component.");

        return image;
    }

    private void ResolveAbilityName(ref AbilityConfig ability) {
        if (ability.PickupData is IPickupData pickupData) {
            if (!string.IsNullOrWhiteSpace(pickupData.PickupName)) {
                ability.AbilityName = pickupData.PickupName;
                return;
            }
            throw new Exception("PickupData has no PickupName!");
        }

        throw new Exception("AbilityConfig is missing AbilityName and has no valid PickupData to derive it from.");
    }

    void ActivateAbility(int newIndex) {
        if (newIndex == activeIndex) {
            // Switching to a new ability
            DeactivateCurrentAbility(true);
        } else if (newIndex == 0) {
            // Clicking "None": just dereference, let it live
            DeactivateCurrentAbility(false);
            return;
        }
        ActivateNewAbility(newIndex);
    }

    void DeactivateCurrentAbility(bool keepObject) {
        if (activeObject.TryGetComponent<IAbilityHandler>(out var ability)) {
            HandleAbilityEnd(ability);
            ability.OnAbilityEnd -= OnAbilityUsed;
        }

        if (!keepObject) Destroy(activeObject);
        activeObject = null;
        activeIndex = 0;
    }

    void ActivateNewAbility(int index) {
        var abilityData = abilities[index];
        activeObject = Instantiate(abilityData.Prefab, GameInputManager.Instance.MouseWorldPosition, Quaternion.identity);

        if (activeObject.TryGetComponent<IAbilityHandler>(out var handler)) {
            if (abilityData.PickupData is IPickupData pData)
                handler.Init(pData);

            handler.OnAbilityEnd += () => actionButtons[index - 1].interactable = false;
            activeIndex = index;
            Cooldowns[index - 1].Timer.Reset(abilityData.Cooldown);

            StartGlobalCooldowns(index);
        } else {
            Debug.LogWarning("Missing IAbilityHandler!");
            activeIndex = 0;
        }
    }

    void StartGlobalCooldowns(int activeAbilityIndex) {
        for (int i = 1; i < abilities.Count; i++) {
            if (i == activeAbilityIndex) continue;

            var cd = Cooldowns[i - 1];
            if (!cd.Timer.IsRunning) {
                cd.Timer.Reset(globalCD);
            }
        }
    }

    void HandleAbilityEnd(IAbilityHandler ability) {
        var cooldownTimer = Cooldowns[activeIndex - 1].Timer;
        var usage = (ability.TotalUsage - ability.RemainingUsage) / ability.TotalUsage;
        var remainingTime = abilities[activeIndex].Cooldown * (cooldownTimer.Progress);
        var reduction = usage * cdOffset;
        var finalValue = Mathf.Max(globalCD, remainingTime * reduction);

        Debug.Log($"Usage: {usage}, RemainingTime: {remainingTime}, Reduction: {reduction}, FinalValue: {finalValue}");

        cooldownTimer.Reset(finalValue);
    }
}