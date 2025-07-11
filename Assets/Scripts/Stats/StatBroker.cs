using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class StatBroker<T> {
    Dictionary<String, StatModifier<T>> activeAbilityMods = new();
    StatSet<T> stats;

    public StatSet<T> CurrentStats { get; private set; }
    public event Action UpdateStats = delegate { };

    public StatBroker(StatSet<T> baseStats) {
        stats = baseStats.Clone();
        CurrentStats = stats.Clone();
        CalculateStat();
    }

    public bool Add(StatModifier<T> mod, SettableType setType) {
        string guid = mod.GUID;

        if (activeAbilityMods.TryGetValue(guid, out StatModifier<T> existing)) {
            switch (setType) {
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
        activeAbilityMods.Add(guid, mod);

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

    static readonly List<IoperationStrategy<T>> _addOpsBuffer = new();
    static readonly List<IoperationStrategy<T>> _mulOpsBuffer = new();

    void CalculateStat() {
        StatSet<T> modifiedStats = stats.Clone();
        _addOpsBuffer.Clear();
        _mulOpsBuffer.Clear();

        foreach (var mod in activeAbilityMods.Values) {
            foreach (var op in mod.Activate()) {
                if (modifiedStats.HasStat(op.Type)) {
                    if (op is AddOperation<T>) _addOpsBuffer.Add(op);
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
        //if (modifiedStats.HasStat(T.CurrentHealth) &&
        //    modifiedStats.HasStat(T.MaxHealth)) {

        //    float max = modifiedStats[T.MaxHealth].value;
        //    float current = modifiedStats[T.CurrentHealth].value;

        //    modifiedStats[T.CurrentHealth].value = Mathf.Clamp(current, 0, max);
        //}
        CurrentStats = modifiedStats;
        UpdateStats.Invoke();
    }

    public void UpdateBaseStat(T type, float value) {
        stats.AddStat(type, value);
        CalculateStat();
    }

    public void UpdateBaseStat(IoperationStrategy<T> operation) {
        UpdateBaseStat(operation.Type, operation.Calculate(stats[operation.Type].value));
    }

    public void UpdateBaseStat(Stat<T> newStat) {
        UpdateBaseStat(newStat.type, newStat.value);
    }
}
