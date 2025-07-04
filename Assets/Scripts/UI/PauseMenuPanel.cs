
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuPanel : VictoryScreenPanel
{
    public void Resume() {
        GameEvents.RaiseResume();
    }
}
