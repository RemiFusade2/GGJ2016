using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;



public class PrescriptionBehaviour : MonoBehaviour {
	
	public int minNumberOfLines;
	public int maxNumberOfLines;

	public int minMedsCount;
	public int maxMedsCount;

	public List<string> prescriptionMedsSingular;
	public List<string> prescriptionMedsPlural;
	public List<string> prescriptionSuffixes;

	public List<string> medications;

	public List<Font> fonts;

	public int maxCharacterCountOnALine;

	public TextMesh prescriptionText;


	// Use this for initialization
	void Start () 
	{
		int numberOfLines = Random.Range (minNumberOfLines, maxNumberOfLines);
		string result = "";

		List<int> usedMeds = new List<int> ();
		List<int> usedSuffixes = new List<int> ();

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
						
			int suffixIndex = Random.Range(0,prescriptionSuffixes.Count);
			while (usedSuffixes.Contains(suffixIndex))
			{
				suffixIndex = Random.Range(0,prescriptionSuffixes.Count);
			}
			usedSuffixes.Add(suffixIndex);

			newLine += prescriptionSuffixes[suffixIndex] + ".";

			string[] stringSplit = newLine.Split(' ');
			newLine = "";
			int characterCount = 0;
			foreach (string str in stringSplit)
			{
				characterCount += str.Length + 1;
				if (characterCount > maxCharacterCountOnALine)
				{
					newLine += "\n" + str + " ";
					characterCount = str.Length + 1;
				}
				else
				{
					newLine += str + " ";
				}
			}

			result += newLine + "\n";
		}
		/*
		int fontIndex = Random.Range (0, fonts.Count);
		prescriptionText.font = fonts [fontIndex];
		*/
		prescriptionText.text = result;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void CreatePrescription()
	{

	}
}
