using Firebase.Database;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Text nameText;
    private string name;
    private DataUser user;

    void Start()
    {
        name = PlayerPrefs.GetString(Constant.KEY_NAME);
        FirebaseDatabase.DefaultInstance
            .GetReference("Users")
            .ValueChanged += GetUser;
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
                var n = childSnapshot.Child("name").Value.ToString();
                if (n == name)
                {
                    SetName(name);
                }
            }
        }
    }

    private void SetName(string name)
    {
        nameText.text = "Welcome " + name;
    }

    public void Play()
    {
        SceneManager.LoadScene("Scene-0_MainMenu");
    }

    public void EditProfile()
    {
        SceneManager.LoadScene("EditProfile");
    }

    public void Leaderboard()
    {
        SceneManager.LoadScene("Leaderboard");
    }

    public void Credits()
    {
        SceneManager.LoadScene("Credit");
    }

    public void Shop()
    {
        SceneManager.LoadScene("Shop");
    }

    public void Exit()
    {
        Application.Quit();
    }
}