using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public Transform Player;
	public Sprite PlayerKey;
	public Sprite PlayerRedKey;
	public Sprite Background;
	public SpriteRenderer Sr;
	public static bool Up = true;
	public static bool Down = true;
	public static bool Left = true;
	public static bool Right = true;

	public AudioSource MoveSound;
	public AudioSource GetKeySound;
	public AudioSource HitWall;
	
	private Vector2 transformedPosition;

	public static bool HasKey;
	public static bool HasRedKey;
	
	void Update ()
	{
		if (Swipe.SwipeUp)
		{
			if (Up)
			{
				Player.transform.position += Vector3.up;
				Up = Down = Right = Left = true;
				MoveSound.Play();
			}
			else
			{
				HitWall.Play();
			}

		}

		if (Swipe.SwipeLeft)
		{
			if (Left)
			{
				Player.transform.position += Vector3.left;
				Up = Down = Right = Left = true;
				MoveSound.Play();
			}
			else
			{
				HitWall.Play();
			}
		}

		if (Swipe.SwipeDown)
		{
			if (Down)
			{
				Player.transform.position += Vector3.down;
				Up = Down = Right = Left = true;
				MoveSound.Play();
			}
			else
			{
				HitWall.Play();
			}
		}

		if (Swipe.SwipeRight)
		{
			if (Right)
			{
				Player.transform.position += Vector3.right;
				Up = Down = Right = Left = true;
				MoveSound.Play();
			}
			else
			{
				HitWall.Play();
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("key") && other.GetComponent<SpriteRenderer>().sprite != Background)
		{
			HasKey = true;
			Sr.sprite = PlayerKey;
			other.GetComponent<SpriteRenderer>().sprite = Background;
			GetKeySound.Play();
		}
		if (other.gameObject.CompareTag("redKey") && other.GetComponent<SpriteRenderer>().sprite != Background)
		{
			HasRedKey = true;
			Sr.sprite = PlayerRedKey;
			other.GetComponent<SpriteRenderer>().sprite = Background;
			GetKeySound.Play();
		}
	}
}
