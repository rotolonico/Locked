using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedLockController : MonoBehaviour
{
	public Sprite LockKey;
	public AudioSource UnlockSound;
	
	private GameObject player;
	private PlayerController playerController;
	private BoxCollider2D centerCollider;
	private SpriteRenderer sr;
	private SpriteRenderer pSr;
	private bool hasKey;

	void Start ()
	{
		sr = gameObject.transform.GetComponent<SpriteRenderer>();
		player = GameObject.FindGameObjectWithTag("Player");
		playerController = player.GetComponent<PlayerController>(); 
		centerCollider = player.GetComponent<BoxCollider2D>();
		pSr = GameObject.FindGameObjectWithTag("Player").transform.GetChild(4).GetComponent<SpriteRenderer>();
	}

	private void OnTriggerStay2D(Collider2D other)
	{
		if (!hasKey)
		{
			if (playerController.HasRedKey > 0)
			{
				sr.sprite = LockKey;
				UnlockSound.Play();
				playerController.HasRedKey -= 1;
				hasKey = true;
				transform.tag = "LockOpen";
				playerController.RemoveRedKey();
				
			}
		}

		if (other != centerCollider)
		{
			pSr.enabled = false;
			
		}
		if (other == centerCollider)
		{
			pSr.enabled = true;
			
		}
	}
}
