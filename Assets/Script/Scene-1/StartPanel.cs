using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class StartPanel : MonoBehaviourPunCallbacks
{
    // Ui Text
    [SerializeField] private Text roomName;
    [SerializeField] private Text playerInRoom;

   
    void Start()
    {
        

    }

    
    void Update()
    {
        // Update player in room
        playerInRoom.text = "Player In Room : " + PhotonNetwork.CurrentRoom.PlayerCount.ToString();
        roomName.text = "Room Name : " + PhotonNetwork.CurrentRoom.Name;
    }

    public void StartButton()
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
        FindObjectOfType<GameManager>().SpawnPlayer();

        // Close panel
        gameObject.SetActive(false);
    }
}
