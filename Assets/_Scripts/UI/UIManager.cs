using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    Text winnerText;
    Text backToTitleCountdown;
    GameObject backGround;

    void Start()
    {
        winnerText = transform.Find("Winner Text").GetComponent<Text>();
        backToTitleCountdown = transform.Find("Countdown").GetComponent<Text>();
        backGround = transform.Find("Background").gameObject;
    }

    public void DisplayWinner(string winningColor)
    {
        backGround.SetActive(true);
        winnerText.text = $"The {winningColor} team wins!";
    }

    public void UpdateCountdown(int time)
    {
        backToTitleCountdown.text = $"Returning to title screen in {time}...";
    }
}
