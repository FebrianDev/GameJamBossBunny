using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class PlayMenuManager : MonoBehaviourPunCallbacks
{
    // Panels
    [SerializeField] private GameObject connectingToServerPanel;
    [SerializeField] private GameObject createRoomPanel;
    [SerializeField] private GameObject joinRoomPanel;

    // Buttons
    [SerializeField] private GameObject selectJoystickButton;
    [SerializeField] private GameObject selectButtonButton;

    // Max PLayer in 1 room
    public byte MaxPlayerInRoom { get; private set; }

    // Scroll control
    [SerializeField] private GameObject scrollBar;
    [SerializeField] private GameObject content;
    [SerializeField] private Text selectedText;
    float dist;
    float scrollPos = 0;
    float[] allScrollPos;
    int SelectedPos;
    private Color selectedColor;
    private Color deselectedColor;

    void Start()
    {
        // Check if connected
        if (!PhotonNetwork.IsConnected)
        {
            // Some UI
            connectingToServerPanel.SetActive(true);

            // Connect to server
            PhotonNetwork.ConnectUsingSettings();
        }

        // Set Max Player
        MaxPlayerInRoom = 4;

        // Setting UI
        SelectController();

        // Scroll view
        allScrollPos = new float[3];
        dist = 1f / (allScrollPos.Length - 1f);
        for (int i = 0; i < allScrollPos.Length; i++)
        {
            allScrollPos[i] = dist * i;
        }
        SelectedPos = 1;
        scrollBar.GetComponent<Scrollbar>().value = allScrollPos[SelectedPos];
        scrollPos = allScrollPos[SelectedPos];
        selectedColor = new Color(1, 1, 1, 1);
        deselectedColor = new Color(1, 1, 1, 130 / 255f);
    }

    
    void Update()
    {
        ScrollControll();
    }

    // Controll scrolling
    private void ScrollControll()
    {
        if (Input.GetMouseButton(0))
        {
            scrollPos = scrollBar.GetComponent<Scrollbar>().value;
        }
        else
        {
            for (int i = 0; i < allScrollPos.Length; i++)
            {
                if (scrollPos < allScrollPos[i] + (dist / 2) && scrollPos > allScrollPos[i] - (dist / 2))
                {
                    scrollBar.GetComponent<Scrollbar>().value = Mathf.Lerp(scrollBar.GetComponent<Scrollbar>().value, allScrollPos[i], .08f);
                    SelectedPos = i;
                    ChangeButtonText(i);
                }
            }
        }

        for (int i = 0; i < allScrollPos.Length; i++)
        {
            if (scrollPos < allScrollPos[i] + (dist / 2) && scrollPos > allScrollPos[i] - (dist / 2))
            {
                content.transform.GetChild(i).localScale = Vector2.Lerp(content.transform.GetChild(i).localScale, new Vector2(1, 1), .05f);
                content.transform.GetChild(i).GetComponent<Image>().color = selectedColor;
            }
            else
            {
                content.transform.GetChild(i).localScale = Vector2.Lerp(content.transform.GetChild(i).localScale, new Vector2(.65f, .65f), .05f);
                content.transform.GetChild(i).GetComponent<Image>().color = deselectedColor;
            }
        }
    }
    public void NextPrevButton(bool isNext)
    {
        if (isNext && SelectedPos <= 1)
        {
            SelectedPos += 1;
        }
        else if (!isNext && SelectedPos >= 1)
        {
            SelectedPos -= 1;
        }

        scrollBar.GetComponent<Scrollbar>().value = allScrollPos[SelectedPos];
        scrollPos = allScrollPos[SelectedPos];
        ChangeButtonText(SelectedPos);
    }
    private void ChangeButtonText(int value)
    {
        if(value == 0)
        {
            selectedText.text = "Create Room";
        }
        else if(value == 1)
        {
            selectedText.text = "Quick Match";
        }
        else if (value == 2)
        {
            selectedText.text = "Join Room";
        }
    }

    // If Connected to server
    public override void OnConnectedToMaster()
    {
        // Some UI
        connectingToServerPanel.SetActive(false);
    }

    // Method For select controller
    public void SelectController()
    {
        if (PlayerManager.isJoystick)
        {
            selectJoystickButton.SetActive(false);
            selectButtonButton.SetActive(true);
            PlayerManager.isJoystick = false;
        }
        else
        {
            selectJoystickButton.SetActive(true);
            selectButtonButton.SetActive(false);
            PlayerManager.isJoystick = true;
        }
    }

    // Method for play button (Join Room) --------------------------------------------------------------------------
    public void QuickMatchButton()
    {
        if (SelectedPos == 1)
        {
            // Some UI
            //matchmakingPanel.SetActive(true);

            // Setting Player Name (Temporary random)
            PhotonNetwork.NickName = PlayerPrefs.GetString(Constant.KEY_NAME);

            // Join some Random Room
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            SelectedPos = 1;
            scrollBar.GetComponent<Scrollbar>().value = allScrollPos[SelectedPos];
            scrollPos = allScrollPos[SelectedPos];
            ChangeButtonText(SelectedPos);
        }
    }
    public void CreateButton()
    {
        if (SelectedPos == 0)
        {
            createRoomPanel.SetActive(true);
        }
        else
        {
            SelectedPos = 0;
            scrollBar.GetComponent<Scrollbar>().value = allScrollPos[SelectedPos];
            scrollPos = allScrollPos[SelectedPos];
            ChangeButtonText(SelectedPos);
        }
    }
    public void JoinButton()
    {
        if (SelectedPos == 2)
        {
            createRoomPanel.SetActive(true);
        }
        else
        {
            SelectedPos = 2;
            scrollBar.GetComponent<Scrollbar>().value = allScrollPos[SelectedPos];
            scrollPos = allScrollPos[SelectedPos];
            ChangeButtonText(SelectedPos);
        }
    }
    // If Join Room Failed (there is no room avaliable)
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // And it's because there is no room
        if (message == "No match found")
        {
            // Make a new room
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.IsVisible = true;
            roomOptions.MaxPlayers = MaxPlayerInRoom;
            // Room name temporary random
            PhotonNetwork.CreateRoom("Room" + Random.Range(10, 100), roomOptions, TypedLobby.Default);
        }
    }
    // If Joined to room
    public override void OnJoinedRoom()
    {
        // Load Game Scene
        SceneManager.LoadScene("Scene-1_Gameplay");
        // Debugging
        Debug.Log("Joined to Room " + PhotonNetwork.CurrentRoom.Name);
    }

    // Back Button --------------------------------------------------------------------------------------------------
    public void BackButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
