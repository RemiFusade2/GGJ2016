using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using AssemblyCSharp;

[System.Serializable]
public class PrescriptionSuffix
{
	public string message;

	public int frequencyByDay;
	public bool needMorning;
	public bool needNoon;
	public bool needEvening;
}

public class PrescriptionBehaviour : MonoBehaviour {
	
	public int minNumberOfLines;
	public int maxNumberOfLines;

	public int minMedsCount;
	public int maxMedsCount;
	
	public List<string> prescriptionMedsName;
	public List<string> prescriptionMedsSingular;
	public List<string> prescriptionMedsPlural;
	public List<PrescriptionSuffix> prescriptionSuffixs;

	public List<Font> fonts;
	public List<Material> fontMaterials;
	public List<int> characterSizeByFont;
	public List<int> maxCharacterCountOnALineByFont;

	public TextMesh prescriptionText;

	public GameEngine gameEngine;


	// Use this for initialization
	void Start () 
	{
		//CreatePrescription ();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void CreatePrescription()
	{
		PrescriptionData resultPrescription = new PrescriptionData ();

		int numberOfLines = Random.Range (minNumberOfLines, maxNumberOfLines);
		string result = "";
		
		List<int> usedMeds = new List<int> ();
		List<int> usedSuffixes = new List<int> ();

		int fontIndex = Random.Range (0, fonts.Count);
		prescriptionText.font = fonts [fontIndex];
		prescriptionText.fontSize = characterSizeByFont [fontIndex];
		prescriptionText.GetComponent<Renderer> ().material = fontMaterials [fontIndex];

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

			resultPrescription.AddMedicationData(prescriptionMedsName[medIndex], medsCount, suffix.frequencyByDay, suffix.needMorning, suffix.needNoon, suffix.needEvening);
			
			result += newLine + "\n";
		}

		prescriptionText.text = result;

		gameEngine.SetPrescription (resultPrescription);
	}

	public void ShowPrescription(bool visible)
	{
		this.GetComponent<Animator> ().SetBool ("Visible", visible);
	}
}
