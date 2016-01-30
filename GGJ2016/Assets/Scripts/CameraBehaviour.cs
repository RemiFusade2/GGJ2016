using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class CameraBehaviour : MonoBehaviour {

	// Animations
	public Animator shakeAnimator;
	public Animator hiccupAnimator;
	public Animator tiltAnimator;
	public Animator zoomAnimator;

	public bool shake;
	public bool hiccup;
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
		this.GetComponent<ContrastStretch> ().enabled = active;
	}
}
