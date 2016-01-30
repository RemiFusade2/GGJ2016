using UnityEngine;
using System.Collections;

public class GameEngine : MonoBehaviour {

	public CameraBehaviour cameraScript;

	public GameObject allPills;

	public GameObject titlePanel;
	public GameObject creditsPanel;

	public void ShowTitle(bool show)
	{
		titlePanel.SetActive (show);
	}

	public void ShowCredits(bool show)
	{
		titlePanel.SetActive (!show);
		creditsPanel.SetActive (show);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void AbsorbPills()
	{
		foreach (Transform child in allPills.transform)
		{
			PillBehaviour pillBehaviour = child.GetComponent<PillBehaviour>();
			if (pillBehaviour != null)
			{
				if (pillBehaviour.shakeEffect)
				{
					cameraScript.SetShake(true);
				}
				if (pillBehaviour.hiccupEffect)
				{
					cameraScript.SetHiccup(true);
				}
				if (pillBehaviour.tiltEffect)
				{
					cameraScript.SetTilt(true);
				}
				if (pillBehaviour.zoomEffect)
				{
					cameraScript.SetZoom(true);
				}
				if (pillBehaviour.noiseEffect)
				{
					cameraScript.SetNoise(true);
				}
			}
			Destroy(child.gameObject);
		}
	}
}
