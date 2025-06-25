using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBarPanel : MonoBehaviour
{
    public GameObject pickupPrefab;

    /// returns true if there is an ability already selected
    bool active;

    private void Start() {
        active = false;
    }

    public void MagnifyingGlass() {
        if (!active) {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0f;

            ModifierPickup pickup = Instantiate(pickupPrefab, worldPos, Quaternion.identity).GetComponent<ModifierPickup>();
            pickup.Init();
            pickup.Dropped += () => active = false;

            active = true;
        }    
    }

    public void Eyedropper() {

    }
}
