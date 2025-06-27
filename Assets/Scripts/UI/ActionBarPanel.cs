using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBarPanel : MonoBehaviour
{
    public GameObject ModPickup;
    public MagnifyingGlassDefinition magnifyingGlass;

    /// returns true if there is an ability already selected
    bool active;

    private void Start() {
        active = false;
    }

    public void MagnifyingGlass() {
        if (!active) {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0f;

            var prefab = Instantiate(ModPickup, worldPos, Quaternion.identity);
            var pickup = prefab.GetComponent<ModifierPickup>();
            Debug.Log("Before Pass " + magnifyingGlass.ToString());
            pickup.Init(magnifyingGlass);
            pickup.Dropped += () => active = false;

            active = true;
        }
    }

    public void Eyedropper() {

    }
}
