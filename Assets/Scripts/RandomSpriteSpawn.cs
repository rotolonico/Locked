using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpriteSpawn : MonoBehaviour
{

	private EditorHandler editorHandler;
	private GameObject[] randomSpawnableBlocks;

	// Use this for initialization
	void Start ()
	{
		editorHandler = GameObject.Find("EditorHandler").GetComponent<EditorHandler>();
		randomSpawnableBlocks = editorHandler.randomSpawnableBlocks;
		
		GameObject randomBlock = randomSpawnableBlocks[EditorHandler.Random.Next(randomSpawnableBlocks.Length)];
		Instantiate(randomBlock, transform.position, Quaternion.identity);
		Destroy(gameObject);
	}
}
