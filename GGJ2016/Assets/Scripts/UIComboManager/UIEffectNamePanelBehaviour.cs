using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIEffectNamePanelBehaviour : MonoBehaviour 
{
	public Text titleText;

	public Color healColor;
	public Color inflictColor;

	public void SetTitle(string title, bool heal)
	{
		titleText.text = title;
		this.GetComponent<Image>().color = heal ? healColor : inflictColor;
	}
}
