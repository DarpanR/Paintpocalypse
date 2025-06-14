using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float timer;

    void Update()
    {
        if (timer > 0)
            timer -= Time.deltaTime; // Decrease timer by the time passed since last frame
        else if (timer < 0)
            timer = 0;
        
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);

        if (timer <= 60)        // Change color to red when timer is below 60s
            timerText.color = Color.red;

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
