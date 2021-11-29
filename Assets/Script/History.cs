using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Firebase.Database;
using UnityEngine;
using UnityEngine.UI;

public class History : MonoBehaviour
{
    public GameObject item;
    [SerializeField] private Transform posNow;

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
            var i = 1;
            var listUser = new List<HistoryData>();
            foreach (var childSnapshot in e2.Snapshot.Children)
            {
                listUser.Add(JsonUtility.FromJson<HistoryData>(childSnapshot.GetRawJsonValue()));
            }

            foreach (var list in listUser)
            {
                if (i <= 7)
                {
                    var go = Instantiate(item, new Vector3(0, 0, 0), Quaternion.identity);
                    go.transform.parent = GameObject.Find("History").transform;
                    go.transform.localPosition = new Vector2(-97, posNow.transform.localPosition.y - (25 * i));
                    go.transform.localScale = new Vector3(1, 1, 1);

                    var win = go.transform.GetChild(0).gameObject;
                    if (list.win == 0)
                        win.GetComponent<Text>().text = "Lose";
                    else
                        win.GetComponent<Text>().text = "Win";
                        
                    
                    var score = go.transform.GetChild(1).gameObject;
                    score.GetComponent<Text>().text = list.score.ToString(CultureInfo.InvariantCulture);
                    i++;
                }
            }
        }
    }

    [Serializable]
    public struct HistoryData
    {
        public string name;
        public double score;
        public int win;
    }
}