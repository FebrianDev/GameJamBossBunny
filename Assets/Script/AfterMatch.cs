using System;
using System.Collections;
using Firebase.Database;
using UnityEngine;
using Random = UnityEngine.Random;

public class AfterMatch : MonoBehaviour
{
    #region Variable untuk menyimpan data player dari database

    private string id, name;
    private int win, lose, match;
    private double score;

    #endregion

    #region Data Dummy Untuk menyimpan data player ke firebase

    private string dummyName;
    private double dummyScore = 100;
    private int dummyWin = 1;
    private int dummyLose = 0;
    private int dummyMatch = 1;
    
    #endregion

    void Start()
    {
        
        //dummyName
        dummyName = PlayerPrefs.GetString(Constant.KEY_NAME);
        
        FirebaseDatabase.DefaultInstance
            .GetReference("Users")
            .ValueChanged += GetUser;


        //Test Demo
        StartCoroutine("Upload");
    }

    private void GetUser(object sender2, ValueChangedEventArgs e2)
    {
        if (e2.DatabaseError != null)
        {
            Debug.LogError(e2.DatabaseError.Message);
        }

        if (e2.Snapshot != null && e2.Snapshot.ChildrenCount > 0)
        {
            foreach (var childSnapshot in e2.Snapshot.Children)
            {
                var myName = childSnapshot.Child("name").Value.ToString();

                if (myName == PlayerPrefs.GetString(Constant.KEY_NAME))
                {
                    id = childSnapshot.Child("id").Value.ToString();
                    name = childSnapshot.Child("name").Value.ToString();
                    score = Convert.ToDouble(childSnapshot.Child("score").Value.ToString());
                    win = Convert.ToInt32(childSnapshot.Child("win").Value.ToString());
                    lose = Convert.ToInt32(childSnapshot.Child("lose").Value.ToString());
                    match = Convert.ToInt32(childSnapshot.Child("match").Value.ToString());
                }

                Debug.Log(PlayerPrefs.GetString(Constant.KEY_NAME));
            }
        }
    }

    private void UploadDataPlayerToFirebase()
    {
        var reference = FirebaseDatabase.DefaultInstance.GetReference("Users").Child(id);
        reference.Child("score").SetValueAsync(score + dummyScore);
        reference.Child("win").SetValueAsync(win + dummyWin);
        reference.Child("lose").SetValueAsync(lose + dummyLose);
        reference.Child("match").SetValueAsync(match + dummyMatch);
        reference.Child("winRate").SetValueAsync(((win + dummyWin) / (match + dummyMatch)) * 100);
    }

    private void UploadHistoryPlayer()
    {
        var reference = FirebaseDatabase.DefaultInstance.GetReference("Histories").Child(id).Child("his"+Random.Range(0,1000)+"tory" + Random.Range(0,1000000));
        reference.Child("name").SetValueAsync(name);
        reference.Child("score").SetValueAsync(dummyScore);
        reference.Child("win").SetValueAsync(dummyWin);
    }

    
    
   // Test Demo
     IEnumerator Upload()
     {
         yield return new WaitForSeconds(3);
         
         UploadDataPlayerToFirebase();
         UploadHistoryPlayer();
     }
}