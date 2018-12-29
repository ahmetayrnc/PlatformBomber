using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour {

	public TextMeshProUGUI gameOver;
	public TextMeshProUGUI score;
	public TextMeshProUGUI best;
	
	void Start()
	{
		best.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () 
	{
		score.text = PlayerStats.score + "";
		gameOver.text = Math.Round((double)PlayerStats.platformsHopped / GameManager.Instance.levelMilestone * 100, 0) + "% PASSED";
	}

	public void ShowNewBestScore()
	{
		best.text = "BEST " + PlayerStats.score;
		best.gameObject.SetActive(true);
	}
	
	public void RestartGame()
	{
		GameManager.Instance.RestartGame();
	}
}
