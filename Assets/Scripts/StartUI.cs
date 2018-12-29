using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StartUI : MonoBehaviour
{

	public TextMeshProUGUI bestText;
	
	public void StartGame()
	{
		MainMenuTracker.Instance.mainMenu = false;
		GameManager.Instance.RestartGame();
	}

	void Update()
	{
		bestText.text = "Best " + PlayerStats.best + "";
	}
	
}
