
using System;
using System.Collections.Generic;
using FullSerializer;
using Proyecto26;
using UnityEngine;
using Object = UnityEngine.Object;

public class DatabaseHandler
{    
    public static readonly fsSerializer serializer = new fsSerializer();
    public static readonly string DatabaseURL = "https://project-id.firebaseio.com/";
    
    public static void PostLevel(Level level)
    {
        //level.authorId = AuthHandler.idToken;
        Debug.Log(AuthHandler.idToken);
        level.id = ((long) (DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();

        RestClient.Put<Level>(DatabaseURL + "levels/" + level.id + ".json?auth=" + AuthHandler.idToken, level).Then(response => {
            
        }).Catch(error =>
        {
            Debug.Log(error.Message);
        });
    }
    

    public static void GetAllLevelsMetadata(LevelHandler.OnLevelsMetadataDownloadCompleted callback)
    {
        RestClient.Get(DatabaseURL + "levels.json?auth=" + AuthHandler.idToken).Then(response =>
        {
            fsData data = fsJsonParser.Parse(response.Text);
            Dictionary<string, LevelMetadata> levels = null;
            serializer.TryDeserialize(data, ref levels).AssertSuccessWithoutWarnings();
            callback(levels);
        });
    }

    public static void GetLevel(string levelId, LevelHandler.OnLevelDownloadCompleted callback)
    {
        RestClient.Get<Level>(DatabaseURL + "levels/" + levelId + ".json?auth=" + AuthHandler.idToken).Then(response =>
        {
            callback(response);
        });
    }
}