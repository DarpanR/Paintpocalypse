using System;
using System.Diagnostics;

public abstract class Timer {
    protected float initialTime;
    public float Time { get; protected set; }
    public bool IsRunning { get; protected set; }
    
    public virtual float Progress => Time / initialTime;

    public Action OnTimerStart = delegate { };
    public Action OnTimerStop = delegate { };

    protected Timer(float value) {
        initialTime = value;
        IsRunning = false;
    }

    // Start is called before the first frame update
    public void Start() {
        Time = initialTime;

        if (!IsRunning) {
            IsRunning = true;
            OnTimerStart.Invoke();
        }
    }

    public void Stop() {
        if (IsRunning) {
            IsRunning = false;
            OnTimerStop.Invoke();
        }
    }

    public abstract void Tick(float deltaTime);

    public void Resume() => IsRunning = true;
    public void Puase() => IsRunning = false;
}

public class CountdownTimer : Timer {
    public override float Progress => 1 - base.Progress;

    public CountdownTimer(float value) : base(value) {
        OnTimerStop += Reset;
    }

    public override void Tick(float deltaTime) {
        if (IsRunning) {
            if (Time > 0) Time -= deltaTime;
            else Stop();
        }
    }

    public bool IsFinished => Time <= 0;

    public void Reset() {
        Time = initialTime;
        Start();
    }
    
    public virtual void Reset(float newTime) {
        initialTime = newTime;
        Reset();
    }
}

public class FireRateTimer : CountdownTimer {
    public float FireRate { get; private set; }
    public float FireInterval  => GetFireInterval(FireRate);

    public FireRateTimer(float fireRate) : base(GetFireInterval(fireRate)) {
        FireRate = fireRate;
    }

    public override void Reset(float newFireRate) {
        FireRate = newFireRate;
        base.Reset(FireInterval);
    }

    static float GetFireInterval(float Rate) => 1f / Rate;
}

    

public class StopwatchTimer : Timer {
    public StopwatchTimer() : base(0) { }

    public override void Tick(float deltaTime) {
        if (IsRunning) Time += deltaTime;
    }

    public void Reset() => Time = 0;
    public float GetTime() => Time;
}
