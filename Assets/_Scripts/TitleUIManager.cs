using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TitleUIManager : MonoBehaviour
{
    public static TitleUIManager instance;
    [SerializeField] List<UIScreen> uiScreens;
	EventSystem eventSystem;

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

		eventSystem = GetComponent<EventSystem>();

		SetUIScreen("Title Screen");
	}

	public void SetUIScreen(string uiName)
    {
		foreach(UIScreen screen in uiScreens)
        {
			GameObject go = screen.gameObject;
			if(go.name == uiName)
            {
				go.SetActive(true);
				SetSelected(screen.firstSelected);
            }
            else
            {
				go.SetActive(false);
            }
        }
    }

	public void SetSelected(GameObject selected)
    {
		eventSystem.SetSelectedGameObject(selected, new BaseEventData(eventSystem));
	}

	public GameObject GetSelected()
    {
		return eventSystem.currentSelectedGameObject;
    }
}
