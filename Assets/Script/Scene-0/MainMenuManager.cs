using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviourPunCallbacks
{
    // Panels
    [SerializeField] private GameObject connectingToServerPanel;

    // Buttons
    [SerializeField] private GameObject selectJoystickButton;
    [SerializeField] private GameObject selectButtonButton;

    // Max PLayer in 1 room
    public byte MaxPlayerInRoom { get; private set; }

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
        MaxPlayerInRoom = 5;

        // Setting UI
        SelectController();
    }

    
    void Update()
    {
        
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
    public void PlayButton()
    {
        // Some UI
        //matchmakingPanel.SetActive(true);

        // Setting Player Name (Temporary random)
        PhotonNetwork.NickName = "User" + Random.Range(10, 100);

        // Join some Random Room
        PhotonNetwork.JoinRandomRoom();
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
        SceneManager.LoadScene(1);
        // Debugging
        Debug.Log("Joined to Room " + PhotonNetwork.CurrentRoom.Name);
    }

    // Exit Button --------------------------------------------------------------------------------------------------
    public void ExitButton()
    {
        Application.Quit();
    }
}
