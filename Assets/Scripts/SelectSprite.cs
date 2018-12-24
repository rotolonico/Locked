using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectSprite : MonoBehaviour
{
	public int spriteGameObjectId;
	public bool selected;
	private EditorHandler editorHandler;

	private void Start()
	{
		editorHandler = GameObject.Find("EditorHandler").GetComponent<EditorHandler>();
		if (CheckPosition())
		{
			editorHandler.placeSound.Play();			
		}
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(2) && !EventSystem.current.IsPointerOverGameObject())
		{
			var mousePositionRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			float dist;
			Plane mouseHitPlane = new Plane(Vector3.forward, transform.position);
			if (mouseHitPlane.Raycast(mousePositionRay, out dist))
			{
				Vector3 mousePosition = mousePositionRay.GetPoint(dist);
				mousePosition.x = (float) Math.Round(mousePosition.x);
				mousePosition.y = (float) Math.Round(mousePosition.y);
				mousePosition.z = (float) Math.Round(mousePosition.z);
				if (Physics2D.OverlapCircle(mousePosition, 0.3f) == gameObject.GetComponent<Collider2D>())
				{
					editorHandler.selectedObject = editorHandler.editorBlocks[spriteGameObjectId];
				}
			}
		}
	}

	public bool CheckPosition()
	{
		if (Mathf.Abs(transform.position.x) > editorHandler.levelColumns/2 || Mathf.Abs(transform.position.y) > editorHandler.levelRows/2)
		{
			Destroy(gameObject);
			return false;
		}
		else
		{
			return true;
		}
	}

	public void Select()
	{
		selected = true;
		gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
	}
	public void Deselect()
	{
		selected = false;
		var overlappingGameObjects = Physics2D.OverlapCircleAll(transform.position, 0.3f);
		foreach (var i in overlappingGameObjects)
		{
			if (i != gameObject.GetComponent<Collider2D>())
			{
				Destroy(i.gameObject);
			}
		}
		gameObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
	}

	public void Duplicate()
	{
		if (spriteGameObjectId != 1)
		{
			var overlappingGameObjects = Physics2D.OverlapCircleAll(transform.position, 0.3f);
			foreach (var i in overlappingGameObjects)
			{
				if (i != gameObject.GetComponent<Collider2D>())
				{
					Destroy(i.gameObject);
				}
			}
			Instantiate(editorHandler.editorBlocks[spriteGameObjectId], transform.position, Quaternion.identity);
		}
	}
}

