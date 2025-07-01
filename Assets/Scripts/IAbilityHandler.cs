using System;

public interface IAbilityHandler {
    float TotalUsage { get; }
    float RemainingUsage { get; }

    event Action OnAbilityEnd;
    void Init(IPickupData pickupData, bool dropIt = false);

}
