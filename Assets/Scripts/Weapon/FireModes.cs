using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FireModeType {
    Burst,
    RateFire,
    FullAuto,
}

public interface IFireMode {
    bool ShouldFire();
    void Tick(float deltaTime);
}

public class RateFire : IFireMode {
    private readonly float interval;
    private float timeUntilNextShot = 0f;

    public RateFire(float fireRate) {
        interval = 1f / Mathf.Max(fireRate, 0.01f);
    }

    public void Tick(float deltaTime) {
        timeUntilNextShot -= deltaTime;
    }

    public bool ShouldFire() {
        if (timeUntilNextShot <= 0f) {
            timeUntilNextShot += interval;
            return true;
        }
        return false;
    }
}

public class BurstFire : IFireMode {
    private readonly int burstSize;
    private readonly float interval;
    private readonly float cooldownDuration;

    ClockTimer burstTimer;
    CountdownTimer cooldownTimer;

    bool isInCooldown = false;
    bool shouldFireFlag = false;

    public BurstFire(float fireRate, int burstSize = 3, float cooldownMultipler = 2f) {
        this.burstSize = burstSize;
        interval = 1 / fireRate;
        cooldownDuration = burstSize * interval * cooldownMultipler;

        List<float> burstAlarms = new();

        for (int i = 0; i < burstSize; i++) {
            burstAlarms.Add(i * interval);
        }
        burstTimer = new ClockTimer(0f, burstAlarms);
        burstTimer.onAlarm += _ => shouldFireFlag = true;
        burstTimer.OnTimerStop += EnterCooldown;

        cooldownTimer = new CountdownTimer(cooldownDuration);
        cooldownTimer.OnTimerStop += RestartBurst;

        RestartBurst();
    }

    void EnterCooldown() {
        isInCooldown = true;
        burstTimer.Stop();
        cooldownTimer.Reset();
    }

    void RestartBurst() {
        isInCooldown = false;
        cooldownTimer.Stop();
        burstTimer.Reset();
    }

    public void Tick(float deltaTime) {
        if (isInCooldown) cooldownTimer.Tick(deltaTime);
        else burstTimer.Tick(deltaTime);
    }

    public bool ShouldFire() {
        if (shouldFireFlag) {
            shouldFireFlag = false;
            return true;
        }
        return false;
    }
}

public class FullAutoFire : IFireMode {
    public FullAutoFire(float fireRate) { }

    public bool ShouldFire() {
        throw new System.NotImplementedException();
    }

    public void Tick(float deltaTime) {
        throw new System.NotImplementedException();
    }
}