using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OperationType {
    None,
    Addition,
    Multiplication,
}

public interface IoperationStrategy<T> {
    T Type { get; }
    float Value { get; }
    float Calculate(float value);
}

public class AddOperation<T> : IoperationStrategy<T> {
    T type;
    float value;

    public AddOperation(T type, float value) {
        this.type = type;
        this.value = value;
    }

    public T Type => type;
    public float Value => value;
    public float Calculate(float value) => value + this.value;
}
public class MultiplyOperation<T> : IoperationStrategy<T> {
    T type;
    float value;

    public MultiplyOperation(T type, float value) {
        this.type = type; 
        this.value = value;
    }

    public T Type => type;
    public float Value => value;
    public float Calculate(float value) => value * this.value;
}

public static class OperationFactory<T> {
    public static IoperationStrategy<T> GetOperation(OperationType operationType, T statType, float value) {
        switch (operationType) {
            case OperationType.Addition: return new AddOperation<T>(statType, value);
            case OperationType.Multiplication: return new MultiplyOperation<T>(statType, value);
            default: return null;
        }
    }
}