using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    Text winnerText;
    Text backToTitleCountdown;

    void Start()
    {
        winnerText = transform.Find("Winner Text").GetComponent<Text>();
        backToTitleCountdown = transform.Find("Countdown").GetComponent<Text>();
    }

    public void DisplayWinner(string winningColor)
    {
        winnerText.text = $"The {winningColor} team wins!";
    }

    public void UpdateCountdown(int time)
    {
        backToTitleCountdown.text = $"Returning to title screen in {time}...";
    }
}
