using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
public sealed class TagMaskFieldAttribute : PropertyAttribute { }

[CustomPropertyDrawer(typeof(TagMaskFieldAttribute))]
public class TagMaskFieldAttributeEditor : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
    }
}
#endif


public class WeaponManager : MonoBehaviour
{
    [Header("Where bullets originate")]
    public Transform firePoint;

    [SerializeField, TagMaskField] string target = "Untagged";

    // all the list of SO defintion
    public List<WeaponDefinition> allWeapons = new List<WeaponDefinition>();

    List<IWeaponModule> weapons = new();

    public void Start() {
        if (firePoint == null)
            firePoint = GetComponent<Transform>();

        foreach (var weapon in allWeapons)
            Equip(weapon);
    }

    private void Update() {
        // each weapon shoots at its own fire-rate
        foreach(var weapon in weapons) {
            weapon.TryFire();
        }
    }

    /// <summary>
    /// Call this when the player picks up a weapon drop or levels up.
    /// </summary>
    public void Equip(WeaponDefinition def) {
        // find existing
        var existing = weapons.FirstOrDefault(m => m.Definition == def);

        if (existing != null)
            existing.Upgrade();
        else
            weapons.Add(def.CreateModule(firePoint, target));
        // TODO: notify HUD with new weapon icon
    }
}
