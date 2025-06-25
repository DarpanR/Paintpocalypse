using UnityEngine;

public interface IVisitor {
    void Visit(IVisitable visitable);
}

public interface IVisitable {
    void Accept<T>(T visitor) where T : Component, IVisitor;
}