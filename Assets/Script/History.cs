using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using UnityEngine;

public class History : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FirebaseDatabase.DefaultInstance
            .GetReference("Histories").Child(PlayerPrefs.GetString(Constant.KEY_ID))
            .ValueChanged += GetHistories;
        
        Debug.Log(Constant.KEY_ID);
    }

    private void GetHistories(object sender, ValueChangedEventArgs e2)
    {
        if (e2.DatabaseError != null)
        {
            Debug.LogError(e2.DatabaseError.Message);
        }

        if (e2.Snapshot != null && e2.Snapshot.ChildrenCount > 0)
        {
            foreach (var childSnapshot in e2.Snapshot.Children)
            {
                Debug.Log(childSnapshot.Child("score").Value.ToString());
            }
        }
    }
}
