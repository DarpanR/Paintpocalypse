using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PickupHandler : MonoBehaviour
{
    public BaseWeapon weapon;
    public float rotationSpeed = 45f;

    public void Update() {
        transform.Rotate(rotationSpeed * Time.deltaTime, 0f, 0f);
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (weapon == null) {
            return;
        }

        if (collision.CompareTag("Player")) {
            WeaponManager wm = collision.GetComponent<WeaponManager>();

            if (wm != null) {
                wm.AddWeapon(weapon);
                Destroy(gameObject);
            }
        }
    }
}
