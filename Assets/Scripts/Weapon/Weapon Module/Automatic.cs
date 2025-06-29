using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Automatic : WeaponModule<AutomaticData> {
    public bool semi;
    
    public Automatic(AutomaticData def, Transform fp, MonoBehaviour r, string t) : base(def, fp, r, t) {}

    protected override void Fire() {
        float fireInt = fireTimer.FireRate / ProjectileCount * data.offset;
        //Debug.Log($"FireRate: {fireTimer.FireRate}, ProjectileCount: {ProjectileCount}, semiOffset: {semiOffset}");
        runner.StartCoroutine(BurstFire(fireInt));
    }

    IEnumerator BurstFire(float interval) {
        for (int i = 0; i < ProjectileCount; i++) {
            float angle = Random.Range(-SpreadAngle / 2f, SpreadAngle / 2f);
            Quaternion rotation = firePoint.rotation * Quaternion.Euler(0f, 0f, angle);

            Fire(rotation);
            if (runner == null) break;

            yield return new WaitForSeconds(interval);
        }
    }

    public float SpreadAngle =>
        data.baseSpreadAngle + data.luSpreadAngle * (Level - 1);
        
}
