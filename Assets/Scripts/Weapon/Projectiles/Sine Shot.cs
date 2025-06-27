using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineShot : LinearShot
{
    [SerializeField] float sineAmp = 0.5f;
    [SerializeField] float sineFreq = 3f;
    Vector3 baseDirection;
    float phaseOffset;

    public override void Init(StatSet _stats, string _target, IoperationStrategy _operation, float _lifetime, int _penetration) {
        base.Init(_stats, _target, _operation, _lifetime, _penetration);
        baseDirection = new Vector2(0f, _stats[StatType.Velocity].value);
        phaseOffset = Random.value * Mathf.PI * 2;
        
    }

    protected override void Update() {
        Vector2 move = baseDirection;
        Vector2 perp = new Vector2(-baseDirection.y, baseDirection.x);

        float sine = Mathf.Sin(Time.time * sineFreq + phaseOffset);
        move += perp * sineAmp * sine;

        transform.position += (Vector3)move * Time.deltaTime;
    }
}
