using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformColorController : MonoBehaviour
{
	public static PlatformColorController Instance;
	
	public Material platformMaterial;
	public Material perfectPlatformMaterial;
	public Material backgroundMaterial;
	public Material perfectMaterial;
	public Material ballMaterial;

	//public Color currentPerfectPlatformMaterial;
	public Color currentPlatformColor;
	public Color currentBackgroundColorTop;
	public Color currentBackgroundColorBottom;
	public Color currentPerfectColor;

	public ColorHolder[] colors;
	public int colorIndex;

	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		if (!PlayerPrefs.HasKey("colorIndex"))
		{
			PlayerPrefs.SetInt("colorIndex", 0);
			PlayerPrefs.Save();
		}

		colorIndex = PlayerPrefs.GetInt("colorIndex");

		SetColors();
		ApplyColors();
	}
	
	public void ChangeColor()
	{
		colorIndex++;
		if (colorIndex >= colors.Length)
		{
			colorIndex = 0;
		}
		
		PlayerPrefs.SetInt("colorIndex", colorIndex);
		PlayerPrefs.Save();

		SetColors();
		ApplyColors();
	}

	public void SetColors()
	{
		currentPlatformColor = colors[colorIndex].platformColor;
		//currentPerfectColor = colors[colorIndex].perfectPlatformColor;
		currentBackgroundColorTop = colors[colorIndex].backgroundColorTop;
		currentBackgroundColorBottom = colors[colorIndex].backgroundColorBottom;
		currentPerfectColor = colors[colorIndex].perfectColor;
	}
	
	public void ApplyColors()
	{
		platformMaterial.SetColor("Color_536EA3B3", currentPlatformColor);
		platformMaterial.SetColor("Color_724635EE", currentBackgroundColorBottom);
		perfectPlatformMaterial.SetColor("Color_536EA3B3", currentPerfectColor);
		perfectPlatformMaterial.SetColor("Color_724635EE", currentBackgroundColorBottom);
		backgroundMaterial.SetColor("Color_219D79F1", currentBackgroundColorTop);
		backgroundMaterial.SetColor("Color_A5B4057B", currentBackgroundColorBottom);
		ballMaterial.SetColor("Color_724635EE", currentBackgroundColorBottom);
		perfectMaterial.color = currentPerfectColor;
	}
}
