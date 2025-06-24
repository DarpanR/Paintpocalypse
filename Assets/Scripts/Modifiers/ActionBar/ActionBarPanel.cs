using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDropper : MonoBehaviour {
    IMouseAbility ability;

    bool dropped = false;
    Camera cam = Camera.main;

    public void Init(IMouseAbility newAbility) {
        ability = newAbility;
    }

    private void Update() {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        if (!dropped) {
            transform.position = mousePos;

            if (Input.GetMouseButtonDown(0)) { 
                dropped = true;
                ability.OnSelect();    
            }
        } else if (ability.OnUse(mousePos))
            Destroy(gameObject);
    }
}

public class ActionBarPanel : MonoBehaviour
{
    public static ActionBarPanel Instance;

    public AbilityDropper dropPrefab;
    public MagnifyingGlassDefinition mgDef;

    IMouseAbility selectedAbilily;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void OnMagnifyingGlassClick() {
        SelectAbility(new MagnifyingGlass(mgDef));
    }

    public void OnEyeDropper() {
        SelectAbilily(new EyeDropper(edDef));
    }

    void SelectAbility(IMouseAbility newAbility) {
        if (selectedAbilily == newAbility) {
            selectedAbilily = null;
            return;
        }
        selectedAbilily = newAbility;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        AbilityDropper dropper = Instantiate(dropPrefab, mousePos, Quaternion.identity);
        dropper.Init(newAbility);
    }
}
