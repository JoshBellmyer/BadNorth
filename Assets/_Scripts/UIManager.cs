using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	public static UIManager instance;

	[SerializeField] GameObject pauseScreen;

	private void Start()
	{
		if (instance == null || instance == this)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);

		pauseScreen.SetActive(false);
	}

	public void Pause()
    {
		pauseScreen.SetActive(true);
    }

	public void Unpause()
	{
		pauseScreen.SetActive(false);
	}
}
