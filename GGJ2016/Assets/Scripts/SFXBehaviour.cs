using UnityEngine;
using System.Collections;

public class SFXBehaviour : MonoBehaviour 
{
	public AudioClip UINavigation;
	public AudioClip UISelection;

	public AudioClip swallowSound;

	public AudioClip takePillSound;
	public AudioClip dropPillSound;

	public void PlayNavigationSound()
	{
		this.GetComponent<AudioSource> ().clip = UINavigation;
		this.GetComponent<AudioSource> ().Play ();
	}
	
	public void PlaySelectionSound()
	{
		this.GetComponent<AudioSource> ().clip = UISelection;
		this.GetComponent<AudioSource> ().Play ();
	}
	
	public void PlaySwallowSound()
	{
		this.GetComponent<AudioSource> ().clip = swallowSound;
		this.GetComponent<AudioSource> ().Play ();
	}
	
	public void PlayTakePillSound()
	{
		this.GetComponent<AudioSource> ().clip = takePillSound;
		this.GetComponent<AudioSource> ().Play ();
	}
	
	public void PlayDropPillSound()
	{
		this.GetComponent<AudioSource> ().clip = dropPillSound;
		this.GetComponent<AudioSource> ().Play ();
	}
}
