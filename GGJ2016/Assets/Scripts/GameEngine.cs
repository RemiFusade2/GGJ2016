using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class MedAndQuantity
{
	[XmlAttribute("name")]
	public string medName;
	[XmlAttribute("dosage")]
	public int minimumDosageToApplyEffect;

	public string ToString()
	{
		return minimumDosageToApplyEffect + medName;
	}
}

[System.Serializable]
public class CombinationOfMeds
{
	[XmlArray("Medications")]
	[XmlArrayItem("Medication")]
	public List<MedAndQuantity> responsibleMedication;
	[XmlIgnore]
	public bool knownCombination;

	public string ToString()
	{
		string result = "";
		foreach (MedAndQuantity med in responsibleMedication)
		{
			result += med.ToString();
		}
		return result;
	}
}

[System.Serializable]
public class SideEffect
{
	[XmlAttribute("keyCode")]
	public string effectKeyCode;
	[XmlAttribute("fullName")]
	public string effectName;
	[XmlAttribute("inflict")]
	public bool inflict;
	[XmlAttribute("heal")]
	public bool heal;
	[XmlArray("MedCombinations")]
	[XmlArrayItem("MedCombination")]
	public List<CombinationOfMeds> responsibleMedCombinations;
	
	public string GetCombinationCode(int index)
	{
		string result = "";
		if (index >= 0 || index < responsibleMedCombinations.Count)
		{
			result = (heal ? "heal" : "inflict") + effectKeyCode + responsibleMedCombinations[index].ToString();
		}
		return result;
	}
}

[XmlRoot("SideEffectsList")]
[System.Serializable]
public class SideEffectsList
{
	[XmlArray("SideEffects")]
	[XmlArrayItem("SideEffect")]
	public List<SideEffect> sideEffects;
}

[System.Serializable]
public class KnownSideEffectsCombinations
{
	public List<string> knownCombinationCodes;

	public KnownSideEffectsCombinations()
	{
		knownCombinationCodes = new List<string> ();
	}
}

public class GameEngine : MonoBehaviour 
{
	public CursorBehaviour cursor;

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
	public GameObject combosPanel;

	public PrescriptionBehaviour prescriptionScript;


	public PrescriptionData currentPrescription;

	public SideEffectsList sideEffectsList;

	public ComboListBuilder comboList;

	public void SaveSideEffects(string path)
	{
		var serializer = new XmlSerializer(typeof(SideEffectsList));
		using(FileStream stream = new FileStream(path, FileMode.Create))
		{
			serializer.Serialize(stream, this.sideEffectsList);
		}
	}

	
	public void LoadSideEffects(string path)
	{
		var serializer = new XmlSerializer(typeof(SideEffectsList));
		using(FileStream stream = new FileStream(path, FileMode.Open))
		{
			sideEffectsList = serializer.Deserialize(stream) as SideEffectsList;
		}
	}

	public void SavePlayerInfo()
	{
		string path = Application.persistentDataPath + "/playerInfo.dat";	
		Debug.Log ("SAVE FILE AT: " + path);	
		XmlSerializer serializer = new XmlSerializer(typeof(KnownSideEffectsCombinations));
		
		KnownSideEffectsCombinations knowCombinations = new KnownSideEffectsCombinations ();
		using(FileStream stream = new FileStream(path, FileMode.Create))
		{
			foreach (SideEffect effect in sideEffectsList.sideEffects)
			{
				for (int index = 0 ; index < effect.responsibleMedCombinations.Count ; index++)
				{
					string code = effect.GetCombinationCode(index);
					if (effect.responsibleMedCombinations[index].knownCombination && code != null && !code.Equals(""))
					{
						knowCombinations.knownCombinationCodes.Add (effect.GetCombinationCode(index));
					}
				}
			}
			serializer.Serialize(stream, knowCombinations);
		}
	}

	public void LoadPlayerInfo()
	{
		string path = Application.persistentDataPath + "/playerInfo.dat";
		Debug.Log ("LOAD FILE AT: " + path);
		XmlSerializer serializer = new XmlSerializer(typeof(KnownSideEffectsCombinations));
		using(FileStream stream = new FileStream(path, FileMode.Open))
		{
			KnownSideEffectsCombinations knowCombinations = serializer.Deserialize(stream) as KnownSideEffectsCombinations;
			foreach (SideEffect effect in sideEffectsList.sideEffects)
			{
				for (int index = 0 ; index < effect.responsibleMedCombinations.Count ; index++)
				{
					string code = effect.GetCombinationCode(index);
					if (knowCombinations.knownCombinationCodes.Contains(code))
					{
						effect.responsibleMedCombinations[index].knownCombination = true;
					}
				}
			}
		}
	}

	public Slider healthBar;

	public int maxPlayerHP;
	private int currentPlayerHP;

	public bool gameLaunched;

	public DoctorSFXManagerBehaviour doctorSFXManager;

	// UI
	public void ShowTitle(bool show)
	{
		titlePanel.SetActive (show);
	}

	public void StartGame()
	{
		blackFadeScreen.SetActive(true);
		blackFadeAnimator.SetBool("Visible", true);
		blackFadeAnimator.SetBool("Instant", true);
		StartCoroutine (WaitAndStartNextDay (4.0f));
		StartCoroutine (WaitAndPlayIntroSound (1.0f));
		backgroundMusicSource.clip = healthyMusic;
		backgroundMusicSource.Play();
	}

	public void ShowCredits(bool show)
	{
		titlePanel.SetActive (!show);
		creditsPanel.SetActive (show);
	}
	
	public void ShowCombos(bool show)
	{
		titlePanel.SetActive (!show);
		combosPanel.SetActive (show);
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

		string sideEffectsPath = Application.dataPath + "/StreamingAssets/" + "SideEffects.xml";
		LoadSideEffects (sideEffectsPath);

		// load player info
		try
		{
			LoadPlayerInfo ();
		}
		catch (FileNotFoundException ex)
		{

		}

		// fill combo list
		comboList.FillContainers ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (!titlePanel.activeSelf)
			{
				// fill combo list
				comboList.FillContainers ();
				
				daysCount = 1;
				currentPlayerHP = maxPlayerHP;
				healthBar.value = currentPlayerHP;
				dayText.text = "DAY 1";
				doctorMessage.text = "\"Hello, I'm Dr. Frank’n Nutter.\nI'm here to take care of you.\nYou are not sick YET, but in prevention, take that prescription.\"";
				cameraScript.RemoveEffects ();
				cursor.shaking = false;

				blackFadeAnimator.gameObject.SetActive(true);
				blackFadeAnimator.SetBool ("Visible", true);
				blackFadeAnimator.SetBool ("Instant", true);

				foreach(Transform pill in outsidePills)
				{
					Destroy(pill.gameObject);
				}

				ShowTitle(true);
			}
			else
			{
				ExitGame();
			}
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
		// save player info
		SavePlayerInfo ();
		comboList.FillContainers ();

		gameLaunched = false;
		prescriptionScript.ShowPrescription (false);

		if (currentPlayerHP > 1)
		{
			daysCount++;
			dayText.text = "DAY " + daysCount;

			float healthRatio = (currentPlayerHP * 1.0f / maxPlayerHP);

			if ( healthRatio > 0.75f)
			{
				doctorMessage.text = healthyMessages[Random.Range(0,healthyMessages.Count)];
				backgroundMusicSource.clip = healthyMusic;
				backgroundMusicSource.Play();
			}
			else if ( healthRatio > 0.4f)
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
			
			StartCoroutine (WaitAndPlayDoctorSound (1.0f, healthRatio));

			StartCoroutine (WaitAndStartNextDay (3.0f));
		}
		else
		{
			dayText.text = "GAME OVER";
			doctorMessage.text = gameOverMessages[Random.Range(0,gameOverMessages.Count)];
			
			StartCoroutine (WaitAndPlayGameOverSound (1.0f));
			StartCoroutine(WaitAndRestart(5.0f));
		}
		blackFadeScreen.SetActive (true);
		blackFadeAnimator.SetBool ("Instant", false);
		blackFadeAnimator.SetBool ("Visible", true);
	}
	
	IEnumerator WaitAndPlayDoctorSound(float timer, float health)
	{
		yield return new WaitForSeconds (timer);
		doctorSFXManager.PlaySound (health);
	}
	
	IEnumerator WaitAndPlayGameOverSound(float timer)
	{
		yield return new WaitForSeconds (timer);
		doctorSFXManager.PlayGameOver ();
	}
	
	IEnumerator WaitAndPlayIntroSound(float timer)
	{
		yield return new WaitForSeconds (timer);
		doctorSFXManager.PlayIntro ();
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
		cursor.shaking = false;
	}

	IEnumerator WaitAndStartNextDay(float timer)
	{
		yield return new WaitForSeconds (timer);
		blackFadeAnimator.SetBool ("Instant", false);
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

	public Transform morningTriggerAreaParent;
	public Transform noonTriggerAreaParent;
	public Transform eveningTriggerAreaParent;
	private void CheckPillsPosition()
	{
		CheckPillsPositionForPeriodOfTime (morningTriggerAreaParent, morningPills);
		CheckPillsPositionForPeriodOfTime (noonTriggerAreaParent, noonPills);
		CheckPillsPositionForPeriodOfTime (eveningTriggerAreaParent, eveningPills);
	}
	private void CheckPillsPositionForPeriodOfTime(Transform triggerAreaParent, Transform containerObject)
	{
		foreach (Transform triggerArea in triggerAreaParent)
		{
			Vector2 origin = new Vector2(triggerArea.position.x, triggerArea.position.y);
			int maxTries = 300;
			float rayLength = 4.0f;
			for (int i = 0 ; i < maxTries ; i++)
			{
				Vector2 direction = rayLength * new Vector2(Mathf.Sin ((i*1.0f/maxTries)*2*Mathf.PI),Mathf.Cos((i*1.0f/maxTries)*2*Mathf.PI));
				//Debug.DrawRay(new Vector3(origin.x, origin.y, 0), new Vector3(direction.x, direction.y, 0), Color.red, 1.0f);
				RaycastHit2D hit = Physics2D.Raycast(origin, direction);
				if (hit != null && hit.collider.tag.Equals("Pill"))
				{
					hit.collider.transform.parent = containerObject;
					hit.collider.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
				}
			}
		}
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
		foreach (SideEffect sideEffect in this.sideEffectsList.sideEffects)
		{
			// check for combinations
			bool oneCombinationIsMet = false;
			foreach (CombinationOfMeds combination in sideEffect.responsibleMedCombinations)
			{
				bool combinationIsMet = true;
				foreach (MedAndQuantity med in combination.responsibleMedication)
				{
					//Debug.Log ("looking for " + combination.minimumDosageToApplyEffect + " " + med);
					combinationIsMet &= (allPillsDico.ContainsKey(med.medName) && allPillsDico[med.medName] >= med.minimumDosageToApplyEffect);
					//Debug.Log ("there was " + (allPillsDico.ContainsKey(med) ? allPillsDico[med] : 0));
				}

				if (combinationIsMet)
				{
					combination.knownCombination = true;
				}

				oneCombinationIsMet |= combinationIsMet;
			}

			if (oneCombinationIsMet)
			{
				if (sideEffect.inflict && !sideEffectsToInflict.Contains(sideEffect.effectKeyCode))
				{
					sideEffectsToInflict.Add(sideEffect.effectKeyCode);
				}
				if (sideEffect.heal && !sideEffectsToHeal.Contains(sideEffect.effectKeyCode))
				{
					sideEffectsToHeal.Add(sideEffect.effectKeyCode);
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

		// Sepia
		if (sideEffectsToHeal.Contains("Sepia"))
		{
			cameraScript.SetSepia(false);
		}
		if (sideEffectsToInflict.Contains("Sepia"))
		{
			cameraScript.SetSepia(true);
		}
		
		// Cursor Shake
		if (sideEffectsToHeal.Contains("CursorShaking"))
		{
			cursor.shaking = false;
		}
		if (sideEffectsToInflict.Contains("CursorShaking"))
		{
			cursor.shaking = true;
		}
		
		// Sneeze
		if (sideEffectsToHeal.Contains("Sneeze"))
		{
			cameraScript.SetSneeze(false);
		}
		if (sideEffectsToInflict.Contains("Sneeze"))
		{
			cameraScript.SetSneeze(true);
		}
		
		// MegaContrasts
		if (sideEffectsToHeal.Contains("MegaContrasts"))
		{
			cameraScript.SetSatanContrasts(false);
		}
		if (sideEffectsToInflict.Contains("MegaContrasts"))
		{
			cameraScript.SetSatanContrasts(true);
		}
	}
	
	public void AbsorbPills()
	{
		CheckPillsPosition ();
		CountPills ();
		EndDay ();
	}
}
