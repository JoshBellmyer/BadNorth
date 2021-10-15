using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] UIController uiController;
    [SerializeField] List<PlayerMenu> menus = new List<PlayerMenu>();

    private void Start()
    {
        foreach(PlayerMenu menu in menus)
        {
            menu.gameObject.SetActive(false);
        }

        menus[0].gameObject.SetActive(true);
    }

    public void SwitchMenu(Type type)
    {
        foreach (PlayerMenu menu in menus)
        {
            if(menu.GetType() == type)
            {
                menu.gameObject.SetActive(true);
            }
            else
            {
                menu.gameObject.SetActive(false);
            }
        }
        uiController.SelectSomething();
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
}