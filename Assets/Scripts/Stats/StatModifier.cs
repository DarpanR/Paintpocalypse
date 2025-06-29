using System;
using System.Collections.Generic;

public abstract class StatModifier : IDisposable {
    public bool Remove { get; private set; }

    public string GUID { get; private set; }

    //public StatModData data { get; private set; }
    public event Action<StatModifier> OnDispose = delegate { };

    CountdownTimer timer;
    public void Reset() => timer.Start();
    public void Tick(float deltaTime) => timer?.Tick(deltaTime);

    public StatModifier(string guid, float duration) {
        GUID = guid;
        Remove = false;
 
        timer = new CountdownTimer(duration);
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
