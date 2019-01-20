using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpriteSpawn : MonoBehaviour
{

	private EditorHandler editorHandler;
	private GameObject[] randomSpawnableBlocks;
	private GameObject[] randomSpawnablePortals;
	private GameObject[] randomSpawnableOther;
	private GameObject randomSpawnableLimitedBlock;
	
	public string randomType = "all";

	void Start ()
	{
		editorHandler = GameObject.Find("EditorHandler").GetComponent<EditorHandler>();
		randomSpawnableBlocks = editorHandler.randomSpawnableBlocks;
		randomSpawnablePortals = editorHandler.randomSpawnablePortals;
		randomSpawnableLimitedBlock = editorHandler.randomSpawnableLimitedBlock;
		randomSpawnableOther = editorHandler.randomSpawnableOther;

		GameObject randomBlock = null;

		if (randomType == "portal")
		{
			randomBlock = randomSpawnablePortals[EditorHandler.Random.Next(randomSpawnablePortals.Length)];
		}
		if (randomType == "number")
		{
			randomBlock = randomSpawnableLimitedBlock;
			var randomLimit = EditorHandler.Random.Next(9)+1;

			var limitedBlockController = randomBlock.GetComponent<LimitedBlockController>();
			limitedBlockController.limit = randomLimit;
			randomBlock.GetComponent<SpriteRenderer>().sprite = limitedBlockController.limitedSprites[randomLimit];
		}
		if (randomType == "other")
		{
			randomBlock = randomSpawnableOther[EditorHandler.Random.Next(randomSpawnableOther.Length)];
		}

		if (randomBlock == null)
		{
			randomBlock = randomSpawnableBlocks[EditorHandler.Random.Next(randomSpawnableBlocks.Length)];
		}

		Instantiate(randomBlock, transform.position, Quaternion.identity);
		Destroy(gameObject);
	}
}
