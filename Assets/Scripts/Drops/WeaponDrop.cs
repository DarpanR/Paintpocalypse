using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class WeaponDrop : MonoBehaviour
{
    [SerializeField] WeaponDefinition weapon;

    [Tooltip("Degrees per second")]
    public float rotationSpeed = 45f;

    SpriteRenderer sr;

    private void Awake() {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Simple rotation to catch the eye
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }

    public void SetWeaponDefinition(WeaponDefinition def) {
        weapon = def;

        if (weapon != null && weapon.weaponIcon != null) {
            sr.sprite = weapon.weaponIcon;
            sr.sortingLayerName = "Pickups";
            sr.sortingOrder = 0;
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("Player")) return;
        var manager = other.GetComponentInChildren<WeaponManager>();
        
        if (manager != null) {
            manager.Equip(weapon);
        }
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
