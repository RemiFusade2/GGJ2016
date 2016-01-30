using UnityEngine;
using System.Collections;

public class PillContainerBehaviour : MonoBehaviour {

	public bool morning;
	public bool noon;
	public bool evening;

	public Transform morningPillsContainer;
	public Transform noonPillsContainer;
	public Transform eveningPillsContainer;
	public Transform outsidePillsContainer;

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.tag.Equals("Pill"))
		{
			if (morning)
			{				
				col.transform.parent = morningPillsContainer;
			}
			else if (noon)
			{				
				col.transform.parent = noonPillsContainer;
			}
			else if (evening)
			{				
				col.transform.parent = eveningPillsContainer;
			}
			else
			{				
				col.transform.parent = outsidePillsContainer;
			}
		}
	}
}
