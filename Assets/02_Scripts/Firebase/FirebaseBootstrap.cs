using UnityEngine;
using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using System.Reflection;

public class FirebaseBootstrap : MonoBehaviour
{
    public static FirebaseAuth Auth { get; private set; }
    public static FirebaseFirestore Db { get; private set; }

    public static string Uid => Auth?.CurrentUser?.UserId;

    public async Task InitializeAndSignInAsync()
    {
        var status = await FirebaseApp.CheckAndFixDependenciesAsync();

        if(status != DependencyStatus.Available)
        {
            Debug.LogError($"Firebase dependency error : {status}");
            return;
        }

        Auth = FirebaseAuth.DefaultInstance;
        Db = FirebaseFirestore.DefaultInstance;

        if(Auth.CurrentUser == null)
        {
            var result = await Auth.SignInAnonymouslyAsync();
            Debug.Log($"Anonymous login succenss :{result.User.UserId}");
        }
        else
        {
            Debug.Log($"Already sign in : {Auth.CurrentUser.UserId}");

        }

    }
}
