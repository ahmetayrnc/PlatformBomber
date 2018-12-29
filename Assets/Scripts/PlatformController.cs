using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class PlatformController : MonoBehaviour
{
	public static PlatformController Instance;

	public PlayerController playerController;
	
	public GameObject platformPrefab;
	public Transform platformParent;

	public int totalPlatformCount;
	
	public List<GameObject> activePlatforms;
	public List<GameObject> inactivePlatforms;
	
	public int activePlatformCount;
	
	[Space]
	public float minPlatformGap;
	public float maxPlatformsGap;

	[Space]
	public Vector3 firstPlatformPos = new Vector3(0,0,-7.5f);
	public Vector3 secondPlatformPos = new Vector3(0,0,7.5f);
	
	public Vector3 lastPlatformPosition = new Vector3(0,0,15);

	public Material regularMaterial;
	public Material endMatarial;
	
	void Awake()
	{
		Instance = this;
	}
	
	// Use this for initialization
	void Start ()
	{
		CreatePlatforms();
		PlaceInitialPlatforms();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void CreatePlatforms()
	{
		for (int i = 0; i < totalPlatformCount; i++)
		{
			inactivePlatforms.Add(
				Instantiate(platformPrefab,platformParent));
			inactivePlatforms[i].SetActive(false);
		}
	}
	
	void PlaceInitialPlatforms()
	{
		CreatePlatform(firstPlatformPos, false, false, false);

		if (!MainMenuTracker.Instance.mainMenu)
		{
			CreatePlatform(secondPlatformPos, true, false, false);
		}
		else
		{
			CreatePlatform(secondPlatformPos, false, false, false);
		}
		
		for (int i = 0; i < activePlatformCount; i++)
		{
			CreateNextPlatform(false);
		}
		
		playerController.SetUpFirstPlatforms(activePlatforms[0].transform,activePlatforms[1].transform);

		if (MainMenuTracker.Instance.mainMenu)
		{
			activePlatforms[1].transform.gameObject.SetActive(false);
		}
	}
	
	Transform CreatePlatform( Vector3 _position, bool animated, bool moving, bool shrinking )
	{
		if (inactivePlatforms.Count == 0)
		{
			inactivePlatforms.Add(activePlatforms[0]);
			activePlatforms.RemoveAt(0);
		}
		GameObject platformToBeCreated = inactivePlatforms[0];

		platformToBeCreated.SetActive(true);
		
		ResetPlatform(platformToBeCreated.transform);
		
		//coming from the ground
		if (animated)
		{
			platformToBeCreated.transform.position = _position + Vector3.up * -20f;
			platformToBeCreated.transform.DOMove(_position, 0.5f);			
		}
		else
		{
			platformToBeCreated.transform.position = _position;
		}

		if (moving)
		{
			float moveAmount = 2.2f;
			//move the platform
			if (_position.x.Equals(lastPlatformPosition.x))
			{
				platformToBeCreated.transform.DOMoveZ(_position.z - moveAmount, 0.8f).SetEase(Ease.InOutSine).OnComplete(() =>
				{
					platformToBeCreated.transform.DOMoveZ(_position.z + moveAmount, 1.6f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
				});
			}
			
		}
		else if (shrinking)
		{
			//shrink the platform
			var scaleVector = new Vector3(0.8f, 1, 0.8f);
			platformToBeCreated.transform.DOScale(scaleVector, 1).SetLoops(-1, LoopType.Yoyo);
		}
		
		lastPlatformPosition = _position;
		
		inactivePlatforms.RemoveAt(0);
		activePlatforms.Add(platformToBeCreated);

		return platformToBeCreated.transform;
	}

	public void CreateNextPlatform(bool end)
	{
		Vector3 position = lastPlatformPosition;
		int isRight = Random.Range(0, 2);
		float distance = Random.Range(minPlatformGap,maxPlatformsGap);
		
		
		if (isRight == 0)
		{
			position.x -= distance;
		}
		else if (isRight == 1)
		{
			position.z += distance;
		}
		/*
		if (PlayerStats.platformsHopped == GameManager.Instance.levelMilestone - 1)
		{
			Transform _platform = CreatePlatform(position, true, false, false);
			_platform.GetComponentInChildren<Renderer>().material = endMatarial;
			return;
		}
		*/

		if (end)
		{
			Transform _platform = CreatePlatform(position, true, false, false);
			_platform.GetComponentInChildren<Renderer>().material = endMatarial;
			return;
		}
		
		//create dull platform if level < 5
		if (PlayerStats.level < 5)
		{
			CreatePlatform(position, true, false, false);
		}
		else
		{
			int specialPlatformThreshold = 30;
			
			float randomNumber = Random.Range(0f, 100f);
			
			//Debug.Log("randomNumber: " + randomNumber);
			//Debug.Log("moving: : " + (movingPlatformThreshold + PlayerStats.levelHardnessMultiplier));
			//Debug.Log("shrinking: : " + (shrinkingPlatformThreshold + PlayerStats.levelHardnessMultiplier));

			if (randomNumber < specialPlatformThreshold + PlayerStats.levelHardnessMultiplier)
			{
				randomNumber = Random.Range(0f, 100f);
				int movingPlatformThreshold = 100;
				int shrinkingPlatformThreshold = 50;
				
				//moving platform
				if (randomNumber < shrinkingPlatformThreshold)
				{
					CreatePlatform(position, true, true, false);
				}
				//shrinking platform
				else if (randomNumber < movingPlatformThreshold)
				{
					CreatePlatform(position, true, false, true);
				}
			}
			//dull platform
			else
			{
				CreatePlatform(position, true, false, false);
			}
		}
	}

	public void CreateFarPlatform(bool end)
	{
		Vector3 position = lastPlatformPosition;
		int isRight = Random.Range(0, 2);
		float distance = 4 * ((minPlatformGap + maxPlatformsGap) * 0.5f);//Random.Range(minPlatformGap,maxPlatformsGap) * 5;
		
		if (isRight == 0)
		{
			position.x -= distance;
		}
		else if (isRight == 1)
		{
			position.z += distance;
		}
		
		if (end)
		{
			Transform _platform = CreatePlatform(position, true, false, false);
			_platform.GetComponentInChildren<Renderer>().material = endMatarial;
			return;
		}
		
		CreatePlatform(position, true, false, false);
	}
	
	public void ResetPlatform(Transform _platform)
	{
		_platform.GetComponentInChildren<Renderer>().material = regularMaterial;
		_platform.DOKill();
		_platform.localScale = Vector3.one;
		_platform.GetChild(1).gameObject.SetActive(true);
	}

	public void ClearPlatforms(Transform current, Transform next)
	{
		foreach (Transform child in platformParent)
		{
			if (child != current && child != next)
			{
				child.gameObject.SetActive(false);
			}
		}

		current.GetComponentInChildren<Renderer>().material = regularMaterial;
	}
	
}
