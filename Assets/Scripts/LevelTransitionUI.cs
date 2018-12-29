using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelTransitionUI : MonoBehaviour
{

	public TextMeshProUGUI completeText;
	public TextMeshProUGUI perfectAmountText;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		completeText.text = "LEVEL " + (PlayerStats.level - 1) + " COMPLETED";
		perfectAmountText.text = PlayerStats.perfectPercentage + "% PERFECT";
	}
}
