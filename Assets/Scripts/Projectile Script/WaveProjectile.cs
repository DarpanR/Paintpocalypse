using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class WaveProjectile : BaseProjectile {

    public float waveFreq = 4f;
    public float waveAmp = 0.5f;

    float prevWaveOffset;
    float elapsed = 0;
    protected override void FireProjectile() {
        elapsed += Time.deltaTime;

        float now = MathF.Sin(elapsed * waveFreq) * waveAmp;
        float delta = now - prevWaveOffset;
        prevWaveOffset = now;

        Vector2 prep = new Vector2(-velocity.y, velocity.x);

        transform.Translate(prep * delta);
    }
}
