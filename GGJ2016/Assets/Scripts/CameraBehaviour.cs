using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class CameraBehaviour : MonoBehaviour {

	// Animations
	public Animator shakeAnimator;
	public Animator hiccupAnimator;
	public Animator tiltAnimator;
	public Animator zoomAnimator;
	public Animator sneezeAnimator;

	public bool shake;
	public bool hiccup;
	public bool sneeze;
	public bool tilt;
	public bool zoom;

	// Image effects
	public bool noise;
	public bool sepia;
	public bool grayscale;
	public bool motionBlur;
	public bool pixelOverlay;
	public bool satanContrasts;


	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
	}

	public void SetShake(bool active)
	{
		shake = active;
		shakeAnimator.SetBool ("Shake", active);
	}
	
	public void SetHiccup(bool active)
	{
		hiccup = active;
		hiccupAnimator.SetBool ("Hiccup", active);
	}
	
	public void SetSneeze(bool active)
	{
		sneeze = active;
		if (sneeze)
		{
			StartCoroutine (WaitAndSneeze (12.0f));
		}
	}
	IEnumerator WaitAndSneeze(float timer)
	{
		yield return new WaitForSeconds (timer);
		sneezeAnimator.SetTrigger ("Sneeze");
		if (sneeze)
		{
			StartCoroutine (WaitAndSneeze (Random.Range (5.0f, 10.0f)));
		}
	}
	
	public void SetTilt(bool active)
	{
		tilt = active;
		tiltAnimator.SetBool ("Tilt", active);
	}
	
	public void SetZoom(bool active)
	{
		zoom = active;
		zoomAnimator.SetBool ("Zoom", active);
	}

	public void SetNoise(bool active)
	{
		noise = active;
		this.GetComponent<NoiseAndScratches> ().enabled = active;
	}
	
	public void SetSepia(bool active)
	{
		sepia = active;
		this.GetComponent<SepiaTone> ().enabled = active;
	}
	
	public void SetGrayscale(bool active)
	{
		grayscale = active;
		this.GetComponent<Grayscale> ().enabled = active;
	}
	
	public void SetMotionBlur(bool active)
	{
		motionBlur = active;
		this.GetComponent<MotionBlur> ().enabled = active;
	}
	
	public void SetPixelOverlay(bool active)
	{
		pixelOverlay = active;
		this.GetComponent<ScreenOverlay> ().enabled = active;
	}
	
	public void SetSatanContrasts(bool active)
	{
		satanContrasts = active;
		if (this.GetComponent<ContrastEnhance> () != null)
		{
			this.GetComponent<ContrastEnhance> ().enabled = active;
		}
	}

	public void RemoveEffects()
	{
		SetGrayscale (false);
		SetHiccup (false);
		SetMotionBlur (false);
		SetNoise (false);
		SetPixelOverlay (false);
		SetSatanContrasts (false);
		SetSepia (false);
		SetShake (false);
		SetSneeze (false);
		SetTilt (false);
		SetZoom (false);
	}
}
