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

    [SerializeField] private GameObject[] buttonGroups;
    [SerializeField] private bool[] isSelected;
    private int selectedSkin;

    // Game Manager
    private GameManager manager;

    // Color button
    private Color selectedColor = new Color(1, 1, 1, 130 / 225f);
    private Color unSelectedColor = new Color(1, 1, 1, 1);

    void Start()
    {
        manager = FindObjectOfType<GameManager>();

        isSelected = new bool[buttonGroups.Length];
        for(int i = 0; i < isSelected.Length; i++)
        {
            isSelected[i] = false;
        }

        selectedSkin = 10;

        photonView.RPC("SyncSelectedSkin", RpcTarget.All);

        startButton.SetActive(false);
    }

    
    void Update()
    {
        // Update player in room
        playerInRoom.text = "Player In Room : " + PhotonNetwork.CurrentRoom.PlayerCount.ToString();
        roomName.text = "Room Name : " + PhotonNetwork.CurrentRoom.Name;

        // Only room owner can start the game
        if (PhotonNetwork.IsMasterClient)
        {
            int count = 0; 
            foreach(bool a in isSelected)
            {
                if (a)
                {
                    count++;
                }
            }

            if(PhotonNetwork.CurrentRoom.PlayerCount == count)
            {
                startButton.SetActive(true);
            }
            else
            {
                startButton.SetActive(false);
            }
        }
    }

    public void StartButtonMethod()
    {
        // Start The Games
        photonView.RPC("StartGames", RpcTarget.All);
        // Close the room
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.CurrentRoom.IsOpen = false;
    }
    public void StartGames()
    {
        photonView.RPC("StartGamesOnline", RpcTarget.All);
    }
    [PunRPC]
    public void StartGamesOnline()
    {
        // Debug
        Debug.Log("Start Games");

        // Spawn Player
        manager.SpawnPlayer();

        // Start Games
        manager.StartGames();

        // Close panel
        gameObject.SetActive(false);
    }

    [PunRPC]
    public void SelectedSkinOnline(int oldButton, int newButton)
    {
        if(oldButton != 10)
        {
            isSelected[oldButton] = false;
        }
        isSelected[newButton] = true;

        for (int i = 0; i < buttonGroups.Length; i++)
        {
            if (isSelected[i])
            {
                buttonGroups[i].GetComponent<Image>().color = selectedColor;
            }
            else
            {
                buttonGroups[i].GetComponent<Image>().color = unSelectedColor;
            }
        }
        
        photonView.RPC("SyncSelectedSkin", RpcTarget.All);
    }
    public void SelectedSkin(int newButton)
    {
        if (!isSelected[newButton])
        {
            photonView.RPC("SelectedSkinOnline", RpcTarget.All, selectedSkin, newButton);
            selectedSkin = newButton;
        }
    }

    [PunRPC]
    public void SyncSelectedSkin()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SyncSkin", RpcTarget.All, isSelected);
        }
    }
    [PunRPC]
    public void SyncSkin(bool[] isSelected)
    {
        for (int i = 0; i < isSelected.Length; i++)
        {
            this.isSelected[i] = isSelected[i];

            if (isSelected[i])
            {
                buttonGroups[i].GetComponent<Image>().color = selectedColor;
            }
            else
            {
                buttonGroups[i].GetComponent<Image>().color = unSelectedColor;
            }
        }
    }

    [PunRPC]
    public void SyncID(int ID)
    {

    }
}
