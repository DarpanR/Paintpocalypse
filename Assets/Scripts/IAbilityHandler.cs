using System;

public interface IAbililtyHandler {
    event Action OnAbilityEnd;
    void Init(IPickupData pickupData, bool dropIt = false);
}
