using UnityEngine;
using System.Collections;

public class GameEngine : MonoBehaviour {

	public CameraBehaviour cameraScript;

	public GameObject allPills;

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
					cameraScript.ApplyShake();
				}
				if (pillBehaviour.noiseEffect)
				{
					cameraScript.ApplyNoise();
				}
			}
			Destroy(child.gameObject);
		}
	}
}
