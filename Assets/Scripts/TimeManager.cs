using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using System;

public class TimeManager : InstancableNB<TimeManager>
{
    [SerializeField] private TextMeshProUGUI timerText;
    private float timer = 299f;

    [SyncVar] string minutesText;
    [SyncVar] string secondsText;

    bool isTicking;

    [Server]
    public void StartTicking()
    {
        isTicking = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isServer && isTicking)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
                timer = 0;
            float minutes = Mathf.Floor(timer / 60);
            float seconds = Mathf.RoundToInt(timer % 60);
            minutesText = minutes.ToString();

            if (seconds < 10)
            {
                secondsText = "0" + Mathf.RoundToInt(seconds).ToString();
            }
            else
            {
                secondsText = Mathf.RoundToInt(seconds).ToString();
            }
        }

        timerText.text = minutesText + ":" + secondsText;

    }
}
