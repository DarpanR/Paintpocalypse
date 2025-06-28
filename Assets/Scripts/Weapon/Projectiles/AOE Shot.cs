using System.Collections;
using System.Collections.Generic;
using System.Xml.Xsl;
using UnityEngine;

public class AOEShot : Projectile
{
    public float minRadius = 0f;
    public float maxRadius = 3f;
    [SerializeField]
    AnimationCurve expansionCurve = AnimationCurve.Linear(0, 0, 1, 1);

    CountdownTimer timer;
    float currentRadius = 0f;

    public override void Init(StatSet stats, string targetTag, IoperationStrategy operation, int penetration) {
        // scale adjustment for scale warping abilities.
        float scale = stats.GetValueOrAdd(StatType.LocalScale, 1f);
        minRadius *= scale;
        maxRadius *= scale;
        currentRadius = minRadius;

        timer = new CountdownTimer(stats[StatType.Lifetime].value);
        timer.Start();

        transform.localScale = Vector3.one * scale * minRadius;
        base.Init(stats, targetTag, operation, penetration);
    }

    protected override void Update() {
        timer.Tick(Time.deltaTime);

        currentRadius = Mathf.Lerp(minRadius, maxRadius, expansionCurve.Evaluate(timer.Progress));
        transform.localScale = Vector3.one * currentRadius * 2f;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag(targetTag)) {
            var id = collision.gameObject.GetInstanceID();

            enemiesHit[id] = 0f;
            collision.GetComponent<BaseEntity>().TakeDamage(operation);
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.CompareTag(targetTag)) {
            var id = collision.gameObject.GetInstanceID();

            if(enemiesHit[id] > stats[StatType.FireRate].value) {
                enemiesHit[id] -= stats[StatType.FireRate].value;
                collision.GetComponent<BaseEntity>().TakeDamage(operation);
            }
        }
    }
}
