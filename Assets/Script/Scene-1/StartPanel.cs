using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class StartPanel : MonoBehaviourPunCallbacks
{
    // Ui
    [SerializeField] private GameObject startButton;
    [SerializeField] private Text roomName;
    [SerializeField] private Text playerInRoom;

    // Game Manager
    private GameManager manager;

   
    void Start()
    {
        manager = FindObjectOfType<GameManager>();

        // Only room owner can start the game
        if (PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
        }
        else
        {
            startButton.SetActive(false);
        }
    }

    
    void Update()
    {
        // Update player in room
        playerInRoom.text = "Player In Room : " + PhotonNetwork.CurrentRoom.PlayerCount.ToString();
        roomName.text = "Room Name : " + PhotonNetwork.CurrentRoom.Name;
    }

    public void StartButtonMethod()
    {
        // Start The Games
        photonView.RPC("StartGames", RpcTarget.All);
        // Close the room
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
    }
    [PunRPC]
    public void StartGames()
    {
        // Spawn Player
        manager.SpawnPlayer();

        // Set first king
        manager.SetFirstKing();

        // Start Games
        manager.StartGames();

        // Close panel
        gameObject.SetActive(false);
    }
}
