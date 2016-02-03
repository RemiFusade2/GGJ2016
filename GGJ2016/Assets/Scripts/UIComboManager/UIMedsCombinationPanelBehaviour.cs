using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

[Serializable]
public struct MedIconAndName 
{
	public string name;
	public Sprite image;
}

public class UIMedsCombinationPanelBehaviour : MonoBehaviour 
{
	public List<MedIconAndName> medsIconFromNameList;

	public Dictionary<string, Sprite> medsIconFromNameDico;

	public List<Image> medsImagesList;
	public List<Text> medsNumberList;


	private void PopulateDico()
	{
		if (medsIconFromNameDico == null)
		{
			medsIconFromNameDico = new Dictionary<string, Sprite> ();
			foreach (MedIconAndName medIconAndName in medsIconFromNameList)
			{
				medsIconFromNameDico.Add(medIconAndName.name, medIconAndName.image);
			}
		}
	}

	public void SetMedsCombo(List<string> names, List<int> quantities, bool visible)
	{
		PopulateDico ();

		if (!visible)
		{
			this.GetComponent<Image>().color = new Color(0.6f,0.6f,0.6f,0.7f);
		}

		for (int index = 0 ; index < names.Count ; index++)
		{
			try
			{				
				string name = names[index];
				medsImagesList[index].sprite = medsIconFromNameDico[name];
				medsImagesList[index].color = visible ? Color.white : Color.black;
				medsNumberList[index].text = visible ? ("x"+quantities[index]) : "x?";
			}
			catch (Exception ex)
			{
				Debug.Log("Exception");
			}
		}
	}
}
