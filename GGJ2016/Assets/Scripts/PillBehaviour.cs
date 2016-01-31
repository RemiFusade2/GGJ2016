using UnityEngine;
using System.Collections;

public class PillBehaviour : MonoBehaviour
{
	public GameObject cursor;

	public string medicationName;
	
	public SFXBehaviour sfxManager;

	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetMouseButtonUp(0))
		{
			Destroy(this.GetComponent<HingeJoint2D>());
			sfxManager.PlayDropPillSound ();
		}
	}

	void OnMouseDown ()
	{
		Vector3 cursorPositionInWorld = Camera.main.ScreenToWorldPoint (Input.mousePosition + Camera.main.transform.forward * 10);
		this.gameObject.AddComponent<HingeJoint2D> ().connectedBody = cursor.GetComponent<Rigidbody2D>();
		sfxManager.PlayTakePillSound ();
	}
}
