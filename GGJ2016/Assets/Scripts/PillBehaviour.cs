using UnityEngine;
using System.Collections;

public class PillBehaviour : MonoBehaviour {

	public bool grabed;

	public bool shakeEffect;
	public bool hiccupEffect;
	public bool tiltEffect;
	public bool zoomEffect;
	public bool noiseEffect;

	// Use this for initialization
	void Start () 
	{
		grabed = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (grabed && Input.GetMouseButton(0))
		{
			Vector3 cursorPositionInWorld = Camera.main.ScreenToWorldPoint (Input.mousePosition + Camera.main.transform.forward * 10);
			this.transform.position = cursorPositionInWorld;
		}
		if (Input.GetMouseButtonUp(0))
		{
			grabed = false;
		}
	}
}
