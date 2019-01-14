
using System;
using System.Collections.Generic;
using FullSerializer;
using Proyecto26;
using UnityEngine;

public class DatabaseHandler
{    
    public static readonly fsSerializer serializer = new fsSerializer();
    public static readonly string DatabaseURL = "https://project-id.firebaseio.com/";
    
    public static void PostLevel(Level level)
    {
        level.authorId = AuthHandler.userId;
        level.id = ((long) (DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();

        RestClient.Put<Level>(DatabaseURL + "levels/" + level.id + ".json?auth=" + AuthHandler.idToken, level).Then(response => {
            
        }).Catch(error =>
        {
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
    
    public static void CheckVersion(string version)
    {
        RestClient.Get(DatabaseURL + "gamedata/version.json").Then(response =>
        {
            version = "\"" + version + "\"";
            if (response.Text != version)
            {
                EditorHandler.CheckVersionOutdated();
            }
            else
            {
                EditorHandler.OnlineMode();
                GameObject.Find("MainCanvas").GetComponent<Canvas>().enabled = true;
            }

            EditorHandler.checkVersion = true;
        }).Catch(error =>
        {
            EditorHandler.CheckVersionFailed();
            EditorHandler.checkVersion = true;
        });
    }
    
    public static void GetNews()
    {
        RestClient.Get(DatabaseURL + "gamedata/news.json").Then(response => { EditorHandler.News = response.Text; });
    }
    
}