using System.Collections.Generic;
using UnityEngine;

public class Spread : WeaponModule<SpreadDefinition> {
    public Spread(SpreadDefinition def, Transform fp, string t) : base(def, fp, t) {}

    protected override void Fire() { 
        foreach (var rots in UpdateSpawnRotations())
            Fire(rots);
    }

    public IEnumerable<Quaternion> UpdateSpawnRotations() {
        var rots = new List<Quaternion>();

        if (ProjectileCount <= 1) {
            rots.Add(firePoint.rotation);
            return rots;
        }
        float step = SpreadAngle / (ProjectileCount - 1);
        float start = -SpreadAngle / 2f;

        for (int i = 0; i < ProjectileCount; i++) {
            float ang = start + step * i;
            // rotate the firePoint’s forward by +ang degrees around Z
            rots.Add(firePoint.rotation * Quaternion.Euler(0, 0, ang));
        }
        return rots;
    }

    public float SpreadAngle =>
        Definition.baseSpreadAngle + Definition.luSpreadAngle * (Level - 1);
        
}
