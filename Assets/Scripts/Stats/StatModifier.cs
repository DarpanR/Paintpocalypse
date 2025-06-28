using System;
using System.Collections.Generic;

public abstract class StatModifier : IDisposable {
    public bool Remove { get; private set; }
    public ModifierDefinition Definition { get; private set; }
    public event Action<StatModifier> OnDispose = delegate { };

    CountdownTimer timer;
    public void ResetTimer() => timer.Reset();
    public void Tick(float deltaTime) => timer?.Tick(deltaTime);

    public StatModifier(ModifierDefinition definition) {
        Definition = definition;
        Remove = false;

        timer = new CountdownTimer(definition.duration);
        timer.OnTimerStop += () => Remove = true;
        timer.Start();
    }

    public abstract List<IoperationStrategy> Activate();

    // Guard against multiple disposals
    bool disposed = false;
    public void Dispose() {
        if (disposed) return;
        disposed = true;

        OnDispose?.Invoke(this);
    }
}
