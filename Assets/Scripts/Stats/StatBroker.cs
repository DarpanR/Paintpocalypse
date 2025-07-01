using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class StatBroker {
    Dictionary<String, StatModifier> activeAbilityMods = new();
    StatSet stats;

    public StatSet CurrentStats { get; private set; }
    public event Action UpdateStats = delegate { };

    public StatBroker(StatSet baseStats) {
        stats = baseStats.Clone();
        CurrentStats = stats.Clone();
        CalculateStat();
    }

    public bool Add(StatModData data) {
        string guid = data.GUID;

        if (activeAbilityMods.TryGetValue(guid, out StatModifier existing)) {
            switch (data.settable) {
                case SettableType.Timer:
                    if (existing != null) {
                        existing.Reset();
                        return true;
                    }
                    break;
                case SettableType.Multi:
                    // allow for stat stacking
                    break;
                case SettableType.Single:
                    // ignore
                    if (existing != null) return false;
                    break;
            }
        }
        activeAbilityMods[guid] = data.CreateModule(data);
        activeAbilityMods[guid].OnDispose += _ => activeAbilityMods.Remove(guid);
        CalculateStat();
        return true;
    }

    public void Tick(float deltaTime) {
        if (activeAbilityMods.Count == 0) return;
        bool shouldRecalculate = false;
        List<string> toRemove = new();

        foreach (var kvp in activeAbilityMods) {
            var key = kvp.Key;
            var mod = kvp.Value;

            mod.Tick(deltaTime);

            if (mod.Remove) {
                toRemove.Add(key);
                shouldRecalculate = true;
            }
        }

        foreach (var key in toRemove) {
            if (activeAbilityMods.TryGetValue(key, out var mod))
                mod.Dispose();
            activeAbilityMods.Remove(key);
        }    
        if (shouldRecalculate) CalculateStat();
    }

    static readonly List<IoperationStrategy> _addOpsBuffer = new();
    static readonly List<IoperationStrategy> _mulOpsBuffer = new();

    void CalculateStat() {
        StatSet modifiedStats = stats.Clone();
        _addOpsBuffer.Clear();
        _mulOpsBuffer.Clear();

        foreach (var mod in activeAbilityMods.Values) {
            foreach (var op in mod.Activate()) {
                if (modifiedStats.HasStat(op.Type)) {
                    if (op is AddOperation) _addOpsBuffer.Add(op);
                    else _mulOpsBuffer.Add(op);
                }
            }
        }

        foreach (var op in _addOpsBuffer) {
            modifiedStats[op.Type].Apply(op);
        }

        foreach (var op in _mulOpsBuffer)
            modifiedStats[op.Type].Apply(op);

        // Clamp CurrentHealth between 0 and MaxHealth
        if (modifiedStats.HasStat(StatType.CurrentHealth) &&
            modifiedStats.HasStat(StatType.MaxHealth)) {

            float max = modifiedStats[StatType.MaxHealth].value;
            float current = modifiedStats[StatType.CurrentHealth].value;

            modifiedStats[StatType.CurrentHealth].value = Mathf.Clamp(current, 0, max);
        }
        CurrentStats = modifiedStats;
        UpdateStats.Invoke();
    }

    public void UpdateBaseStat(StatType type, float value) {
        stats.AddStat(type, value);
        CalculateStat();
    }

    public void UpdateBaseStat(IoperationStrategy operation) {
        UpdateBaseStat(operation.Type, operation.Calculate(stats[operation.Type].value));
    }

    public void UpdateBaseStat(Stat newStat) {
        UpdateBaseStat(newStat.type, newStat.value);
    }
}
