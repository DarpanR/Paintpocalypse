using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseProjectile : MonoBehaviour {
    public Vector2 velocity;
    public float damage;
    public float lifetime;
    public int penetration;
    public int curPen;

    public void Init(Vector2 _velocity, float _damage, float _lifetime, int _pen) {
        velocity = _velocity;
        damage = _damage;
        lifetime = _lifetime;
        penetration = _pen;

        Detonte();
    }

    void Update() {
        FireProjectile();
    }

    protected abstract void FireProjectile();

    protected virtual void Detonte() {
        Destroy(gameObject, lifetime);
    }

    protected void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Enemy")) {
            curPen++;

            if (curPen >= penetration) {
                Destroy(gameObject);
            }
            EnemyAI enemy = collision.GetComponent<EnemyAI>();

            if (enemy != null) {
                enemy.TakeDamage(damage);
            }
        }
    }
}
