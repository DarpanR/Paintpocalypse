using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineShot : LinearShot
{
    [SerializeField] float sineAmp = 0.5f;
    [SerializeField] float sineFreq = 3f;
    Vector3 baseDirection;
    float phaseOffset;

    public override void Init(Vector2 _velocity, int _damage, float _lifetime, int _penetration, float _fireRate, string _target) {
        baseDirection = _velocity.normalized;
        phaseOffset = Random.value * Mathf.PI * 2;
        base.Init(_velocity, _damage, _lifetime, _penetration, _fireRate, _target);
    }

    protected override void Update() {
        Vector2 move = velocity;
        Vector2 perp = new Vector2(-baseDirection.y, baseDirection.x);

        float sine = Mathf.Sin(Time.time * sineFreq + phaseOffset);
        move += perp * sineAmp * sine;

        transform.position += (Vector3)move * Time.deltaTime;
    }
}
