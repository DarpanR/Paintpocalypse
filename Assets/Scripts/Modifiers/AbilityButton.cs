using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour
{
    public Button button;
    IMouseAbility ability;

    float curCD;

    private void Awake() {
        button = button ? button : GetComponent<Button>();
        ability = GetComponent<IMouseAbility>();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => AbilityManager.Instance.SelectAbility(ability));
    }
}
