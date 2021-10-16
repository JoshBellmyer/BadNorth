using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    public event Action switchMenuEvent;

    [SerializeField] List<PlayerMenu> menus = new List<PlayerMenu>();

    PlayerMenu activeMenu;

    private void Start()
    {
        SwitchMenu(typeof(OverlayMenu));
    }

    public void SwitchMenu(Type type)
    {
        foreach (PlayerMenu menu in menus)
        {
            if(menu.GetType() == type)
            {
                activeMenu = menu;
                menu.gameObject.SetActive(true);
            }
            else
            {
                menu.gameObject.SetActive(false);
            }
        }
        switchMenuEvent();
    }

    public T GetMenu<T>() where T : PlayerMenu
    {
        foreach (PlayerMenu menu in menus)
        {
            var specificMenu = menu as T;
            if (specificMenu != null)
            {
                return specificMenu;
            }
        }
        return null;
    }

    public GameObject SelectNavigationStart()
    {
        return activeMenu.navigationStart;
    }
}