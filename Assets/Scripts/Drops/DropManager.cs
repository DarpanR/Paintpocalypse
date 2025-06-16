using System.Linq;
using UnityEngine;

public class DropManager : MonoBehaviour
{
    public static DropManager Instance;

    public GameObject dropPrefab;
    public DropTable dropTable;
    [SerializeField, Range(0f, 100f)] float dropChance;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    WeaponDefinition GetRandomDrop(DropTable table) {
        int totalWeight = table.drops.Sum(e => e.weight);
        int roll = Random.Range(0, totalWeight);

        int cumulative = 0;

        foreach (var entry in table.drops) {
            cumulative += entry.weight;

            if (roll < cumulative)
                return entry.weapon;
        }
        return null; // Shouldn't happen if weights are correct
    }

    public void TryDrop(Vector3 position) {
        TryDrop(position, dropTable, dropChance);
    }

    public void TryDrop(Vector3 position, DropTable table, float chance) {
        if (Random.value * 100 <= chance) {
            WeaponDefinition def = GetRandomDrop(table);

            if (def != null) {
                GameObject pf = Instantiate(dropPrefab, position, Quaternion.identity);
                pf.GetComponent<WeaponDrop>().SetWeaponDefinition(def);
            }
        }
    }
}
