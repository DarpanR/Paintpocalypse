using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBarPanel : MonoBehaviour
{
    public ModifierDefinition def1;
    public ModifierDefinition def2;

    /// I dont like this part where the new () has to be hard coded
    /// if someone knows how to make this more generic do it!
    public void GenerateModifier() {
        MouseController.Instance.SetModifier(ModifierFactory.CreateModifier(def1));
    }
}
