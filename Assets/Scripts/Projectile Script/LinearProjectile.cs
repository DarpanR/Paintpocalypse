using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearProjectile : BaseProjectile {
    protected override void FireProjectile() {
        transform.Translate(velocity * Time.deltaTime);
    }
}
