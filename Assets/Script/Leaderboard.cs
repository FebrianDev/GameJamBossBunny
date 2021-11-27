using System.Collections.Generic;
using System.Linq;
using Firebase.Database;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Leaderboard : MonoBehaviour
{
    public GameObject item;
    [SerializeField] private Transform posNow;

    private void Start()
    {
        FirebaseDatabase.DefaultInstance
            .GetReference("Users").OrderByChild("score").LimitToLast(4)
            .ValueChanged += GetLeaderBoard;
    }
    
    private void GetLeaderBoard(object sender2, ValueChangedEventArgs e2)
    {
        if (e2.DatabaseError != null)
        {
            Debug.LogError(e2.DatabaseError.Message);
        }

        if (e2.Snapshot != null && e2.Snapshot.ChildrenCount > 0)
        {
            var i = 1;
            var listUser = new List<DataUser>();
            foreach (var childSnapshot in e2.Snapshot.Children)
            {
                listUser.Add(JsonUtility.FromJson<DataUser>(childSnapshot.GetRawJsonValue()));
            }

            var listSort = listUser.OrderByDescending(list => list.score).ToList();
            foreach (var list in listSort)
            {
                print(list.name);
                print(list.score);

                var go = Instantiate(item, new Vector3(0, 0, 0), Quaternion.identity);
                go.transform.parent = GameObject.Find("Panel").transform;
                go.transform.localPosition = new Vector2(-180, posNow.transform.localPosition.y - (40 * i));
                go.transform.localScale = new Vector3(1, 1, 1);

                var no = go.transform.GetChild(0).gameObject;
                no.GetComponent<Text>().text = i.ToString();
                var name = go.transform.GetChild(1).gameObject;
                name.GetComponent<Text>().text = list.name.ToString();
                var score = go.transform.GetChild(2).gameObject;
                score.GetComponent<Text>().text = list.score.ToString();

                i++;
            }
        }
    }

    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
