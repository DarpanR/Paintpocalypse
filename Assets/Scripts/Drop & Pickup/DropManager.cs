using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct DropEntry {
    public PickupType PickupType;
    [SerializeField]
    public ScriptableObject DropData;
    public float weight;
}

[Serializable]
public struct EntityDropEntry {
    public int Cost;
    public int Exp;
    public int[] dropTablesAllowed;
    [Range(0f, 1f)]
    public float weight;
}

public class DropManager : MonoBehaviour {
    public static DropManager Instance;

    public AnimationCurve globalDropChance;
    public AnimationCurve noDropCurve;

    public GameObject weapPickupPrefab;
    public GameObject modPickupPrefab;
    public GameObject currencyPrefab;

    public List<DropTable> tables;

    StopWatchTimer lastDropTimer = new StopWatchTimer();

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);

        if (tables == null) {
            Debug.LogWarning("DropManager has no DropTable assigned. Drops will be disabled.");
            return;
        }
    }

    private void OnEnable() {
        GameEvents.OnEntityDeath += TryDrop;
        GameEvents.OnGameStart += Init;
    }

    private void OnDisable() {
        GameEvents.OnEntityDeath -= TryDrop;
        GameEvents.OnGameStart -= Init;
    }

    private void Update() {
        lastDropTimer.Tick(Time.deltaTime);
    }

    void Init() {
        lastDropTimer.Reset();
    }

    void TryDrop(string guid, Vector3 position) {
        EntityDropEntry entry = EntityManager.Instance.GetEntityData(guid).dropEntry;

        if (entry.dropTablesAllowed == null && entry.dropTablesAllowed.Length == 0) {
            Debug.LogWarning($"DropManager: No EntityData found for GUID {guid}");
            return;
        }

        SpawnExp(entry.Exp, position);

        float dynamicChance = GetDynamicDropChance(entry);
        float roll = Random.value * 100f;

        Debug.Log($"DropManager: Roll={roll:F1} vs chance={dynamicChance:F1}");

        if (roll <= dynamicChance) {
            var ind = Random.Range(0, entry.dropTablesAllowed.Length);
            TryDropFromTable(tables[ind], position);
            lastDropTimer.Reset();
        }
    }

    void TryDropFromTable(DropTable table, Vector3 position) {
        if (table == null || table.drops == null || table.drops.Count == 0) {
            Debug.Log("DropManager: No drops configured.");
            return;
        }

        // Compute total weight
        float totalWeight = 0f;
        foreach (var entry in table.drops)
            totalWeight += entry.weight;

        float roll = UnityEngine.Random.value * totalWeight;

        float cumulative = 0f;
        foreach (var entry in table.drops) {
            cumulative += entry.weight;
            if (roll <= cumulative) {
                SpawnDrop(entry, position);
                return;
            }
        }
    }

    void SpawnDrop(DropEntry entry, Vector3 position) {
        switch(entry.PickupType) {
            case PickupType.StatModifier:
                Instantiate(modPickupPrefab, position, Quaternion.identity).GetComponent<PickupHandler>().Init(entry.DropData as IPickupData, true);
                break;

                case PickupType.Weapon:
                Instantiate(weapPickupPrefab, position, Quaternion.identity).GetComponent<PickupHandler>().Init(entry.DropData as IPickupData, true);
                break;
            case PickupType.Exp:
            case PickupType.Currency:
                SpawnExp(CalcCurrency(), position);
                break;
            default:
                return;
        }
    }

    void SpawnExp(int amount, Vector3 position) {
        var go = Instantiate(currencyPrefab, position, currencyPrefab.transform.rotation);
        var currency = go.GetComponent<CurrencyPickup>();
        currency.Init(amount);
    }

    int CalcCurrency() {
        return 10;
    }

    float GetDynamicDropChance(EntityDropEntry entry) {
        float timeFactor = PhaseManager.Instance.TotalElapsedTime / PhaseManager.Instance.TotalDuration;
        //float killFactor = GameController.Instance.CurrentKillStreak / GameController.Instance.TotalKills;
        float killFactor = 0.5f;
        float pityFactor = noDropCurve.Evaluate(lastDropTimer.Time);
        float dynamicChance = globalDropChance.Evaluate ((timeFactor - killFactor + pityFactor) * entry.weight) * 100f;

        return Mathf.Clamp(dynamicChance, 0f, 100f);
    }
}
