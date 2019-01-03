using System.Collections;
using System.Collections.Generic;
using FullSerializer;
using Proyecto26;
using UnityEngine;

public class AuthHandler : MonoBehaviour
{
    public static readonly fsSerializer serializer = new fsSerializer();
    public static string userId;
    private static readonly string AuthSignupURL = "hidden";
    private static readonly string AuthSigninURL = "hidden";
    
    
    public static void SignupUser(string email, string password)
    {
        string payLoad = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";
        RestClient.Post(AuthSignupURL, payLoad).Then(response =>
        {
            fsData data = fsJsonParser.Parse(response.Text);
            Debug.Log(response.Text);
            Dictionary<string, string> userInfo = null;
            serializer.TryDeserialize(data, ref userInfo).AssertSuccessWithoutWarnings();
            userId = userInfo["localId"];
        }).Catch(error =>
        {
            Debug.Log(error.Message);
        });
    }
    
    public static void SigninUser(string email, string password)
    {
        string payLoad = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";
        RestClient.Post(AuthSigninURL, payLoad).Then(response =>
        {
            fsData data = fsJsonParser.Parse(response.Text);
            Debug.Log(response.Text);
            Dictionary<string, string> userInfo = null;
            serializer.TryDeserialize(data, ref userInfo).AssertSuccessWithoutWarnings();
            userId = userInfo["localId"];
        }).Catch(error =>
        {
            Debug.Log(error.Message);
        });
    }
}
