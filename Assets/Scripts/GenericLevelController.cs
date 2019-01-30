using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenericLevelController : MonoBehaviour
{
	
	public Sprite likeSprite;
	
	private EditorHandler editorHandler;
	private GameObject likeButton;
	private GameObject editButton;

	void Start ()
	{
		editorHandler = GameObject.Find("EditorHandler").GetComponent<EditorHandler>();
		likeButton = GameObject.Find("LikeButton");
		editButton = GameObject.Find("EditButton");
		Initialize();
	}

	public void Initialize()
	{
		editorHandler.ClearEditor();
		editorHandler.ChangeSize(true, EditorHandler.objectSavedLevel.levelRows, EditorHandler.objectSavedLevel.levelColumns);
		editorHandler.LoadLevel(EditorHandler.objectSavedLevel);
		EditorHandler.GameOver = false;
		if (EditorHandler.playingOnlineLevel && EditorHandler.isOnline)
		{
			DatabaseHandler.CheckLike(EditorHandler.onlineLevelId, isLiked =>
			{
				if (!isLiked)
				{
					likeButton.GetComponent<Button>().enabled = true;
					likeButton.GetComponent<Image>().sprite = likeSprite;
				}
				likeButton.GetComponent<Animator>().Play("GorightAnimation");
			});
			DatabaseHandler.CheckWin(EditorHandler.onlineLevelId, isWon =>
			{
				if (isWon)
				{
					editButton.GetComponent<Button>().enabled = true;
					editButton.GetComponent<Animator>().Play("GorightAnimation");
				}
			});
		}
	}
}
