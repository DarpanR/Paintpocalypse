using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OperationType {
    None,
    Addition,
    Multiplication,
}

public interface IoperationStrategy {
    StatType Type { get; }
    float Calculate(float value);
}

public class AddOperation : IoperationStrategy {
    StatType type;
    float value;

    public AddOperation(StatType type, float value) {
        this.type = type;
        this.value = value;
    }

    public StatType Type => type;

    public float Calculate(float value) => value + this.value;
}
public class MultiplyOperation : IoperationStrategy {
    StatType type;
    float value;

    public MultiplyOperation(StatType type, float value) {
        this.type = type; 
        this.value = value;
    }

    public StatType Type => type;

    public float Calculate(float value) => value * this.value;
}

public static class OperationFactory {
    public static IoperationStrategy GetOperation(OperationType operationType, StatType statType, float value) {
        switch (operationType) {
            case OperationType.Addition: return new AddOperation(statType, value);
            case OperationType.Multiplication: return new MultiplyOperation(statType, value);
            default: return null;
        }
    }
}