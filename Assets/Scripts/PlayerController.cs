using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public Sprite Background;
	public SpriteRenderer KeySr;
	public SpriteRenderer RedKeySr;
	public AudioSource GetKeySound;
	public Transform KeyImage;
	public Transform RedKeyImage;
	
	private readonly Vector3 screenSize = new Vector3(Screen.width/130,3,0);
	private Vector2 transformedPosition;
	private GameObject camera;
	private GameObject[] keyImages;
	private GameObject[] redKeyImages;
	private Rigidbody2D[] keyImagesRb;
	private Rigidbody2D[] redKeyImagesRb;
	private float screenWidth;
	private readonly Vector2 keyShift = new Vector2(1,0);

	public int HasKey;
	public int HasRedKey;

	void Start()
	{
		camera = GameObject.FindGameObjectWithTag("MainCamera");
		screenWidth = Screen.width;
		ReloadKeys();
	}

	private void Update()
	{
		if (HasKey > 0)
		{
			KeySr.enabled = true;
		}
		else
		{
			KeySr.enabled = false;
		}
		if (HasRedKey > 0)
		{
			RedKeySr.enabled = true;
		}
		else
		{
			RedKeySr.enabled = false;
		}
				
		if (Screen.width != screenWidth)
		{
			screenWidth = Screen.width;
			Rescale();
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("key") && other.GetComponent<SpriteRenderer>().sprite != Background)
		{
			HasKey += 1;
			other.GetComponent<SpriteRenderer>().sprite = Background;
			KeySr.enabled = true;
			KeyShift();
			Instantiate(KeyImage, camera.transform.position - screenSize, Quaternion.identity).transform.parent = GameObject.FindGameObjectWithTag("Canvas").transform;
			GetKeySound.Play();
		}
		if (other.gameObject.CompareTag("redKey") && other.GetComponent<SpriteRenderer>().sprite != Background)
		{
			HasRedKey += 1;
			other.GetComponent<SpriteRenderer>().sprite = Background;
			RedKeySr.enabled = true;
			KeyShift();
			Instantiate(RedKeyImage, camera.transform.position - screenSize, Quaternion.identity).transform.parent = GameObject.FindGameObjectWithTag("Canvas").transform;
			GetKeySound.Play();
		}
	}

	private void ReloadKeys()
	{
		keyImages = GameObject.FindGameObjectsWithTag("KeyImage");
		redKeyImages = GameObject.FindGameObjectsWithTag("RedKeyImage");
		keyImagesRb = new Rigidbody2D[keyImages.Length];
		redKeyImagesRb = new Rigidbody2D[redKeyImages.Length];
		for (var i = 0; i < keyImages.Length; i++)
		{
			keyImagesRb[i] = keyImages[i].GetComponent<Rigidbody2D>();
		}
		for (var i = 0; i < redKeyImages.Length; i++)
		{
			redKeyImagesRb[i] = redKeyImages[i].GetComponent<Rigidbody2D>();
		}
	}

	private void KeyShift()
	{
		ReloadKeys();
		foreach (var i in keyImagesRb)
		{
			i.MovePosition(i.position + keyShift);
		}
		foreach (var i in redKeyImagesRb)
		{
			i.MovePosition(i.position + keyShift);
		}
	}

	public void RemoveRedKey()
	{
		ReloadKeys();
		Debug.Log(redKeyImagesRb.Length);
		foreach (var i in keyImagesRb)
		{
			if (i.position.x > redKeyImagesRb[0].position.x)
			{
				i.MovePosition(i.position - keyShift);
			}
		}
		foreach (var j in redKeyImagesRb)
		{
			if (j.position.x > redKeyImagesRb[0].position.x)
			{
				j.MovePosition(j.position - keyShift);
			}
		}
		Destroy(redKeyImages[0]);
	}
	
	public void RemoveKey()
	{
		ReloadKeys();
		foreach (var i in keyImagesRb)
		{
			if (i.position.x > keyImagesRb[0].position.x)
			{
				i.MovePosition(i.position - keyShift);
			}
		}
		foreach (var j in redKeyImagesRb)
		{
			if (j.position.x > keyImagesRb[0].position.x)
			{
				j.MovePosition(j.position - keyShift);
			}
		}
		Destroy(keyImages[0]);
	}
	
	
	private void Rescale()
	{
		ReloadKeys();
		for (var i = 0; i > keyImagesRb.Length; i++)
		{
			keyImagesRb[i].MovePosition(camera.transform.position - screenSize + (Vector3)keyShift*i);
		}
		for (var i = 0; i > redKeyImagesRb.Length; i++)
		{
			redKeyImagesRb[i].MovePosition(camera.transform.position - screenSize + (Vector3)keyShift*(i+keyImagesRb.Length));
		}
		Debug.Log(screenWidth);
		Debug.Log(keyImagesRb);

	}
}
