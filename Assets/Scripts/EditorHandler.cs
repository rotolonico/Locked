using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EditorHandler : MonoBehaviour {
	
	public GameObject selectedObject;
	public AudioSource EditorMusic;
	
	private Plane mouseHitPlane;
	private Ray mousePositionRay;
	private Collider2D[] hitColliders;
	private bool musicOn;
	private CameraController cameraController; 
	
	private int levelRows = 96;
	private int levelColumns = 96;	

	private String savedLevel;
	
	public GameObject[] blocks = new GameObject[31];
	public GameObject[] editorBlocks = new GameObject[31];

	private bool playMode;

	void Start()
	{
		DontDestroyOnLoad(gameObject);
		cameraController = Camera.main.GetComponent<CameraController>();
		mouseHitPlane = new Plane(Vector3.forward, transform.position);
	}
	
	
	void Update () {
		
		if (!playMode)
		{
			if (!musicOn)
			{
				EditorMusic.Play();
				musicOn = true;
			}

			if (Input.GetMouseButtonDown(0))
			{
				mousePositionRay = Camera.main.ScreenPointToRay(Input.mousePosition);
				float dist;
				if (mouseHitPlane.Raycast(mousePositionRay, out dist))
				{
					Vector3 mousePosition = mousePositionRay.GetPoint(dist);
					mousePosition.x = (float) Math.Round(mousePosition.x);
					mousePosition.y = (float) Math.Round(mousePosition.y);
					mousePosition.z = (float) Math.Round(mousePosition.z);
					hitColliders = Physics2D.OverlapCircleAll(mousePosition, 0.3f);
					if (!isTouchingUI())
					{
						foreach (var i in hitColliders)
						{
							Destroy(i.gameObject);
						}

						if (selectedObject.CompareTag("1"))
						{
							Destroy(GameObject.FindGameObjectWithTag("1"));
						}
						if (!selectedObject.CompareTag("0"))
						{
							Instantiate(selectedObject, mousePosition, Quaternion.identity);
						}
					}
				}
			}
		}
	}
	
	bool isTouchingUI()
	{
		foreach (var i in hitColliders)
		{
			if (i.CompareTag("UI"))
			{
				return true;
			}
		}

		return false;
	}
	
	public void PlayMode()
	{
		musicOn = false;
		EditorMusic.Stop();
		var level = SaveLevel();
		ClearEditor();
		LoadLevel(level);
	}

	private string SaveLevel()
	{
		var level = new string[levelRows,levelColumns];
		
		for (var i = 0; i < levelColumns; i++)
		{
			for (var j = 0; j < levelRows; j++)
			{
				var circlePos = new Vector2(-levelRows/2 + j,-levelColumns/2 + i);
				if (Physics2D.OverlapCircle(circlePos, 0.3f) == null)
				{
					level[j,i] = "0";
				}
				else
				{
					var blockHit = Physics2D.OverlapCircle(circlePos, 0.3f);
					level[j,i] = blockHit.tag;
				}	
			}
		}

		var levelString = new string(string.Join(",",level.Cast<string>().ToArray()).ToCharArray());
		
		return levelString; 
	}

	private void ClearEditor()
	{
		var levelEdgeA = new Vector2(-levelRows/2, -levelColumns/2);
		var levelEdgeB = new Vector2(levelRows/2, levelColumns/2);
		var gameobjectsToClear = Physics2D.OverlapAreaAll(levelEdgeA, levelEdgeB);
		foreach (var i in gameobjectsToClear)
		{
			Destroy(i.gameObject);
		}
	}

	private void LoadLevel(string level)
	{
		var levelStrings = level.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
	    
		for (var i = 0; i < levelColumns; i++)
		{
			for (var j = 0; j < levelRows; j++)
			{
				int k;
				if (j + i * levelRows < levelRows*levelColumns && Int32.TryParse(levelStrings[i + j * levelRows], out k))
				{
					if (k != 0)
					{
						var blockPosition = new Vector2(-levelRows / 2 + j, -levelColumns / 2 + i);
						Instantiate(blocks[k], blockPosition, Quaternion.identity);
					}
				}
			}
		}

		playMode = true;
		cameraController.Initialize();
	}
	
	private void LoadLevelInEditor(string level)
	{
		var levelStrings = level.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
	    
		for (var i = 0; i < levelColumns; i++)
		{
			for (var j = 0; j < levelRows; j++)
			{
				int k;
				if (j + i * levelRows < levelRows*levelColumns && Int32.TryParse(levelStrings[i + j * levelRows], out k))
				{
					if (k != 0)
					{
						var blockPosition = new Vector2(-levelRows / 2 + j, -levelColumns / 2 + i);
						Instantiate(editorBlocks[k], blockPosition, Quaternion.identity);
					}
				}
			}
		}
		
		playMode = false;
	}
}
