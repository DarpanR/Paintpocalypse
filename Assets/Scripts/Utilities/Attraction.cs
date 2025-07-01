using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttractor {
    Transform GetAttractor();
    event Action OnAttractionEnd;
}

public interface IAttractable {
    void SetOverrideTarget(Transform overrideTarget);
    void OnAttractionEnd();
}