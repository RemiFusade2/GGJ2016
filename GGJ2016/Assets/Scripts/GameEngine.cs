using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class CombinationOfMeds
{
	public List<string> responsibleMeds;
	public int minimumDosageToApplyEffect;
}

[System.Serializable]
public class SideEffect
{
	public string effectName;
	public bool inflict;
	public bool heal;
	public List<CombinationOfMeds> responsibleMedCombinations;
}

public class GameEngine : MonoBehaviour {

	public CameraBehaviour cameraScript;

	public AudioSource backgroundMusicSource;
	public AudioClip menuMusic;
	public AudioClip healthyMusic;
	public AudioClip mediumMusic;
	public AudioClip weakMusic;

	public Text dayText;
	public Text doctorMessage;

	public List<string> healthyMessages;
	public List<string> mediumHealthMessages;
	public List<string> weakMessages;
	public List<string> gameOverMessages;

	public Animator blackFadeAnimator;
	public GameObject blackFadeScreen;

	public int daysCount;
	
	public Transform morningPills;
	public Transform noonPills;
	public Transform eveningPills;
	public Transform outsidePills;

	public GameObject titlePanel;
	public GameObject creditsPanel;

	public PrescriptionBehaviour prescriptionScript;


	public PrescriptionData currentPrescription;

	public List<SideEffect> sideEffects;

	public Slider healthBar;

	public int maxPlayerHP;
	private int currentPlayerHP;

	public bool gameLaunched;

	// UI
	public void ShowTitle(bool show)
	{
		titlePanel.SetActive (show);
		if (!show)
		{
			StartCoroutine (WaitAndStartNextDay (4.0f));
			backgroundMusicSource.clip = healthyMusic;
			backgroundMusicSource.Play();
		}
	}

	public void ShowCredits(bool show)
	{
		titlePanel.SetActive (!show);
		creditsPanel.SetActive (show);
	}
	
	public void ExitGame()
	{
		Application.Quit ();
	}

	// Use this for initialization
	void Start ()
	{
		daysCount = 1;
		currentPlayerHP = maxPlayerHP;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			ExitGame();
		}
	}

	public void StartDay()
	{
		prescriptionScript.CreatePrescription ();
		prescriptionScript.ShowPrescription (true);
		gameLaunched = true;
	}

	public void EndDay()
	{
		gameLaunched = false;
		prescriptionScript.ShowPrescription (false);

		if (currentPlayerHP > 0)
		{
			daysCount++;
			dayText.text = "DAY " + daysCount;
			
			if ( (currentPlayerHP * 1.0f / maxPlayerHP) > 0.75f)
			{
				doctorMessage.text = healthyMessages[Random.Range(0,healthyMessages.Count)];
				backgroundMusicSource.clip = healthyMusic;
				backgroundMusicSource.Play();
			}
			else if ( (currentPlayerHP * 1.0f / maxPlayerHP) > 0.4f)
			{
				doctorMessage.text = mediumHealthMessages[Random.Range(0,mediumHealthMessages.Count)];
				backgroundMusicSource.clip = mediumMusic;
				backgroundMusicSource.Play();
			}
			else
			{
				doctorMessage.text = weakMessages[Random.Range(0,weakMessages.Count)];
				backgroundMusicSource.clip = weakMusic;
				backgroundMusicSource.Play();
			}
			StartCoroutine (WaitAndStartNextDay (3.0f));
		}
		else
		{
			dayText.text = "GAME OVER";
			doctorMessage.text = gameOverMessages[Random.Range(0,gameOverMessages.Count)];

			StartCoroutine(WaitAndRestart(5.0f));
		}
		blackFadeScreen.SetActive (true);
		blackFadeAnimator.SetBool ("Visible", true);
	}
	
	IEnumerator WaitAndRestart(float timer)
	{
		yield return new WaitForSeconds (timer);
		
		daysCount = 1;
		currentPlayerHP = maxPlayerHP;
		healthBar.value = currentPlayerHP;
		dayText.text = "DAY 1";
		doctorMessage.text = "\"Hello, I'm Dr. Frank’n Nutter.\nI'm here to take care of you.\nYou are not sick YET, but in prevention, take that prescription.\"";
		ShowTitle (true);
		cameraScript.RemoveEffects ();
	}

	IEnumerator WaitAndStartNextDay(float timer)
	{
		yield return new WaitForSeconds (timer);
		blackFadeAnimator.SetBool ("Visible", false);
		StartCoroutine (WaitAndHideBlackFadeScreen (2.0f));
		StartDay ();
	}

	IEnumerator WaitAndHideBlackFadeScreen(float timer)
	{
		yield return new WaitForSeconds (timer);
		blackFadeScreen.SetActive (false);
	}

	public void SetPrescription(PrescriptionData prescription)
	{
		currentPrescription = prescription;
	}
	
	public Dictionary<string,int> CreatePillsDico(Transform pillsContainer)
	{
		List<Transform> pillsContainers = new List<Transform> ();
		pillsContainers.Add (pillsContainer);
		return CreatePillsDico (pillsContainers);
	}

	public Dictionary<string,int> CreatePillsDico(List<Transform> pillsContainers)
	{
		Dictionary<string,int> resultDico = new Dictionary<string,int>();
		foreach (Transform container in pillsContainers)
		{
			foreach (Transform child in container) 
			{
				PillBehaviour pillBehaviour = child.GetComponent<PillBehaviour> ();
				if (resultDico.ContainsKey(pillBehaviour.medicationName))
				{
					resultDico[pillBehaviour.medicationName]++;
				}
				else
				{
					resultDico.Add(pillBehaviour.medicationName, 1);
				}
			}
		}
		return resultDico;
	}

	public void CountPills()
	{
		// count
		Dictionary<string,int> morningPillsDico = CreatePillsDico(morningPills);
		Dictionary<string,int> noonPillsDico = CreatePillsDico(noonPills);
		Dictionary<string,int> eveningPillsDico = CreatePillsDico(eveningPills);
		
		List<Transform> allPillsContainers = new List<Transform> ();
		allPillsContainers.Add (morningPills);
		allPillsContainers.Add (noonPills);
		allPillsContainers.Add (eveningPills);
		Dictionary<string,int> allPillsDico = CreatePillsDico(allPillsContainers);

		List<int> remainingMedsToTakeFromPrescription = new List<int> ();

		// compare with prescription and remove pills
		int howManyPillsLeftOnPrescription = 0;
		foreach (MedicationData medData in currentPrescription.listOfMedications)
		{
			string medicationName = medData.medicationName;
			int medicationCount = medData.numberOfPills * medData.frequencyByDay;
						
			//Debug.Log ("You need " + medicationCount + " pills of " + medicationName);

			if (medData.periodOfTimeIsImportant)
			{
				int medicationCount_Morning = medData.needMorning ? medicationCount : 0;
				int medicationCount_Noon = medData.needNoon ? medicationCount : 0;
				int medicationCount_Evening = medData.needEvening ? medicationCount : 0;
				
				//Debug.Log ("You need " + medicationCount_Morning + " pills of " + medicationName + " on morning");
				//Debug.Log ("You need " + medicationCount_Noon + " pills of " + medicationName + " at noon");
				//Debug.Log ("You need " + medicationCount_Evening + " pills of " + medicationName + " on evening");

				if (medData.needMorning)
				{
					while (morningPillsDico.ContainsKey(medicationName) && morningPillsDico[medicationName] >= 1 && medicationCount_Morning >= 1)
					{
						morningPillsDico[medicationName] = morningPillsDico[medicationName] - 1;
						allPillsDico[medicationName] = allPillsDico[medicationName] - 1;
						medicationCount_Morning--;
					}
					if (morningPillsDico.ContainsKey(medicationName) && morningPillsDico[medicationName] <= 0)
					{
						morningPillsDico.Remove(medicationName);
					}
				}
				if (medData.needNoon)
				{
					while (noonPillsDico.ContainsKey(medicationName) && noonPillsDico[medicationName] >= 1 && medicationCount_Noon >= 1)
					{
						noonPillsDico[medicationName] = noonPillsDico[medicationName] - 1;
						allPillsDico[medicationName] = allPillsDico[medicationName] - 1;
						medicationCount_Noon--;
					}
					if (noonPillsDico.ContainsKey(medicationName) && noonPillsDico[medicationName] <= 0)
					{
						noonPillsDico.Remove(medicationName);
					}
				}
				if (medData.needEvening)
				{
					while (eveningPillsDico.ContainsKey(medicationName) && eveningPillsDico[medicationName] >= 1 && medicationCount_Evening >= 1)
					{
						eveningPillsDico[medicationName] = eveningPillsDico[medicationName] - 1;
						allPillsDico[medicationName] = allPillsDico[medicationName] - 1;
						medicationCount_Evening--;
					}
					if (eveningPillsDico.ContainsKey(medicationName) && eveningPillsDico[medicationName] <= 0)
					{
						eveningPillsDico.Remove(medicationName);
					}
				}
				if (allPillsDico.ContainsKey(medicationName) && allPillsDico[medicationName] <= 0)
				{
					allPillsDico.Remove(medicationName);
				}
				medicationCount = medicationCount_Morning + medicationCount_Noon + medicationCount_Evening;
				//Debug.Log("medicationCount_Morning = " + medicationCount_Morning);
				//Debug.Log("medicationCount_Noon = " + medicationCount_Noon);
				//Debug.Log("medicationCount_Evening = " + medicationCount_Evening);
				howManyPillsLeftOnPrescription += medicationCount;
			}
			else
			{
				while (allPillsDico.ContainsKey(medicationName) && allPillsDico[medicationName] >= 1 && medicationCount >= 1)
				{
					allPillsDico[medicationName] = allPillsDico[medicationName] - 1;
					medicationCount--;
					//Debug.Log ("You take one " + medicationName);
					//Debug.Log (allPillsDico[medicationName] + " " + medicationName + " left");
				}
				if (allPillsDico.ContainsKey(medicationName) && allPillsDico[medicationName] <= 0)
				{
					allPillsDico.Remove(medicationName);
				}
				//Debug.Log("medicationCount = " + medicationCount);
				howManyPillsLeftOnPrescription += medicationCount;
			}
			remainingMedsToTakeFromPrescription.Add(medicationCount);
			
			//Debug.Log ("You need " + howManyPillsLeftOnPrescription + " total pills");
		}
		//Debug.Log ("You missed " + howManyPillsLeftOnPrescription + " pills on your prescription");

		// too much pills ?
		int numberOfUnwantedPills = 0;
		foreach (int val in allPillsDico.Values)
		{
			numberOfUnwantedPills += val;
		}
		//Debug.Log ("numberOfUnwantedPills: " + numberOfUnwantedPills);
		//Debug.Log ("You took " + numberOfUnwantedPills + " pills you didn't need");

		// HP loss
		prescriptionScript.ShowErrors (remainingMedsToTakeFromPrescription);
		int HPLoss = howManyPillsLeftOnPrescription + numberOfUnwantedPills;
		currentPlayerHP -= HPLoss;
		healthBar.value = currentPlayerHP;
		//Debug.Log ("HPLoss: " + HPLoss);

		// side effects
		allPillsDico = CreatePillsDico(allPillsContainers);
		List<string> sideEffectsToInflict = new List<string> ();
		List<string> sideEffectsToHeal = new List<string> ();
		foreach (SideEffect sideEffect in sideEffects)
		{
			// check for combinations
			bool oneCombinationIsMet = false;
			foreach (CombinationOfMeds combination in sideEffect.responsibleMedCombinations)
			{
				bool combinationIsMet = true;
				foreach (string med in combination.responsibleMeds)
				{
					//Debug.Log ("looking for " + combination.minimumDosageToApplyEffect + " " + med);
					combinationIsMet &= (allPillsDico.ContainsKey(med) && allPillsDico[med] >= combination.minimumDosageToApplyEffect);
					//Debug.Log ("there was " + (allPillsDico.ContainsKey(med) ? allPillsDico[med] : 0));
				}
				oneCombinationIsMet |= combinationIsMet;
				if (oneCombinationIsMet)
				{
					break;
				}
			}

			if (oneCombinationIsMet)
			{
				if (sideEffect.inflict && !sideEffectsToInflict.Contains(sideEffect.effectName))
				{
					sideEffectsToInflict.Add(sideEffect.effectName);
				}
				if (sideEffect.heal && !sideEffectsToHeal.Contains(sideEffect.effectName))
				{
					sideEffectsToHeal.Add(sideEffect.effectName);
				}
			}
		}

		// apply effects
		ApplyEffects (sideEffectsToInflict, sideEffectsToHeal);

		// remove all pills
		foreach (Transform pill in morningPills)
		{
			Destroy(pill.gameObject);
		}
		foreach (Transform pill in noonPills)
		{
			Destroy(pill.gameObject);
		}
		foreach (Transform pill in eveningPills)
		{
			Destroy(pill.gameObject);
		}
		foreach (Transform pill in outsidePills)
		{
			Destroy(pill.gameObject);
		}
	}

	public void ApplyEffects (List<string> sideEffectsToInflict, List<string> sideEffectsToHeal)
	{
		int maxNumberOfSideEffects = 2;

		// Motion Blur
		if (sideEffectsToHeal.Contains("MotionBlur"))
		{
			cameraScript.SetMotionBlur(false);
		}
		if (sideEffectsToInflict.Contains("MotionBlur"))
		{
			cameraScript.SetMotionBlur(true);
		}
		
		// Camera Shake
		if (sideEffectsToHeal.Contains("CameraShake"))
		{
			cameraScript.SetShake(false);
		}
		if (sideEffectsToInflict.Contains("CameraShake"))
		{
			cameraScript.SetShake(true);
		}
		
		// Rocking Boat
		if (sideEffectsToHeal.Contains("RockingBoat"))
		{
			cameraScript.SetTilt(false);
		}
		if (sideEffectsToInflict.Contains("RockingBoat"))
		{
			cameraScript.SetTilt(true);
		}
		
		// Monochrome Filter
		if (sideEffectsToHeal.Contains("Monochrome"))
		{
			cameraScript.SetGrayscale(false);
		}
		if (sideEffectsToInflict.Contains("Monochrome"))
		{
			cameraScript.SetGrayscale(true);
		}
		
		// Noise
		if (sideEffectsToHeal.Contains("Noise"))
		{
			cameraScript.SetNoise(false);
		}
		if (sideEffectsToInflict.Contains("Noise"))
		{
			cameraScript.SetNoise(true);
		}
		
		// Overlay
		if (sideEffectsToHeal.Contains("Overlay"))
		{
			cameraScript.SetPixelOverlay(false);
		}
		if (sideEffectsToInflict.Contains("Overlay"))
		{
			cameraScript.SetPixelOverlay(true);
		}
	}
	
	public void AbsorbPills()
	{
		CountPills ();
		EndDay ();
	}
}
