using UnityEngine;
using System.Collections;

public class PillsBottleBehaviour : MonoBehaviour {

	public GameObject pillPrefab;

	public GameObject outsidePillsContainer;

	public GameObject cursor;

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
		pill.transform.parent = outsidePillsContainer.transform;
		pill.AddComponent<HingeJoint2D> ().connectedBody = cursor.GetComponent<Rigidbody2D>();
		pill.GetComponent<PillBehaviour> ().cursor = cursor;
	}
}
