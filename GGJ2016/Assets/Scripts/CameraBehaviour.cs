using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class CameraBehaviour : MonoBehaviour {

	public bool shake;
	private Vector3 shakeVec;
	private Vector3 originalPosition ;

	public bool noise;

	// Use this for initialization
	void Start () 
	{
		originalPosition = this.transform.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (shake)
		{
			shakeVec = new Vector3(Random.Range(-0.1f,0.1f), Random.Range(-0.1f,0.1f), 0);
			this.transform.position = originalPosition + shakeVec;
		}
		if (noise)
		{

		}
	}

	public void ApplyShake()
	{
		shake = true;
	}

	public void ApplyNoise()
	{
		noise = true;
		this.GetComponent<NoiseAndScratches> ().enabled = true;
	}
}
