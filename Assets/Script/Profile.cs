using Firebase.Database;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Profile : MonoBehaviour
{
    private new string name;
    private string password, id;
    private string tempName;

    [SerializeField] private Text message;

    [Header("Input Field")] [SerializeField]
    private InputField inputName;

    [SerializeField] private InputField inputPassword;
    [SerializeField] private InputField inputScore;
    [SerializeField] private InputField inputWin;
    [SerializeField] private InputField inputLose;
    [SerializeField] private InputField inputMatch;
    [SerializeField] private InputField inputWinRate;

    [Header("Error Field")] [SerializeField]
    private Text errName;

    [SerializeField] private Text errPwd;

    void Start()
    {
        tempName = PlayerPrefs.GetString(Constant.KEY_NAME);
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
                var myName = childSnapshot.Child("name").Value.ToString();

                if (myName == PlayerPrefs.GetString(Constant.KEY_NAME))
                {
                    id = childSnapshot.Child("id").Value.ToString();
                    name = childSnapshot.Child("name").Value.ToString();
                    password = childSnapshot.Child("password").Value.ToString();
                    inputName.text = name;
                    inputPassword.text = password;
                    inputScore.text = childSnapshot.Child("score").Value.ToString();
                    inputWin.text = childSnapshot.Child("win").Value.ToString();
                    inputLose.text = childSnapshot.Child("lose").Value.ToString();
                    inputMatch.text = childSnapshot.Child("match").Value.ToString();
                    inputWinRate.text = childSnapshot.Child("winRate").Value.ToString();
                }

                Debug.Log(PlayerPrefs.GetString(Constant.KEY_NAME));
            }
        }
    }
    
    public void UpdateData()
    {
        if (NameCheck() && PwdCheck())
        {
            var reference = FirebaseDatabase.DefaultInstance.GetReference("Users").Child(id);
            reference.Child("name").SetValueAsync(inputName.text);
            reference.Child("password").SetValueAsync(inputPassword.text);
            PlayerPrefs.DeleteKey(Constant.KEY_NAME);
            PlayerPrefs.SetString(Constant.KEY_NAME, inputName.text);
            
            tempName = inputName.text;
            
            var myMessage = "Success";
            message.color = new Color32(30,145,50,255);
            message.text = myMessage;
            print(myMessage);
        }
        else
        {
            var myMessage = "Failed!";
            message.color = Color.red;
            message.text = myMessage;
        }
    }

    private bool PwdCheck()
    {
        if (inputPassword != null && inputPassword.text != "")
        {
            errPwd.text = "";
            return true;
        }

        var message = "Password must be filled!";
        errPwd.text = message;
        print(message);

        return false;
    }

    private bool NameCheck()
    {
        if (inputName != null && inputName.text != "" && name != inputName.text)
        {
            errName.text = "";
            return true;
        }

        else if (name == inputName.text && tempName != inputName.text)
        {
            var message = "Username has been used!";
            print(message);
            errName.text = message;
            return false;
        }
        else if(inputName.text == "")
        {
            var message = "Username must be filled!";
            print(message);
            errName.text = message;
            return false;
        }
        else
        {
            return true;
        }
    }

    public void Logout()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("Login");
    }
}