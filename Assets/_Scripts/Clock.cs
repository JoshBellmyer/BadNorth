using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Clock : NetworkBehaviour
{
    public static Clock instance;

    public Text[] winTexts;

    public float startTime;
    public NetworkVariable<float> time;
    private float localTime;
    public float CurrentTime
    {
        get => Game.online ? time.Value : localTime;
        set {
            if (Game.online)
            {
                time.Value = value;
            }
            else
            {
                localTime = value;
            }
        }
    }

    Text text;
    public bool finished;

    public event Action clockFinished;

    private void Start()
    {
        if (instance == null || instance == this)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        text = transform.GetComponentInChildren<Text>();
        if(!Game.online || Game.isHost)
        {
            time.Value = startTime;
        }
    }

    private void Update()
    {
        if (Game.instance.isPaused) {
            return;
        }

        if (!Game.online || Game.isHost)
        {
            if (!finished)
            {
                CurrentTime -= Time.deltaTime;
                if (time.Value <= 0)
                {
                    finished = true;
                    clockFinished();
                }
            }
        }
        text.text = (int)time.Value + "";
    }
}
