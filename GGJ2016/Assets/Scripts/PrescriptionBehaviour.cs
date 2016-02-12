using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using AssemblyCSharp;
using System.Xml.Serialization;

[System.Serializable]
public class PrescriptionSuffix
{
	public string message;

	public int frequencyByDay;
	public bool needMorning;
	public bool needNoon;
	public bool needEvening;

	public bool isRequired;
}

[System.Serializable]
public class DifficultyLevel
{
	public int minNumberOfLines;
	public int maxNumberOfLines;
	
	public int minNumberOfMeds;
	public int maxNumberOfMeds;
}

[XmlRoot("PrescriptionSuffixesList")]
[System.Serializable]
public class PrescriptionSuffixesList
{
	[XmlArray("PrescriptionSuffixes")]
	[XmlArrayItem("PrescriptionSuffix")]
	public List<PrescriptionSuffix> suffixes;
}

public class PrescriptionBehaviour : MonoBehaviour 
{	
	public MedsContainersManagerBehaviour medsContainerManager;

	public List<DifficultyLevel> difficultyLevels;

	public int minMedsCount;
	public int maxMedsCount;
	
	public List<string> prescriptionMedsName;
	public List<string> prescriptionMedsSingular;
	public List<string> prescriptionMedsPlural;
	public List<PrescriptionSuffix> prescriptionSuffixs;
	public PrescriptionSuffixesBehaviour prescriptionSuffixesBackupData;

	public TextMesh doctorSignature;
	public List<string> doctorNames;
	public List<Font> fonts;
	public List<Material> fontMaterials;
	public List<int> characterSizeByFont;
	public List<int> maxCharacterCountOnALineByFont;

	public TextMesh prescriptionText;
	private List<string> prescriptionLines;

	public GameEngine gameEngine;

	public void PlayPaperSound()
	{
		this.GetComponent<AudioSource>().Play();
	}

	// Use this for initialization
	void Start () 
	{
		prescriptionLines = new List<string> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void CreatePrescription()
	{
		PrescriptionData resultPrescription = new PrescriptionData ();

		prescriptionLines.Clear ();

		int dayIndex = gameEngine.daysCount - 1;
		int difficultyIndex = (dayIndex >= difficultyLevels.Count ? (difficultyLevels.Count - 1) : dayIndex);
		DifficultyLevel difficulty = difficultyLevels [difficultyIndex];


		int numberOfMedsOnTable = Random.Range( difficulty.minNumberOfMeds, difficulty.maxNumberOfMeds );

		int numberOfLines = Random.Range (difficulty.minNumberOfLines, difficulty.maxNumberOfLines);
		string result = "";

		
		List<int> usedMeds = new List<int> ();
		List<int> usedSuffixes = new List<int> ();

		int fontIndex = Random.Range (0, fonts.Count);
		prescriptionText.font = fonts [fontIndex];
		prescriptionText.fontSize = characterSizeByFont [fontIndex];
		prescriptionText.GetComponent<Renderer> ().material = fontMaterials [fontIndex];

		doctorSignature.text = doctorNames [fontIndex];
		doctorSignature.font = fonts [fontIndex];
		doctorSignature.fontSize = characterSizeByFont [fontIndex];
		doctorSignature.GetComponent<Renderer> ().material = fontMaterials [fontIndex];

		for (int i = 0 ; i < numberOfLines ; i++)
		{
			string newLine = "";
			
			int medsCount = Random.Range (minMedsCount, maxMedsCount);
			newLine += medsCount + " ";
			
			int medIndex = Random.Range(0,prescriptionMedsSingular.Count);
			while (usedMeds.Contains(medIndex))
			{
				medIndex = Random.Range(0,prescriptionMedsSingular.Count);
			}
			usedMeds.Add(medIndex);
			
			if (medsCount <= 1)
			{
				newLine += prescriptionMedsSingular[medIndex] + " ";
			}
			else
			{
				newLine += prescriptionMedsPlural[medIndex] + " ";
			}
			
			int suffixIndex = Random.Range(0,prescriptionSuffixs.Count);
			while (usedSuffixes.Contains(suffixIndex))
			{
				suffixIndex = Random.Range(0,prescriptionSuffixs.Count);
			}
			usedSuffixes.Add(suffixIndex);

			PrescriptionSuffix suffix = prescriptionSuffixs[suffixIndex];

			newLine += suffix.message + ".";
			
			string[] stringSplit = newLine.Split(' ');
			newLine = "";
			int characterCount = 0;
			foreach (string str in stringSplit)
			{
				characterCount += str.Length + 1;
				if (characterCount > maxCharacterCountOnALineByFont[fontIndex])
				{
					newLine += "\n" + str + " ";
					characterCount = str.Length + 1;
				}
				else
				{
					newLine += str + " ";
				}
			}

			resultPrescription.AddMedicationData(prescriptionMedsName[medIndex], medsCount, suffix.frequencyByDay, suffix.needMorning, suffix.needNoon, suffix.needEvening, suffix.isRequired);

			prescriptionLines.Add(newLine);

			result += newLine + "\n";
		}

		prescriptionText.text = result;

		List<string> usedMedNames = new List<string>();
		foreach (MedicationData data in resultPrescription.listOfMedications)
		{
			usedMedNames.Add(data.medicationName);
		}
		medsContainerManager.HideAllContainers ();
		medsContainerManager.ShowContainers (usedMedNames);
		medsContainerManager.AddRandomContainers (numberOfMedsOnTable - usedMedNames.Count);

		gameEngine.SetPrescription (resultPrescription);
	}

	public void ShowErrors(List<int> remainingMedsToTake)
	{
		string result = "";
		int index = 0;
		foreach (int remainingMeds in remainingMedsToTake)
		{
			if (remainingMeds > 0)
			{
				result += "<color=red>" + prescriptionLines[index] + "</color>\n";
			}
			else
			{
				result += prescriptionLines[index] + "\n";
			}
			index++;
		}
		prescriptionText.text = result;
	}

	public void ShowPrescription(bool visible)
	{
		this.GetComponent<Animator> ().SetBool ("Visible", visible);
	}
}
