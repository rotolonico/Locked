using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectSprite : MonoBehaviour
{
	public bool selected;
	private static int maxSortingLayer;

	public void Select ()
	{
		selected = true;
		gameObject.GetComponent<SpriteRenderer>().sortingOrder = maxSortingLayer+1;
		maxSortingLayer += 1;
	}
}
