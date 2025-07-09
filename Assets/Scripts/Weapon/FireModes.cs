using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFireMode {
    bool ShouldFire();
    void Tick(float deltaTime);
}

public class BurstFire : IFireMode {
    private readonly int burstSize;
    private readonly float interval;
    private readonly float cooldown;

    private ClockTimer burstTimer;
    private bool isInCooldown = false;
    private bool shouldFireFlag = false;

    public BurstFire(float fireRate, int burstSize = 3, float cooldownMultipler = 2f) {
        this.burstSize = burstSize;
        interval = 1 / fireRate;
        cooldown = burstSize * interval * cooldownMultipler;

        List<float> alarms = new();

        for (int i = 0; i < burstSize; i++) {
            alarms.Add(i * interval);
        }
        burstTimer = new ClockTimer(0f, alarms);
        burstTimer.onAlarm += _ => shouldFireFlag = true;
        burstTimer.OnTimerStop += EnterCooldown;
        burstTimer.Start();
    }

    void EnterCooldown() {
        isInCooldown = true;
        burstTimer.Reset(cooldown);
        burstTimer.OnTimerStop += RestartBurst;
    }

    void RestartBurst() {
        isInCooldown = false;
        burstTimer.OnTimerStop -= RestartBurst;
        burstTimer.Reset();
    }

    public void Tick(float deltaTime) {
        burstTimer.Tick(deltaTime);
    }

    public bool ShouldFire() {
        if (shouldFireFlag) {
            shouldFireFlag = false;
            return true;
        }
        return false;
    }

}