using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Settings settings;

    static int numPlayers = 0;
    int playerId;

    private void Awake()
    {
        numPlayers++;
        playerId = numPlayers;
    }

    private void Start()
    {
        settings = Settings.Load(playerId);
    }
}
