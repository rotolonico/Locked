using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class User
{
    public string userName;
    public string userId;

    public User(string userNameString, string userIdString)
    {
        userName = userNameString;
        userId = userIdString;
    }
}
