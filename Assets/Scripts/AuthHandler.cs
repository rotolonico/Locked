using System.Collections;
using System.Collections.Generic;
using FullSerializer;
using Proyecto26;
using UnityEngine;

public class AuthHandler : MonoBehaviour
{
    public static readonly fsSerializer serializer = new fsSerializer();
    public static string userId;
    public static string idToken;
    public User user;
    private static readonly string AuthSignupURL = "https://www.googleapis.com/identitytoolkit/v3/relyingparty/signupNewUser?key=api_key";
    private static readonly string AuthSigninURL = "https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword?key=api_key";

    public static void SignupUser(string email, string username, string password)
    {
        string payLoad = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";
        RestClient.Post(AuthSignupURL, payLoad).Then(response =>
        {
            fsData data = fsJsonParser.Parse(response.Text);
            Debug.Log(response.Text);
            Dictionary<string, string> userInfo = null;
            serializer.TryDeserialize(data, ref userInfo).AssertSuccessWithoutWarnings();
            
            userId = userInfo["localId"];
            User user = new User(username, userId);
            RestClient.Put<User>(DatabaseHandler.DatabaseURL + "users/" + userId + ".json", user);
            
            SigninUser(email, password);
            
        }).Catch(error =>
        {
            GameObject.FindGameObjectWithTag("EditorHandler").GetComponent<EditorHandler>().SignupFailed();
        });
    }
    
    public static void SigninUser(string email, string password)
    {
        string payLoad = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";
        Debug.Log(payLoad);
        RestClient.Post(AuthSigninURL, payLoad).Then(response =>
        {
            fsData data = fsJsonParser.Parse(response.Text);
            SigninResponse signinResponse = new SigninResponse();
            serializer.TryDeserialize(data, ref signinResponse).AssertSuccessWithoutWarnings();
            userId = signinResponse.localId;
            Debug.Log("test");
            idToken = signinResponse.idToken;
            GameObject.FindGameObjectWithTag("EditorHandler").GetComponent<EditorHandler>().SigninSucceded();

        }).Catch(error =>
        {
            GameObject.FindGameObjectWithTag("EditorHandler").GetComponent<EditorHandler>().SigninFailed();
        });
    }
}
