using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverMenu : PlayerMenu
{
    [SerializeField] Text message;

    protected new void Awake()
    {
        base.Awake();
        Game.instance.OnDeclareWinner += ShowGameOver;
    }

    private void ShowGameOver(string team)
    {
        if(team == player.playerId.ToString())
        {
            message.text = "You Won!";
        }
        else
        {
            message.text = "You Lost!";
        }
    }

    public void OnMainMenuButton()
    {
        Game.instance.SwitchToMainMenu();
    }

    private void OnDestroy()
    {
        Game.instance.OnDeclareWinner -= ShowGameOver;
    }
}
