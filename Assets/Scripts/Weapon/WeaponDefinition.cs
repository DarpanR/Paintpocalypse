using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponDefinition : ScriptableObject {
    [Header("Identity & UI")]
    public Sprite weaponIcon;
    public int poolSize = 20;

    [Header("Projectile Settings")]
    public GameObject projPrefab;

    [Header("Base Stats")]
    public float baseDamage = 10f;
    public float baseFireRate = 1f;
    public Vector2 baseVelocity = new Vector2();
    public float baseLifetime = 2f;
    public int basePenetration = 1;
    public int baseProjectileCount = 1;
    public int maxLevel = 5;

    [Header("Levelling Stats")]
    public float luDamage = 10f;
    public float luFireRate = 1f;
    public Vector2 luVelocity = new Vector2();
    public float luLifetime = 2f;
    public int luPenetration = 1;
    public int luProjectileCount = 1;

    public abstract IWeaponModule CreateModule(Transform firePoint, string target);
}