using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public enum CurrencyType { Exp, Coin }

public class CurrencyPickup : MonoBehaviour
{
    public CurrencyType type;
    public int amount;
    public Color[] tierColors;
    public ParticleSystem pSystem;

    //Collider2D targetCol;
    bool settled = false;
    GameObject player;

    void Start() {
        pSystem = pSystem != null ? pSystem : transform.Find("Particle").GetComponent<ParticleSystem>();
        player = GameObject.FindWithTag("Player");

        var trigger = pSystem.trigger;
        trigger.AddCollider(player.transform);
    }

    // Update is called once per frame
    void Update() {
        if (settled && pSystem.main.duration > pSystem.main.startLifetime.constant) {
            Debug.Log(pSystem.main.duration);
            //Despawn();
        }

        if (pSystem.IsAlive()) {
            // Still alive (could be emitting or just particles floating)
            if (!pSystem.isEmitting && !settled) {
                // Emission has stopped; check particle velocities
                var particles = new ParticleSystem.Particle[pSystem.main.maxParticles];
                int count = pSystem.GetParticles(particles);

                bool allStopped = true;
                
                for (int i = 0; i < count; i++)
                    if (particles[i].velocity.sqrMagnitude > 0.05f) {
                        allStopped = false;
                        break;
                    }

                if (count > 0 && allStopped) {                    
                    // All particles settled down
                    var main = pSystem.main;
                    main.gravityModifier = 0;

                    var collision = pSystem.collision;
                    collision.enabled = false;

                    var forces = pSystem.externalForces;
                    forces.enabled = true;

                    forces.AddInfluence(player.transform.Find("Forcefield").GetComponent<ParticleSystemForceField>());

                    settled = true;
                    // Optionally enable collider here if you still wanted it
                }
            }
        } else {
            AwardCurrency();
        }
    }

    public void Init(int amount) {
        
        this.amount = amount;

        /// reduce the amount of particle to leading digit
        float trailingZeros = Mathf.Floor(Mathf.Log10(amount));
        float magnitude = Mathf.Pow(10, trailingZeros);
        int leadDigit = Mathf.FloorToInt(amount / magnitude);

        var main = pSystem.main;
        /// debuging change to lead digit later
        main.maxParticles = UnityEngine.Random.Range(leadDigit, 5);

        int tier = Mathf.FloorToInt(trailingZeros % tierColors.Length);
        main.startColor = tierColors[tier - 1];
    }

    void AwardCurrency() {
        switch (type) {
            case CurrencyType.Exp:
                GameEvents.RaiseExpBarUpdate(amount);
                break;
            case CurrencyType.Coin:
                GameEvents.RaiseCoinCollected(amount);
                break;
        }
        Despawn();
    }

    void Despawn() {
        /// maybe send back to pool
        Destroy(gameObject);
    }
}
