using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    public int maxLevel = 5;
    public float fireRate = 1f;

    [Header("Projectile Settings")]
    public GameObject projectile;
    public List<Transform> firePoints;
    public Vector2 velocity = new Vector2(20,0);
    public float damage = 10f;
    public float lifetime = 10f;
    public int penetration = 1;

    float timer = 0f;
    int level = 1;

    public void TryFire(Vector3 position) {
        timer += Time.deltaTime;

        if (timer >= 1f / fireRate) {
            Fire(position);
            timer = 0f;
        }
    }

    protected virtual void Fire(Vector3 position) {
        foreach (Transform fp in firePoints) {
            BaseProjectile proj = Instantiate(projectile, position + fp.position, fp.rotation).GetComponent<BaseProjectile>();
            proj.Init(velocity, damage, lifetime, penetration);
        }
    }

    public virtual void LevelUp() {
        level = Mathf.Clamp(level + 1, 1, maxLevel);
    }

    public virtual void Upgrade() {
        throw new System.NotImplementedException();
    }
}