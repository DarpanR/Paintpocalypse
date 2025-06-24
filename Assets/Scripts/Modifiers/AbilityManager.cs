using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance { get; private set; }

    Camera cam;
    IMouseAbility selectedAbility;

    private void Awake() {
        cam = Camera.main;

        if(Instance == null)Instance = this;
        else Destroy(this);
    }
    private void Update() {
        if (selectedAbility == null) return;
        if (Input.GetMouseButtonDown(0)) {
            Vector3 worldpos = cam.ScreenToWorldPoint(Input.mousePosition);
            worldpos.z = 0f;

            // GUI update here

            // triggers the ability and returns a bool (true means end of ability)
            if (selectedAbility.OnUse(worldpos))
                DeselectAbility();
        }
    }

    public void SelectAbility(IMouseAbility newAbility) {
        selectedAbility = newAbility;
        selectedAbility.OnSelect();
        /// GUI update here
    }

    public void DeselectAbility() {
        selectedAbility = null;

        /// GUI update here
    }
}
