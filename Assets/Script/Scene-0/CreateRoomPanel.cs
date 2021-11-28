using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CreateRoomPanel : MonoBehaviourPunCallbacks
{
    [SerializeField] private InputField inputRoomName;
    [SerializeField] private GameObject createRoomButton;
    [SerializeField] private Text maxPlayer;
    private int maxPlayerCount;
    [SerializeField] private Text visibility;
    private bool isPublic;

    private void Start()
    {
        createRoomButton.SetActive(false);
        maxPlayerCount = int.Parse(maxPlayer.text);
        if (visibility.text == "Public")
        {
            isPublic = true;
        }
        else
        {
            isPublic = false;
        }
    }

    // Check room name length
    public void MaxMinName()
    {
        if (inputRoomName.text.Length < 4 || inputRoomName.text.Length > 14)
        {
            createRoomButton.SetActive(false);
        }
        else
        {
            createRoomButton.SetActive(true);
        }
    }

    // Increment and decrement max player
    public void PlusMaxPlayer()
    {
        if (maxPlayerCount < 5)
        {
            maxPlayerCount++;
            maxPlayer.text = maxPlayerCount.ToString();
        }
    }
    public void MinMaxPlayer()
    {
        if (maxPlayerCount > 2)
        {
            maxPlayerCount--;
            maxPlayer.text = maxPlayerCount.ToString();
        }
    }

    // Change room visibility
    public void ChangeVisibility()
    {
        if (visibility.text == "Public")
        {
            visibility.text = "Private";
        }
        else
        {
            visibility.text = "Public";
        }
    }

    // Make mew room
    public void CreateRoom()
    {
        // Make a new room
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = isPublic;
        roomOptions.MaxPlayers = (byte)maxPlayerCount;
        PhotonNetwork.CreateRoom(inputRoomName.text, roomOptions, TypedLobby.Default);
    }
}
