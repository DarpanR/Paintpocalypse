using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineShot : LinearShot {
    [SerializeField] float sineAmp = 0.5f;
    [SerializeField] float sineFreq = 3f;
    Vector2 fireDirection;
    float speed;
    float phaseOffset;

    public override void Init(StatSet _stats, string _targetTag, IoperationStrategy _operation, int _penetration) {
        base.Init(_stats, _targetTag, _operation, _penetration);
        fireDirection = transform.up;
        speed = stats[StatType.Speed].value;
        phaseOffset = Random.value * Mathf.PI * 2;

    }

    protected override void Update() {
        Vector2 move = speed * fireDirection;
        Vector2 perp = new Vector2(-fireDirection.y, fireDirection.x);
        float sine = Mathf.Sin(Time.time * sineFreq + phaseOffset);

		move += perp * sineAmp * sine;

        transform.position += (Vector3)move * Time.deltaTime;
    }
}
