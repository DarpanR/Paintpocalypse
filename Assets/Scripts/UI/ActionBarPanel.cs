using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseAbilityPanel : MonoBehaviour
{
    public GameObject ModPickup;
    public MagGlassModData magnifyingGlass;

    public GameObject WeapPickup;
    public WeaponData pencil;

    public GameObject Eyedropper;

    public GameObject textBomb;

    ActiveAbillity active = ActiveAbillity.None;
    GameObject activeObject;

    Action OnDropHandler;

    /// returns true if there is an ability already selected
    enum ActiveAbillity {
        None,
        MagGlass,
        Pencil,
    };

    private void Start() {
        active = ActiveAbillity.None;
        OnDropHandler += () => active = ActiveAbillity.None;
        OnDropHandler?.Invoke();
    }

    void OnAbilityButtonClick (ActiveAbillity clickedAbility, IPickupData def, GameObject obj) {
        if (active == clickedAbility) {
            if (activeObject != null) {
                if (activeObject.TryGetComponent<PickupHandler>(out PickupHandler pickup)) 
                    pickup.Dropped -= OnDropHandler;
                Destroy(activeObject);
                activeObject = null;
            }
            OnDropHandler?.Invoke();
        } else {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0f;

            activeObject = Instantiate(obj, worldPos, Quaternion.identity);

            if (activeObject.TryGetComponent<PickupHandler>(out var pickup)) {
                pickup.Init(def);
                pickup.Dropped += OnDropHandler;
                active = clickedAbility;
            } else {
                Debug.LogWarning("Missing pickup handler in current active ability. Review ability!");
            }
        }
    }

    public void OnMagnifyingGlass() {
        OnAbilityButtonClick(ActiveAbillity.MagGlass, magnifyingGlass, ModPickup);
    }

    public void OnPencil() {
        OnAbilityButtonClick(ActiveAbillity.Pencil, pencil, WeapPickup);
    }

    public void EyeDropper() {

    }
}
