using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IoperationStrategy {
    float Calculate(float value);
}

public class AddOperation : IoperationStrategy {
    readonly float value;

    public AddOperation(float value) {
        this.value = value;
    }

    public float Calculate(float value) => value + this.value;
}
public class MultiplyOperation : IoperationStrategy {
    readonly float value;

    public MultiplyOperation(float value) {
        this.value = value;
    }

    public float Calculate(float value) => value * this.value;
}