using UnityEngine;
using System.Collections;

public class CursorBehaviour : MonoBehaviour {

	public bool shaking;
	public float shakingForce;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 cursorPositionInWorld = Camera.main.ScreenToWorldPoint (Input.mousePosition + Camera.main.transform.forward * 10);
		this.transform.position = cursorPositionInWorld + (shaking ? (Vector3.up * Random.Range(-shakingForce,shakingForce) + Vector3.right * Random.Range(-shakingForce,shakingForce)) : Vector3.zero);
	}
}
