using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DoctorSFXManagerBehaviour : MonoBehaviour {

	public List<AudioClip> healthySounds;
	public List<AudioClip> mediumSounds;
	public List<AudioClip> weakSounds;

	public AudioClip intro;

	public AudioClip gameOver;

	public void PlaySound(float health)
	{
		if (health > 0.7f)
		{
			this.GetComponent<AudioSource> ().clip = healthySounds[Random.Range(0,healthySounds.Count)];
		}
		else if (health > 0.4f)
		{
			this.GetComponent<AudioSource> ().clip = mediumSounds[Random.Range(0,mediumSounds.Count)];
		}
		else
		{
			this.GetComponent<AudioSource> ().clip = weakSounds[Random.Range(0,weakSounds.Count)];
		}
		this.GetComponent<AudioSource> ().Play ();
	}

	public void PlayIntro()
	{
		this.GetComponent<AudioSource> ().clip = intro;
		this.GetComponent<AudioSource> ().Play ();
	}
	
	public void PlayGameOver()
	{
		this.GetComponent<AudioSource> ().clip = gameOver;
		this.GetComponent<AudioSource> ().Play ();
	}
}
