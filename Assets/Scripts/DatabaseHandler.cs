
using System;
using System.Collections.Generic;
using FullSerializer;
using Proyecto26;
using UnityEngine;

public class DatabaseHandler
{    
    public static readonly fsSerializer serializer = new fsSerializer();
    private static readonly string DatabaseURL = "https://hidden.firebaseio.com/";

    public static void PostLevel(Level level)
    {
        level.id = ((long) (DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();

        RestClient.Put<Level>(DatabaseURL + "levels/" + level.id + ".json", level).Then(response => {
            
        });
    }

    public static void GetAllLevelsMetadata(LevelHandler.OnLevelsMetadataDownloadCompleted callback)
    {
        RestClient.Get(DatabaseURL + "levels.json").Then(response =>
        {
            Debug.Log(response.Text);
            fsData data = fsJsonParser.Parse(response.Text);
            Dictionary<string, LevelMetadata> levels = null;
            serializer.TryDeserialize(data, ref levels).AssertSuccessWithoutWarnings();
            callback(levels);
        });
    }

    public static void GetLevel(string levelId, LevelHandler.OnLevelDownloadCompleted callback)
    {
        RestClient.Get<Level>(DatabaseURL + "levels/" + levelId + ".json").Then(response =>
        {
            callback(response);
        });
    }
}