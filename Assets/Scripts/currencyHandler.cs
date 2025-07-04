using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CurrencyType { EXP, Coin }

public class currencyHandler : MonoBehaviour
{
    public CurrencyType type;
    public int amount;
    public float shutoff;
    public ParticleSystem particle;
    public Collider2D coll;
    CountdownTimer emitterShutoff;


    void Start()
    {
        //particle = transform.Find("Particle").GetComponent<ParticleSystem>();
        coll.enabled = false;

        emitterShutoff = new CountdownTimer(shutoff);
        emitterShutoff.OnTimerStop += () => {
            var main = particle.main;
            main.gravityModifier = 0;

            var collision = particle.collision;
            collision.enabled = false;

            var forceField = particle.externalForces;
            forceField.enabled = true;

            coll.enabled = true;
        };
    }

    // Update is called once per frame
    void Update()
    {
        emitterShutoff.Tick(Time.deltaTime);

        if (particle.emission.enabled && !particle.isEmitting) {
            var emission = particle.emission;
            emission.enabled = false;

            emitterShutoff.Start();
        }
    }
}
