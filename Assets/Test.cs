using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
         var reference = FirebaseDatabase.DefaultInstance.RootReference;
         reference.Child("Data").SetValueAsync(10);
    }
}
