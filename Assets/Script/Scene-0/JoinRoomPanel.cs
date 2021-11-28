using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class JoinRoomPanel : MonoBehaviourPunCallbacks
{
    [SerializeField] private InputField inputName;
    [SerializeField] private GameObject joinRoomButton;
    [SerializeField] private Text infoText;

    private int minNameSize = 4;
    private int maxNameSize = 10;

    void Start()
    {
        joinRoomButton.SetActive(false);
    }

    // Join Room
    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(inputName.text);
    }
    // If Join Failed
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        // And it's because ...
        if (message == "Game does not exist")
        {
            infoText.text = "Room Not Found !";
            StartCoroutine(InfoTextCooldown());
        }
    }
    private IEnumerator InfoTextCooldown()
    {
        yield return new WaitForSeconds(3);
        infoText.text = "";
    }

    // Check name character size
    public void MaxMinName()
    {
        if (inputName.text.Length < minNameSize || inputName.text.Length > maxNameSize)
        {
            joinRoomButton.SetActive(false);
        }
        else
        {
            joinRoomButton.SetActive(true);
        }
    }
}
