using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LevelHandler : MonoBehaviour
{
    public GameObject LevelDisplayPrefab;
    private static Dictionary<string, LevelMetadata> levelsHolder;

    public delegate void OnLevelsMetadataDownloadCompleted(Dictionary<string, LevelMetadata> levels);
    public delegate void OnLevelDownloadCompleted(Level level);
    public delegate void OnCheckLikeCompleted(bool isLiked);

    private Button randomLevel;
    
    void Start()
    {   
        DatabaseHandler.GetAllLevelsMetadata(levelHolder =>
        {
            levelsHolder = levelHolder;
            foreach (var level in levelHolder)
            {
                var levelDisplay = Instantiate(LevelDisplayPrefab, GameObject.Find("LevelListContent").transform, false);
                levelDisplay.GetComponent<OnlineLevelButton>().levelId = level.Value.id;
                levelDisplay.transform.GetChild(0).GetComponent<Text>().text = level.Value.name;
                levelDisplay.transform.GetChild(1).GetComponent<Text>().text = "Created by: " + level.Value.author;
                levelDisplay.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = level.Value.likes.ToString();
                levelDisplay.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = level.Value.wins.ToString();
                levelDisplay.transform.GetChild(4).GetChild(0).GetComponent<Text>().text = level.Value.restarts.ToString();
            }
            EnableStuff();
        });
    }

    private void EnableStuff()
    {
        randomLevel = GameObject.Find("RandomLevel").GetComponent<Button>();
        randomLevel.interactable = true;
    }

    public static string GetRandomOnlineLevel()
    {
        var randomNumber = EditorHandler.Random.Next(0, levelsHolder.Count);
        var levelsHolderArray = levelsHolder.Values.ToArray();
        return levelsHolderArray[randomNumber].id;
    }


}
