using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Projectile {
    public float fuseTime;
    public float explosionRadius;

    CountdownTimer timer;

    private void Start() {
        timer = new CountdownTimer(fuseTime);
        timer.OnTimerStop += Explode;
        timer.Start();
    }

    protected override void Update() {
        timer.Tick(Time.deltaTime);
    }

    void Explode() {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (var col in cols) {
            if (col != null && col.CompareTag(targetTag)) {
                if (col.TryGetComponent<BaseEntity>(out var entity)) {
                    entity.TakeDamage(operation);
                }
            }
        }
    }
}
