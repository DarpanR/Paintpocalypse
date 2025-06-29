using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Timer {
    protected float initialTime;
    public float CurrentTime { get; protected set; }
    public bool IsRunning { get; protected set; }
    
    public virtual float Progress => CurrentTime / initialTime;

    public Action OnTimerStart = delegate { };
    public Action OnTimerStop = delegate { };

    public Timer(float value) {
        initialTime = value;
        IsRunning = false;
    }

    // Start is called before the first frame update
    public void Start() {
        CurrentTime = initialTime;

        if (!IsRunning) {
            IsRunning = true;
            OnTimerStart?.Invoke();
        }
    }

    public void Stop() {
        if (IsRunning) {
            IsRunning = false;
            OnTimerStop?.Invoke();
        }
    }

    public abstract void Tick(float deltaTime);
    public virtual void Reset(float newTime) {
        initialTime = newTime;
        Start();
    }

    public void ResumeTimer() => IsRunning = true;
    public void PauseTimer() => IsRunning = false;
}

public class CountdownTimer : Timer {
    public override float Progress => 1 - base.Progress;

    public CountdownTimer(float value) : base(value) { }

    public override void Tick(float deltaTime) {
        if (!IsRunning) return;
        if (!IsFinished) CurrentTime -= deltaTime;
        else Stop();
    }

    public bool IsFinished => CurrentTime <= 0;
}

public class FireRateTimer : CountdownTimer {
    public float FireRate => GetFireInterval(initialTime);

    public FireRateTimer(float fireRate) : base(GetFireInterval(fireRate)) {
        OnTimerStop -= Start;
        OnTimerStop += Start;
    }

    public override void Reset(float newFireRate) {
        base.Reset(GetFireInterval(newFireRate));  
    }

    static float GetFireInterval(float rate) => 1f / rate;
}    

public class StopWatchTimer : Timer {
    public StopWatchTimer(float value) : base(0) {
    }

    public override void Tick(float deltaTime) {
        CurrentTime += deltaTime;
    }

    public float GetTime() => CurrentTime;
}

public class ClockTimer : StopWatchTimer {
    List<float> alarms;
    int ticker;

    public IReadOnlyList<float> Alarms => alarms;
    public int CurrentTicker => ticker;

    public event Action<float> onAlarm;

    public ClockTimer(float initialTime, float interval, float duration) : base(initialTime) {
        int count = (int)(duration / interval);
        alarms = new List<float>(count);

        for (int i = 0; i < count; i++)
            alarms.Add(initialTime + interval * i);
        ticker = 0;
    }

    public ClockTimer(float initialTime, List<float> incAlarms) : base(initialTime) {
        alarms = incAlarms ?? new List<float>(); 
        alarms.Sort();
        ticker = 0;
    }

    public override void Tick(float deltaTime) {
        base.Tick(deltaTime);

        if(alarms == null || alarms.Count == 0) return;
        
        while (ticker < alarms.Count && CurrentTime >= alarms[ticker]) {
            onAlarm?.Invoke(alarms[ticker]);
            ticker++;
        }

        if (ticker == alarms.Count - 1) Stop();
        
    }

    public void Reset() {
        ticker = 0;
        Start();
    }

    public override void Reset(float newTime) {
        ticker = 0;
        base.Reset(newTime);
    }
}