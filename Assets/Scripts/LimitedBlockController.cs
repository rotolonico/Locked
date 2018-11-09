using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitedBlockController : MonoBehaviour
{
	public int limit = 1;
	public GameObject Hole;

	public Sprite[] limitedSprites = new Sprite[9];
	private SpriteRenderer sr;

	private void Start()
	{
		sr = gameObject.GetComponent<SpriteRenderer>();
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.name == "player")
		{
			limit -= 1;
			if (limit == 0)
			{
				Instantiate(Hole, transform.position, transform.rotation);
				Destroy(gameObject);
			}
			sr.sprite = limitedSprites[limit];
		}
	}

	private void Update()
	{
		
	}
}
