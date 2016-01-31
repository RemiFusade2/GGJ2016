using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MedsContainersManagerBehaviour : MonoBehaviour 
{
	public List<string> allMedsNames;
	public List<GameObject> allMedsGameObjects;
	public List<GameObject> visibleMeds;

	public void ShowContainers(List<string> medsName)
	{
		foreach (string name in medsName)
		{
			int index = allMedsNames.IndexOf(name);
			allMedsGameObjects[index].SetActive(true);
			visibleMeds.Add(allMedsGameObjects[index]);
		}
	}

	public void AddRandomContainers(int count)
	{
		int currentVisibleMedsContainers = visibleMeds.Count;
		while ( (visibleMeds.Count - currentVisibleMedsContainers) < count )
		{
			int randomIndex = Random.Range(0, allMedsNames.Count);
			if (!allMedsGameObjects[randomIndex].activeSelf)
			{
				allMedsGameObjects[randomIndex].SetActive(true);
				visibleMeds.Add(allMedsGameObjects[randomIndex]);
			}
		}
	}

	public void HideAllContainers()
	{
		foreach (GameObject container in allMedsGameObjects)
		{
			container.SetActive(false);
		}
		visibleMeds.Clear ();
	}
}
