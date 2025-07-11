using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class StatModifier<T> : IDisposable {
    public readonly string GUID;
    public readonly List<StatModBehavior> statMods;

    public event Action<StatModifier<T>> OnDispose = delegate { };
    public bool Remove { get; private set; }

    CountdownTimer timer;
    // Guard against multiple disposals
    bool disposed = false;

    public StatModifier(List<StatModBehavior> statMods, string guid, float duration) {
        GUID = guid;
        Remove = false;
        this.statMods = statMods;
 
        timer = new CountdownTimer(duration);
        timer.OnTimerStop += () => Remove = true;
        timer.Start();
    }

    public IEnumerable<IoperationStrategy<T>> Activate() {
        foreach (var mod in statMods) {
            yield return (mod as IStatOperationProvider<T>).ToOperation();
        }
    }

    public void Reset() => timer.Start();
    public void Tick(float deltaTime) => timer?.Tick(deltaTime);
    public void Dispose() {
        if (disposed) return;
        disposed = true;

        OnDispose?.Invoke(this);
    }
}

public class CompositeStatModifier : IDisposable {
    readonly List<IDisposable> parts = new();

    public string GUID { get; }
    public bool Remove => allRemovable();

    public event Action<CompositeStatModifier> OnDispose = delegate { };

    public CompositeStatModifier(string guid) {
        GUID = guid;
    }

    public void AddPart<T>(StatModifier<T> modifier) {
        parts.Add(modifier);
        modifier.OnDispose += _ => RemovePart(modifier);
    }

    public IEnumerable<StatModifier<T>> GetPartsOfType<T>() {
        foreach (var part in parts) {
            if (part is StatModifier<T> typed) {
                yield return typed;
            }
        }
    }

    public void Tick(float dt) {
        foreach (var part in parts)
            if (part is ITickable tickable)
                tickable.Tick(dt);
    }

    public void Reset() {
        foreach (var part in parts)
            if (part is IResettable resettable)
                resettable.Reset();
    }

    public void Dispose() {
        foreach (var p in parts)
            p.Dispose();
        OnDispose?.Invoke(this);
    }

    private void RemovePart(IDisposable part) {
        parts.Remove(part);
    }

    private bool allRemovable() {
        foreach (var part in parts)
            if (part is IRemovable r && !r.Remove)
                return false;
        return true;
    }
}
