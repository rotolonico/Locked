
using System;
using System.Collections.Generic;
using FullSerializer;
using Proyecto26;
using UnityEngine;
using UnityScript.Steps;

public class DatabaseHandler
{    
    public static readonly fsSerializer serializer = new fsSerializer();
    public static readonly string DatabaseURL = "https://project-id.firebaseio.com/";
    
    public static void PostLevel(Level level)
    {
        level.authorId = AuthHandler.userId;
        level.id = ((long) (DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();

        RestClient.Put<Level>(DatabaseURL + "levels/" + level.id + ".json?auth=" + AuthHandler.idToken, level);
    }
    
    public static void LikeLevel(string levelId)
    {
        string likes = "1";
        string liked = "1";
        
        RestClient.Put<string>(DatabaseURL + "users/" + AuthHandler.userId + "/stats/levels/" + levelId + "/liked.json?auth=" + AuthHandler.idToken,
            liked).Catch(error =>
        {
            Debug.Log(error);
        });
        
        RestClient.Get(DatabaseURL + "levels/" + levelId + "/likes.json?auth=" + AuthHandler.idToken).Then(response =>
            {
                if (response.Text != "null")
                {
                    likes = (int.Parse(likes) + int.Parse(response.Text)).ToString();
                }

                    RestClient.Put<string>(DatabaseURL + "levels/" + levelId + "/likes.json?auth=" + AuthHandler.idToken, likes).Then(
                        response2 =>
                        {
                            
                        }).Catch(error =>
                    {
                        Debug.Log(error);
                    });
            }).Catch(error =>
        {
            Debug.Log(error);
        });
    }
    
    public static void CheckLike(string levelId, LevelHandler.OnCheckLikeCompleted callback)
    {   
        RestClient.Get(DatabaseURL + "users/" + AuthHandler.userId + "/stats/levels/" + levelId + "/liked.json?auth=" + AuthHandler.idToken).Then(response =>
            {
                if (response.Text != "null")
                {
                    callback(true);
                }
                else
                {
                    callback(false);
                }
            }).Catch(error =>
        {
            Debug.Log(error);
        });
    }
    
    public static void WinLevel(string levelId)
    {
        string wins = "1";
        string won = "1";
        
        RestClient.Put<string>(DatabaseURL + "users/" + AuthHandler.userId + "/stats/levels/" + levelId + "/won.json?auth=" + AuthHandler.idToken,
            won).Catch(error =>
        {
            Debug.Log(error);
        });
        
        RestClient.Get(DatabaseURL + "levels/" + levelId + "/wins.json?auth=" + AuthHandler.idToken).Then(response =>
        {
            if (response.Text != "null")
            {
                wins = (int.Parse(wins) + int.Parse(response.Text)).ToString();
            }

            RestClient.Put<string>(DatabaseURL + "levels/" + levelId + "/wins.json?auth=" + AuthHandler.idToken, wins).Then(
                response2 =>
                {
                            
                }).Catch(error =>
            {
                Debug.Log(error);
            });
        }).Catch(error =>
        {
            Debug.Log(error);
        });
    }
    
    public static void CheckWin(string levelId, LevelHandler.OnCheckWinCompleted callback)
    {   
        RestClient.Get(DatabaseURL + "users/" + AuthHandler.userId + "/stats/levels/" + levelId + "/won.json?auth=" + AuthHandler.idToken).Then(response =>
        {
            if (response.Text != "null")
            {
                callback(true);
            }
            else
            {
                callback(false);
            }
        }).Catch(error =>
        {
            Debug.Log(error);
        });
    }
    
    public static void RestartLevel(string levelId)
    {
        string restarts = "1";
        
        RestClient.Get(DatabaseURL + "levels/" + levelId + "/restarts.json?auth=" + AuthHandler.idToken).Then(response =>
        {
            if (response.Text != "null")
            {
                restarts = (int.Parse(restarts) + int.Parse(response.Text)).ToString();
            }

            RestClient.Put<string>(DatabaseURL + "levels/" + levelId + "/restarts.json?auth=" + AuthHandler.idToken, restarts).Then(
                response2 =>
                {
                            
                }).Catch(error =>
            {
                Debug.Log(error);
            });
        }).Catch(error =>
        {
            Debug.Log(error);
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
        }).Catch(error =>
        {
            Debug.Log(error);
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