using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IResettable {
    void Reset();
}

public interface ITickable {
    void Tick(float deltaTime);
}

public interface IRemovable {
    bool Remove { get; }
}
