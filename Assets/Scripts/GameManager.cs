using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	[Space]
	public StartUI startUI;
	public GameUI gameUI;
	public GameOverUI gameOverUI;
	public LevelTransitionUI levelTransitionUI;
	
	[Space]
	public int levelMilestone;
	public int perfectFlyMilestone;
	public int levelHardnessMultiplier;
	
	[Space]
	public bool gameIsOver;
	public bool gamePaused;
	public bool mainMenu;
	
	void Awake()
	{
		Instance = this;
		Application.targetFrameRate = 60;
	}
	
	// Use this for initialization
	void Start ()
	{
		mainMenu = MainMenuTracker.Instance.mainMenu;

		if (mainMenu)
		{
			startUI.gameObject.SetActive(true);
			gameUI.gameObject.SetActive(false);
			gamePaused = true;
		}
		else
		{
			startUI.gameObject.SetActive(false);
			gameUI.gameObject.SetActive(true);
			gamePaused = false;
		}
		
		levelTransitionUI.gameObject.SetActive(false);
		gameOverUI.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () 
	{
		levelHardnessMultiplier = PlayerStats.levelHardnessMultiplier;
	}

	public void CheckNewLevel()
	{
		if (PlayerStats.platformsHopped >= levelMilestone)
		{
			NextLevel();
		}	
	}
	
	public void NextLevel()
	{
		PlayerStats.IncrementLevel();
		//PlatformColorController.Instance.ChangeColor();
	}

	public void LevelTransition()
	{
		StartCoroutine(LevelTransitionCor());
	}

	IEnumerator LevelTransitionCor()
	{
		gameUI.gameObject.SetActive(false);
		levelTransitionUI.gameObject.SetActive(true);
		yield return new WaitForSeconds(2f);
		gameUI.gameObject.SetActive(true);
		levelTransitionUI.gameObject.SetActive(false);
	}
	
	public void EndGame()
	{
		gameIsOver = true;
		
		gameUI.gameObject.SetActive(false);
		gameOverUI.gameObject.SetActive(true);
		
		if (PlayerStats.score > PlayerStats.best)
		{
			gameOverUI.ShowNewBestScore();
			PlayerStats.best = PlayerStats.score;
			PlayerPrefs.SetInt("best", PlayerStats.best);
			PlayerPrefs.Save();
		}
	}
	
	public void RestartGame()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
