using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuTracker : MonoBehaviour
{
	public static MainMenuTracker Instance;
	
	public bool mainMenu;
	
	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(this);
		}

		if (Instance != this)
		{
			Destroy(this);
		}

		mainMenu = true;
	}

}
