using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Status Effect")]
public class StatusEffectDefinition : ScriptableObject {
    public List<StatusFlashEffect> flashEffects;
}
