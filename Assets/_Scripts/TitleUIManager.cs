using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TitleUIManager : MonoBehaviour
{
    TitleUIManager instance;
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
				eventSystem.SetSelectedGameObject(screen.firstSelected, new BaseEventData(eventSystem));
            }
            else
            {
				go.SetActive(false);
            }
        }
    }
}
