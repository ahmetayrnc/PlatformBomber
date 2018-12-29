using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Platform : MonoBehaviour
{
	public GameObject perfect;
	public GameObject perfectParticle;
	public Renderer platformRenderer;
	public Material regularMaterial;
	public Material perfectMaterial;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void PerfectPulse(Transform player)
	{
		platformRenderer.material = perfectMaterial;
		
		/*
		transform.DOMoveY(-0.35f, 0.35f).SetEase(Ease.OutSine).OnComplete(() =>
			{
				transform.DOMoveY(0f, 0.35f).SetEase(Ease.InSine);
			});
		
		player.DOMoveY(-0.35f, 0.35f).SetEase(Ease.OutSine).OnComplete(() =>
		{
			player.DOMoveY(0f, 0.35f).SetEase(Ease.InSine);
		});
		*/
		
		StartCoroutine(PerfectPulseCor());
		/*
		tempMaterial.CopyPropertiesFromMaterial(regularMaterial);
		platformRenderer.material = tempMaterial;
		var mainColor = platformRenderer.material.GetColor("Color_536EA3B3");
		DOTween.To(()=> mainColor, x=> mainColor = x, Color.white, 1f).OnComplete(()=>
		{
			Debug.Log("done");
		});
		*/

	}

	IEnumerator PerfectPulseCor()
	{
		perfectParticle.SetActive(true);
		
		//perfectMaterial.CopyPropertiesFromMaterial(regularMaterial);
		
		//var mainColor = platformRenderer.material.GetColor("Color_536EA3B3");
		//var oldColor = mainColor;
		//platformRenderer.material.SetColor("Color_536EA3B3", Color.white );
		perfect.SetActive(false);
		yield return new WaitForSeconds(2f);
		
		//platformRenderer.material.SetColor("Color_536EA3B3", mainColor);
		
		perfectParticle.SetActive(false);
	}
}
