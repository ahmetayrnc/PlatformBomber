using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DG.Tweening;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	//[HideInInspector]
	public Transform targetPlatform;
	//[HideInInspector]
	public Transform currentPlatform;
	public CameraFollow playerCam;
	
	public float maxAmount;

	public GameObject bomb;
	public GameObject boomParticlePrefab;
	
	//public Transform arrow;
	
	private Vector3 bombStartSize;
	private Vector3 arrowStartSize;

	private float timeDifference;
	
	private bool acceptInput;
	private bool mousePressed;
	private bool count;
	
	void Start()
	{
		acceptInput = true;

		bombStartSize = bomb.transform.localScale;
		//arrowStartSize = arrow.transform.localScale;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (GameManager.Instance.gameIsOver)
		{
			return;
		}

		if (GameManager.Instance.gamePaused)
		{
			return;
		}
		
		if (!acceptInput)
		{
			return;
		}

		if (count)
		{
			if (timeDifference < maxAmount)
			{
				timeDifference += Time.deltaTime;
				bomb.transform.localScale = bombStartSize + Vector3.one * timeDifference;
				//arrow.transform.localScale = arrowStartSize + Vector3.up * timeDifference;
			}
		}
		
		if (Input.GetMouseButtonDown(0))
		{
			mousePressed = true;
			count = true;
		}

		if (Input.GetMouseButtonUp(0) && mousePressed)
		{
			acceptInput = false;
			mousePressed = false;
			count = false;
			
			Jump(timeDifference);
			ResetTimes();
		}
	}

	void ResetTimes()
	{
		timeDifference = 0f;
	}
	
	void Jump( float _timeDifference )
	{
		bomb.SetActive(false);
		
		//particle
		GameObject boomParticleGO = Instantiate(boomParticlePrefab, bomb.transform.position,Quaternion.identity);
		Destroy(boomParticleGO, 2f);
		//arrow.gameObject.SetActive(false);
		
		//moving forward
		transform.DOMove(transform.position + (targetPlatform.position - transform.position).normalized * _timeDifference * PlatformController.Instance.maxPlatformsGap * 2f, 0.7f)
			.SetEase(Ease.Linear);
		
		//moving upward
		transform.DOMoveY(PlatformController.Instance.maxPlatformsGap * 0.8f, 0.35f).SetLoops(2,LoopType.Yoyo).OnComplete(() =>
		{
			RaycastHit hit;
			
			//if hit
			if (Physics.Raycast(transform.position,Vector3.down,out hit,100f))
			{
				
				if (hit.transform.CompareTag("Ground"))
				{
					Fall();
					return;
				}
				
				if ( hit.transform.parent == currentPlatform)
				{
					StartCoroutine(SetupJump(true));
					return;
				}
				
				currentPlatform = hit.transform.parent;

				currentPlatform.DOKill();
				
				PlayerStats.IncrementPlatformsHopped();
				GameManager.Instance.CheckNewLevel();
				
				if (hit.transform.CompareTag("Perfect"))
				{
					PlayerStats.IncrementMultiplier();
					GameUI.Instance.CreateMotivationPopUp(PlayerStats.multiplier);
					
					hit.transform.parent.GetComponent<Platform>().PerfectPulse(transform);
					
				}
				else
				{
					PlayerStats.ResetMultiplier();
				}
				PlayerStats.IncrementScore();
				GameUI.Instance.CreateScorePopUp(currentPlatform.position);
				
				if (PlayerStats.multiplier % GameManager.Instance.perfectFlyMilestone == 0)
				{

					if (PlayerStats.platformsHopped + 4 >= GameManager.Instance.levelMilestone)
					{
						PlatformController.Instance.CreateFarPlatform(true);
					}
					else
					{
						PlatformController.Instance.CreateFarPlatform(false);
					}
					
					
					targetPlatform = PlatformController.Instance.activePlatforms[PlatformController.Instance.activePlatforms.IndexOf(currentPlatform.gameObject) + 1].transform;
				}
				else
				{
					if (PlayerStats.platformsHopped == GameManager.Instance.levelMilestone - 1)
					{
						PlatformController.Instance.CreateNextPlatform(true);
					}
					else
					{
						PlatformController.Instance.CreateNextPlatform(false);
					}
					
					targetPlatform = PlatformController.Instance.activePlatforms[PlatformController.Instance.activePlatforms.IndexOf(currentPlatform.gameObject) + 1].transform;
				}
				
				StartCoroutine(SetupJump(false));
			}
			else
			{
				Fall();
			}
					
		});

	}

	public void Fall()
	{
		transform.DOMoveY(-20f, 0.75f).OnComplete(() =>
		{
			GameManager.Instance.EndGame();
		});
	}
	
	IEnumerator SetupJump(bool newLevel)
	{
		var resetDuration = 0.2f;
		var lookPos = targetPlatform.position - transform.position;
		lookPos.y = 0;
		var rotation = Quaternion.LookRotation(lookPos).eulerAngles;
	
		transform.DORotate(rotation, resetDuration);
		
		playerCam.MoveTheCamera(GetMiddleOfPlatforms(), resetDuration * 2);
		
		yield return new WaitForSeconds(resetDuration);
		acceptInput = true;
		bomb.transform.localScale = bombStartSize;
		
		bomb.SetActive(true);
		
		//end jump
		if (PlayerStats.platformsHopped == 0 && !newLevel)
		{
			Debug.Log("next level");
			acceptInput = false;
			GameManager.Instance.gameUI.gameObject.SetActive(false);
			yield return new WaitForSeconds(resetDuration * 2);
			EndJump();
		}
		
		if (PlayerStats.multiplier % GameManager.Instance.perfectFlyMilestone == 0)
		{
			acceptInput = false;
			yield return new WaitForSeconds(resetDuration);
			PerfectJump();	
		}
	}

	public void EndJump()
	{
		bomb.SetActive(false);
		
		GameObject boomParticleGO = Instantiate(boomParticlePrefab, bomb.transform.position,Quaternion.identity);
		Destroy(boomParticleGO, 2f);

		playerCam.transform.DOMoveY(playerCam.transform.position.y + 50f, 1f).SetEase(Ease.OutSine).OnComplete(() =>
			{
				playerCam.transform.DOMoveY(playerCam.transform.position.y - 50f, 1f).SetEase(Ease.InSine);
			});
		transform.DOMoveY(50f, 1f).SetEase(Ease.OutSine).OnComplete(() =>
			{
				PlatformColorController.Instance.ChangeColor();
				PlatformController.Instance.ClearPlatforms(currentPlatform, targetPlatform);
				transform.DOMoveY(0.5f, 1f).SetEase(Ease.InSine).OnComplete(() =>
				{
					StartCoroutine(SetupJump(true));
				});
			});

		transform.DOMoveX(currentPlatform.position.x, 2f);
		transform.DOMoveZ(currentPlatform.position.z, 2f);
		
		GameManager.Instance.LevelTransition();
		GameManager.Instance.gameUI.ClearPopUps();
		
		Debug.Log("Level Complete");

		
	}
	
	public void PerfectJump()
	{
		bomb.SetActive(false);
		
		GameObject boomParticleGO = Instantiate(boomParticlePrefab, bomb.transform.position,Quaternion.identity);
		Destroy(boomParticleGO, 2f);

		var newPos = targetPlatform.position;
		newPos.y = transform.position.y;
		transform.DOMove(newPos, 1.2f).SetEase(Ease.Linear);

		transform.DOMoveY(PlatformController.Instance.maxPlatformsGap * 0.8f, 0.6f).SetLoops(2, LoopType.Yoyo).OnComplete(
			() =>
			{
				RaycastHit hit;
				
				if (Physics.Raycast(transform.position,Vector3.down,out hit,100f))
				{
					
					currentPlatform = hit.transform.parent;

					currentPlatform.DOKill();
				
					PlayerStats.IncrementPlatformsHopped(4);
					GameManager.Instance.CheckNewLevel();
				
					if (hit.transform.CompareTag("Perfect"))
					{
						PlayerStats.IncrementMultiplier();
						GameUI.Instance.CreateMotivationPopUp(PlayerStats.multiplier);
					}
					else
					{
						PlayerStats.ResetMultiplier();
					}
					PlayerStats.IncrementScore();
					GameUI.Instance.CreateScorePopUp(currentPlatform.position);
					
					if (PlayerStats.platformsHopped == GameManager.Instance.levelMilestone - 1)
					{
						PlatformController.Instance.CreateNextPlatform(true);
					}
					else
					{
						PlatformController.Instance.CreateNextPlatform(false);
					}
					
					targetPlatform = PlatformController.Instance.activePlatforms[PlatformController.Instance.activePlatforms.IndexOf(currentPlatform.gameObject) + 1].transform;
				
					StartCoroutine(SetupJump(false));
				}
				else
				{
					Fall();
				}

			});
	}

	public void SetUpFirstPlatforms(Transform current, Transform next)
	{
		currentPlatform = current;
		targetPlatform = next;
	}

	public Vector3 GetMiddleOfPlatforms()
	{
		return (targetPlatform.position + currentPlatform.position) / 2;
	}
}
