using UnityEngine;

public class AuthHandler : MonoBehaviour
{
/*
private static FirebaseAuth authInstance;
private static FirebaseApp app;


public static void RegisterUser(String email, String password)
{
    initFirebase();
    FirebaseAuth.GetAuth(app).CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
    {
        if (task.IsCompleted)
        {
            if (task.IsFaulted)
            {
                if (task.Exception != null) Debug.Log(task.Exception);
                AggregateException ex = task.Exception as AggregateException;
                if (ex != null) {
                    Firebase.FirebaseException fbEx = null;
                    foreach (Exception e in ex.InnerExceptions) {
                        fbEx = e as Firebase.FirebaseException;
                        if (fbEx != null)
                            break;
                    }

                    if (fbEx != null) {
                        Debug.Log("Encountered a FirebaseException:" + fbEx.Message);
                    }
                }
            }
            else
            {
                Debug.Log("Successfully registered");
                LoginUser(email, password);
            }
        }
    });
}

public static void LoginUser(String email, String password)
{
    initFirebase();
    FirebaseAuth.GetAuth(app).SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
    {
        if (task.IsCompleted)
        {
            if (task.IsFaulted)
            {
               if (task.Exception != null) Debug.Log(task.Exception.Message);
                
                AggregateException ex = task.Exception as AggregateException;
                if (ex != null) {
                    Firebase.FirebaseException fbEx = null;
                    foreach (Exception e in ex.InnerExceptions) {
                        fbEx = e as Firebase.FirebaseException;
                        if (fbEx != null)
                            break;
                    }

                    if (fbEx != null) {
                        Debug.Log("Encountered a FirebaseException:" + fbEx.Message);
                    }
                }
            }
            else
            {
                Debug.Log("Successfully login");
            }
        }
    });
}

private static void initFirebase()
{
    AppOptions ops = new Firebase.AppOptions();
    
    app = FirebaseApp.Create(ops, "Secondary");

    // NOTE: You'll need to replace this url with your Firebase App's database
    // path in order for the database connection to work correctly in editor.
    app.SetEditorDatabaseUrl("https://locked-3426c.firebaseio.com/");
    if (app.Options.DatabaseUrl != null) app.SetEditorDatabaseUrl(app.Options.DatabaseUrl);
}
*/
}
