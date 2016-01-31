using UnityEngine;
using System.Collections;

public class SneezeBehaviour : MonoBehaviour {

	public void PlaySneezeSound()
	{
		this.GetComponent<AudioSource> ().Play ();
	}
}
