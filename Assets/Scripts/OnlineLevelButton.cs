using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnlineLevelButton : MonoBehaviour
{
    public string levelId;

    public void PlayButtonLevel()
    {
        var allButtons = GameObject.FindGameObjectsWithTag("GenericLevelButton");

        foreach (var button in allButtons)
        {
            button.GetComponent<Button>().enabled = false;
        }
        
        DatabaseHandler.GetLevel(levelId, level =>
        {
            GameObject.FindGameObjectWithTag("EditorHandler").GetComponent<EditorHandler>().LoadOnlineLevelInLevelScene(level);
            EditorHandler.playingOnlineLevel = true;
        });
    }
}
