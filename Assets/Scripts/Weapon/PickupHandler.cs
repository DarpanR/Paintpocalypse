using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PickupHandler : MonoBehaviour
{
    [Tooltip("Which weapon this pickup grants")]
    public WeaponDefinition weapon;

    [Tooltip("Degrees per second")]
    public float rotationSpeed = 45f;

    SpriteRenderer sr;

    private void Awake() {
        sr = GetComponent<SpriteRenderer>();

        if(weapon != null && weapon.weaponIcon != null) {
            sr.sprite = weapon.weaponIcon;
            sr.sortingLayerName = "Pickups";
            sr.sortingOrder = 0;
        }
    }

    void Update()
    {
        // Simple rotation to catch the eye
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Only react to the player
        if (!other.CompareTag("Player")) return;

        // Find the WeaponManager on the player (could be on the same GameObject or a child)
        var manager = other.GetComponentInChildren<WeaponManager>();
        if (manager != null)
        {
            // Equip or upgrade the weapon
            manager.Equip(weapon);
        }

        // Destroy the pickup object so it can’t be grabbed twice
        Destroy(gameObject);
    }

    // (Optional) Draw a little gizmo in the editor so you can see
    // which definition is assigned without running the game.
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (weapon != null && weapon.weaponIcon != null)
        {
            Gizmos.DrawIcon(transform.position, 
                AssetDatabase.GetAssetPath(weapon.weaponIcon), true);
        }
    }
#endif
}
