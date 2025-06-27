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

    float lt;
    float timer = 0f;
    float currentRadius = 0f;

    public override void Init(StatSet stats, string target, IoperationStrategy operation, float lifetime, int penetration) {
        currentRadius = minRadius;
        timer = 0;
        lt = lifetime;

        transform.localScale = Vector3.one * minRadius;
        base.Init(stats, target, operation, lifetime, penetration);
    }

    protected override void Update() {
        timer += Time.deltaTime;

        float normalizedTime = Mathf.Clamp01(timer / lt);
        float curveValue = expansionCurve.Evaluate(normalizedTime);

        currentRadius = Mathf.Lerp(minRadius, maxRadius, curveValue);
        transform.localScale = Vector3.one * currentRadius * 2f;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag(target)) {
            var id = collision.gameObject.GetInstanceID();

            enemiesHit[id] = 0f;
            collision.GetComponent<BaseEntity>().TakeDamage(operation);
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.CompareTag(target)) {
            var id = collision.gameObject.GetInstanceID();

            if(enemiesHit[id] > stats[StatType.FireRate].value) {
                enemiesHit[id] -= stats[StatType.FireRate].value;
                collision.GetComponent<BaseEntity>().TakeDamage(operation);
            }
        }
    }
}
