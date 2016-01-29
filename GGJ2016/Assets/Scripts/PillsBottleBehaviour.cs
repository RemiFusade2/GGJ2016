using UnityEngine;
using System.Collections;

public class PillsBottleBehaviour : MonoBehaviour {

	public GameObject pillPrefab;

	public GameObject allPillsContainer;

	// Use this for initialization
	void Start () 
	{ 
	
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	void OnMouseDown ()
	{
		Vector3 cursorPositionInWorld = Camera.main.ScreenToWorldPoint (Input.mousePosition + Camera.main.transform.forward * 10);
		GameObject pill = (GameObject)Instantiate (pillPrefab, cursorPositionInWorld, Quaternion.identity);
		pill.transform.parent = allPillsContainer.transform;
	}
}
