using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ComboListBuilder : MonoBehaviour {

	public GameEngine gameEngine; // contains combo list

	public GameObject effectNameCellPrefab;
	public List<GameObject> medCombinationCellPrefabsList;

	public RectTransform help1Text;
	public GameObject inflictedEffectsContainer;
	public RectTransform help2Text;
	public GameObject healedEffectsContainer;

	private float inflictedEffectsYOffset;
	private float healedEffectsYOffset;

	public bool hideUnknownCombos;

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.U))
		{
			hideUnknownCombos = false;
			FillContainers();
		}
	}

	private void EmptyContainer(GameObject container)
	{
		foreach (Transform child in container.transform)
		{
			Destroy (child.gameObject);
		}
		inflictedEffectsYOffset = 0;
		healedEffectsYOffset = 0;
	}
	
	public void FillContainers()
	{
		// Clear
		EmptyContainer (inflictedEffectsContainer);
		EmptyContainer (healedEffectsContainer);

		// Fill
		foreach (SideEffect sideEffect in gameEngine.sideEffectsList.sideEffects) 
		{
			string name = sideEffect.effectName;
			bool heal = sideEffect.heal;

			// create cell
			Vector3 position =  Vector3.zero;
			GameObject effectNameCell = (GameObject) Instantiate (effectNameCellPrefab, position, Quaternion.identity);
			effectNameCell.GetComponent<UIEffectNamePanelBehaviour>().SetTitle(name, heal);

			// set parent
			effectNameCell.transform.SetParent(heal ? healedEffectsContainer.transform : inflictedEffectsContainer.transform);

			// set size and position
			RectTransform rectTransform = effectNameCell.GetComponent<RectTransform>();
			rectTransform.offsetMin = new Vector2(0, rectTransform.offsetMin.y);
			rectTransform.offsetMax = new Vector2(0, rectTransform.offsetMax.y);
			effectNameCell.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, heal ? healedEffectsYOffset : inflictedEffectsYOffset, 50);
			
			inflictedEffectsYOffset += !heal ? 50 : 0;
			healedEffectsYOffset += heal ? 50 : 0;

			foreach (CombinationOfMeds combo in sideEffect.responsibleMedCombinations)
			{
				// create cell
				Vector3 comboCellPosition =  Vector3.zero;
				GameObject comboCell = (GameObject) Instantiate (medCombinationCellPrefabsList[combo.responsibleMedication.Count-1], comboCellPosition, Quaternion.identity);
				List<string> medsNames = new List<string>();
				List<int> medsQties = new List<int>();
				foreach (MedAndQuantity medQty in combo.responsibleMedication)
				{
					medsNames.Add(medQty.medName);
					medsQties.Add(medQty.minimumDosageToApplyEffect);
				}
				comboCell.GetComponent<UIMedsCombinationPanelBehaviour>().SetMedsCombo(medsNames, medsQties, combo.knownCombination || !hideUnknownCombos);
				
				// set parent
				comboCell.transform.SetParent(heal ? healedEffectsContainer.transform : inflictedEffectsContainer.transform);
				
				// set size and position
				RectTransform comboCellRectTransform = comboCell.GetComponent<RectTransform>();
				comboCellRectTransform.offsetMin = new Vector2(0, comboCellRectTransform.offsetMin.y);
				comboCellRectTransform.offsetMax = new Vector2(0, comboCellRectTransform.offsetMax.y);
				comboCell.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, heal ? healedEffectsYOffset : inflictedEffectsYOffset, 50);

				inflictedEffectsYOffset += !heal ? 50 : 0;
				healedEffectsYOffset += heal ? 50 : 0;
			}
		}

		// Place
		float YOffset = 0;
		YOffset += help1Text.rect.height;
		inflictedEffectsContainer.GetComponent<RectTransform> ().SetInsetAndSizeFromParentEdge (RectTransform.Edge.Top, YOffset, inflictedEffectsYOffset);
		YOffset += inflictedEffectsContainer.GetComponent<RectTransform> ().rect.height;
		help2Text.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, YOffset, help2Text.rect.height);
		YOffset += help2Text.rect.height;
		healedEffectsContainer.GetComponent<RectTransform> ().SetInsetAndSizeFromParentEdge (RectTransform.Edge.Top, YOffset, healedEffectsYOffset);
		YOffset += healedEffectsContainer.GetComponent<RectTransform> ().rect.height;

		// Size of overall container
		this.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge (RectTransform.Edge.Top, 0, YOffset);
	}
}
