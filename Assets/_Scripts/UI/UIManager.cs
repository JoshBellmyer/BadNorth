using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    Text winnerText;
    Text backToTitleCountdown;
    GameObject backGround;

    public static UIManager instance;


    void Start()
    {
        if (instance == null || instance == this) {
            instance = this;
        }
        else {
            Destroy(gameObject);

            return;
        }

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
