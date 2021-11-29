using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameOverPanel : MonoBehaviourPunCallbacks
{
    public Text[] playerName;
    public Text[] playerKingTime;

    public void ContinueButton()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("Scene-0_PlayMenu");
    }
}
