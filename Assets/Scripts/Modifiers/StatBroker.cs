using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;

public class StatBroker {

    public event Action<StatSet> UpdateStats = delegate { };

    List<StatModifier> activeMods = new();
    StatSet stats;
    public StatSet CurrentStats { get; private set; }

    public StatBroker(StatSet baseStats) {
        this.stats = baseStats.Clone();
        CurrentStats = baseStats.Clone();
        CalculateStat();
    }

    public bool Add(StatModifier modifier) {
        StatModifier existing = activeMods.Find(mod => mod.Definition.GUID == modifier.Definition.GUID);

        switch (modifier.Definition.settable) {
            case settableType.Timer:
                if (existing != null) {
                    existing.ResetTimer();
                    return true;
                }
                break;
            case settableType.Multi:
                // allow for stat stacking
                break;
            case settableType.Single:
                // ignore
                if (existing != null) return false;
                break;
        }
        activeMods.Add(modifier);
        modifier.OnDispose += _ => activeMods.Remove(modifier);
        CalculateStat();
        return true;
    }

    public void Tick(float deltaTime) {
        if (activeMods.Count == 0) return;

        for( int i = activeMods.Count - 1; i >= 0; i-- ) {
            var mod = activeMods[i];
            mod.Tick(deltaTime);

            if (mod.Remove) {
                mod.Dispose();
                CalculateStat();
            }
        }
    }

    public void CalculateStat() {
        StatSet modifiedStats = stats.Clone();

        foreach (var mod in activeMods) {
            if (mod is IStatSetModifier ssMod)
                ssMod.Activate(modifiedStats);
        }

        CurrentStats = modifiedStats;
        UpdateStats.Invoke(modifiedStats);
    }

    public void UpdateBaseStat(StatType type, float value) {
        UpdateBaseStat(new Stat { type = type, value = value });   
    }
    public void UpdateBaseStat(Stat newStat) {
        stats.AddStat(newStat.type, newStat.value);
        CalculateStat();
    }

}
