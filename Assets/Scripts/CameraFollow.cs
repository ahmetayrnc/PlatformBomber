using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public Transform target;
	public Vector3 offset;
	private float fixedY;
	
	// Use this for initialization
	void Start ()
	{
		fixedY = offset.y;
	}
	
	// Update is called once per frame
	void Update ()
	{
		//Vector3 newPos = target.position + offset;
		//newPos.y = fixedY;
		//transform.position = newPos;
	}

	public void MoveTheCamera(Vector3 _position, float _duration)
	{
		Vector3 newPos = _position + offset;
		newPos.y = fixedY;
		if (PlayerStats.multiplier % GameManager.Instance.perfectFlyMilestone == 0)
		{
			newPos += transform.forward * -150;
		}
		
		
		transform.DOMove(newPos, _duration);
		
	}
}
