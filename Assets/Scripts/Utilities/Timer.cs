using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public abstract class Timer {
    protected float initialTime;
    public float Time { get; protected set; }
    public bool IsRunning { get; protected set; }
    
    public virtual float Progress => Time / initialTime;

    public Action OnTimerStart = delegate { };
    public Action OnTimerStop = delegate { };

    public Timer(float value) {
        Time = initialTime = value;
        IsRunning = false;
    }

    // Start is called before the first frame update
    public void Start() {
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

    protected virtual void ClearAllListeners() {
        OnTimerStart = null;
        OnTimerStop = null;
    }

    public abstract void Tick(float deltaTime);
    
    public virtual void Reset() {
        Time = initialTime;
        Start();
    }

    public virtual void Reset(float newTime) {
        //UnityEngine.Debug.Log(newTime);
        Time = initialTime = newTime;
        Start();
    }

    public void Resume() => IsRunning = true;
    public void Pause() => IsRunning = false;
}

public class CountdownTimer : Timer {
    public override float Progress => 1 - base.Progress;

    public CountdownTimer(float value) : base(value) { }

    public override void Tick(float deltaTime) {
        if (!IsRunning) return;
        if (!IsFinished) Time -= deltaTime;
        else Stop();
    }

    public bool IsFinished => Time <= 0;
}

public class FireRateTimer : CountdownTimer {
    public float FireRate => GetFireInterval(initialTime);

    public FireRateTimer(float fireRate) : base(GetFireInterval(fireRate)) {
        OnTimerStop -= Reset;
        OnTimerStop += Reset;
    }

    public override void Reset(float newFireRate) {
        base.Reset(GetFireInterval(newFireRate));  
    }

    public void ChangeProgress(float newProgress) {
        Time = initialTime * newProgress;
    }

    static float GetFireInterval(float rate) => 1f / rate;
}    

public class StopWatchTimer : Timer {
    public StopWatchTimer() : base(0) { }
    public StopWatchTimer(float value) : base(value) {}

    public override void Tick(float deltaTime) {
        Time += deltaTime;
    }

    public float GetTime() => Time;
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
        
        while (ticker < alarms.Count && Time >= alarms[ticker]) {
            onAlarm?.Invoke(alarms[ticker]);
            ticker++;
        }

        if (ticker == alarms.Count) Stop();
    }

    protected override void ClearAllListeners() {
        base.ClearAllListeners();
        onAlarm = null;
    }

    public override void Reset() {
        ticker = 0;
        base.Reset();
    }

    public override void Reset(float newTime) {
        ticker = 0;
        base.Reset(newTime);
    }

    public void Reset(List<float> newAlarms) {
        Reset(initialTime, newAlarms);
    }

    public void Reset(float newTime, List<float> newAlarms) {
        alarms = newAlarms ?? alarms;
        Reset(newTime);
    }
}