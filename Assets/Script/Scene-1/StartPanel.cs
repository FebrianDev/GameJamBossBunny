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

        // Only room owner can start the game
        if (PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
        }
        else
        {
            startButton.SetActive(false);
        }

        isSelected = new bool[buttonGroups.Length];
        for(int i = 0; i < isSelected.Length; i++)
        {
            isSelected[i] = new bool();
        }

        PickSkin();
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

    private void PickSkin()
    {
        for (int i = 0; i < buttonGroups.Length; i++)
        {
            if (!isSelected[i])
            {
                photonView.RPC("SelectedSkinOnline", RpcTarget.All, i, i);
                selectedSkin = i;
                return;
            }
        }
    }
    [PunRPC]
    public void SelectedSkinOnline(int oldButton, int newButton)
    {
        if (!isSelected[newButton])
        {
            isSelected[oldButton] = false;
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
        }
    }
    public void SelectedSkin(int newButton)
    {
        photonView.RPC("SelectedSkinOnline", RpcTarget.All, selectedSkin, newButton);
        selectedSkin = newButton;
    }
}
