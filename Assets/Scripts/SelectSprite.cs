using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectSprite : MonoBehaviour
{
	public int spriteGameObjectId;
	public bool selected;
	private EditorHandler editorHandler;

	private void Start()
	{
		editorHandler = GameObject.Find("EditorHandler").GetComponent<EditorHandler>();
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
