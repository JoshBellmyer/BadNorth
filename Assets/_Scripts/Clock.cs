using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clock : MonoBehaviour
{
    public static Clock instance;

    public float startTime;
    float time;
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
        time = startTime;
    }

    private void Update()
    {
        if (!finished)
        {
            time -= Time.deltaTime;
            if (time <= 0)
            {
                finished = true;
                clockFinished();
            }
            text.text = (int)time + "";
        }
    }
}
