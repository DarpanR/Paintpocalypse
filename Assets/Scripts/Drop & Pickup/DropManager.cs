using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class DropEntry {
    public WeaponData weapon;
    public int weight;
}

public class DropManager : MonoBehaviour
{
    public static DropManager Instance;

    public GameObject pickupPrefab;
    public DropTable dropTable;
    [SerializeField, Range(0f, 100f)] float dropChance;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);

        if (dropTable == null) {
            Debug.LogWarning("DropManager has no DropTable assigned. Drops will be disabled.");
            return;
        }
    }

    public void TryDrop(Vector3 position) {
        TryDrop(position, dropTable, dropChance);
    }

    public void TryDrop(Vector3 position, DropTable table, float chance) {
        if (table == null) {
            Debug.LogWarning("DropManager has no DropTable assigned. Drops will be disabled.");
            return;
        } else if (UnityEngine.Random.value * 100 <= chance) {
            WeaponData def = GetRandomDrop(table);

            if (def != null) {
                GameObject pf = Instantiate(pickupPrefab, position, Quaternion.identity);
                pf.GetComponent<WeaponPickup>().Init(def, true);
            }
        }
    }

    WeaponData GetRandomDrop(DropTable table) {
        int totalWeight = table.drops.Sum(e => e.weight);
        int roll = UnityEngine.Random.Range(0, totalWeight);

        int cumulative = 0;

        foreach (var entry in table.drops) {
            cumulative += entry.weight;

            if (roll < cumulative)
                return entry.weapon;
        }
        return null; // Shouldn't happen if weights are correct
    }
}
