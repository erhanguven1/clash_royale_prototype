using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    private float timer = 299f;

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
            timer = 0;
        float minutes = Mathf.Floor(timer / 60);
        float seconds = Mathf.RoundToInt(timer%60);
        string minutesText;
        string secondsText;
        minutesText = minutes.ToString();
        
        if(seconds < 10) {
            secondsText  = "0" + Mathf.RoundToInt(seconds).ToString();
        }
        else
        {
            secondsText  = Mathf.RoundToInt(seconds).ToString();
        }
        timerText.text = minutesText + ":" + secondsText;
    }
}
