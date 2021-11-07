using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class PlayerMenu : MonoBehaviour
{
    public GameObject navigationStart;
    protected Player player;
    protected PlayerUIManager playerUIManager;

    protected void Awake()
    {
        FindPlayer();
        FindPlayerUIManager();
    }

    private void FindPlayer()
    {
        Transform parent = transform.parent;
        do
        {
            player = parent.GetComponent<Player>();
            parent = parent.parent;
        }
        while (player == null && parent != null);

        if(player == null)
        {
            Debug.LogError("PlayerMenu is not a child of a Player!");
        }
    }

    private void FindPlayerUIManager()
    {
        Transform parent = transform.parent;
        do
        {
            playerUIManager = parent.GetComponent<PlayerUIManager>();
            parent = parent.parent;
        }
        while (playerUIManager == null && parent != null);

        if (playerUIManager == null)
        {
            Debug.LogError("PlayerMenu is not a child of a PlayerUIManager!");
        }
    }
}
