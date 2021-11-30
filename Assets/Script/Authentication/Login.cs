using Firebase.Database;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    private string nameExist, pwdExist;
    [SerializeField] private InputField name, pwd;
    [SerializeField] private Text err;

    public void Enter()
    {
        FirebaseDatabase.DefaultInstance
            .GetReference("Users")
            .ValueChanged += GetUserExist;
    }

    private void GetUserExist(object sender2, ValueChangedEventArgs e2)
    {
        if (e2.DatabaseError != null)
        {
            Debug.LogError(e2.DatabaseError.Message);
        }

        if (e2.Snapshot != null && e2.Snapshot.ChildrenCount > 0)
        {
            var check = false;
            int i = 0;
            foreach (var childSnapshot in e2.Snapshot.Children)
            {
                var id = childSnapshot.Child("id").Value.ToString();
                nameExist = childSnapshot.Child("name").Value.ToString();
                pwdExist = childSnapshot.Child("password").Value.ToString();

                if (name.text == nameExist)
                {
                    check = true;
                    if (pwd.text == pwdExist)
                    {
                        err.text = "";
                        PlayerPrefs.SetString(Constant.KEY_NAME, name.text);
                        PlayerPrefs.SetString(Constant.KEY_ID, id);
                        SceneManager.LoadScene("Story");
                    }
                    else
                    {
                        var message = "Password is Wrong!";
                        err.text = message;
                        print(message);
                    }
                }

                if (e2.Snapshot.ChildrenCount == i)
                    check = false;

                i++;
            }

            if (!check)
            {
                var message2 = "Username is not Exist!";
                err.text = message2;
                print(message2);
            }
        }
    }


    public void GotoRegister()
    {
        SceneManager.LoadScene("Register");
    }
}