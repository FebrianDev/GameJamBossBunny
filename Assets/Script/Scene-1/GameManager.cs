using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class GameManager : MonoBehaviourPunCallbacks
{
    // Panels
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject gameOverPanel;

    // Players UI
    public GameObject[] PlayersUI;
    public static bool[] PlayerUIisSelected;

    // Buttons
    public GameObject JumpButton;
    public GameObject HitButton;
    public GameObject MoveLeftButton;
    public GameObject MoveRightButton;

    // UI
    public Image HitButtonLoading;

    // Prefabs
    [SerializeField] private GameObject[] playerPrefabs;

    // Timer
    public static bool GameIsRolling;
    public float PlayTimeLimit { get; private set; }
    public float playTimeCooldown { get; private set; }
    [SerializeField] private Text timerText;

    // Database
    public const byte SEND_DATA_EVENT = 77;
    [SerializeField] private GameObject dataBaseObject;

    // Crown
    [SerializeField] private GameObject movingPlatform;
    [SerializeField] private Transform movingPlatformSpawnPoint;

    // Avatar
    public Sprite[] avatar;
    public int selectedAvatar;

    void Start()
    {
        // Open start panel
        startPanel.SetActive(true);

        // Setup
        PlayTimeLimit = 180f;
        playTimeCooldown = PlayTimeLimit;
        ChangeTimeUI(playTimeCooldown, timerText);

        // Turn off all UI
        foreach(GameObject a in PlayersUI)
        {
            a.SetActive(false);
        }
        PlayerUIisSelected = new bool[PlayersUI.Length];
        for(int i = 0; i < PlayerUIisSelected.Length; i++)
        {
            PlayerUIisSelected[i] = false;
        }
    }

    void Update()
    {
        if (GameIsRolling && playTimeCooldown > 0)
        {
            playTimeCooldown -= Time.deltaTime;
            ChangeTimeUI(playTimeCooldown, timerText);
        }
        else if(playTimeCooldown <= 0 && PhotonNetwork.IsMasterClient)
        {
            // Time out
            GameOverOnline();
        }
    }

    // Spawning Player
    public void SpawnPlayer(int playerID)
    {
        SpawnPlayer(new Vector3(0, 0, 0), playerID);
    }
    public void SpawnPlayer(Vector3 position, int playerID)
    {
        Debug.Log("Spawn Player : " + playerID);
        PhotonNetwork.Instantiate(playerPrefabs[playerID].name, position, Quaternion.identity, 0);
    }
    public void SpawnMovingPlatform()
    {
        PhotonNetwork.Instantiate(movingPlatform.name, movingPlatformSpawnPoint.position, Quaternion.identity);
    }

    // Staring Games
    public void StartGames()
    {
        GameIsRolling = true;
    }

    // Convert float second to 00:00 time
    public static void ChangeTimeUI(float theTime, Text theText)
    {
        if (theTime <= 0)
        {
            theText.text = "00:00";
        }
        else
        {
            float minutes = Mathf.Floor(theTime / 60);
            float seconds = Mathf.RoundToInt(theTime % 60);
            string min, sec;
            if (minutes < 10)
            {
                min = "0" + minutes.ToString();
            }
            else
            {
                min = minutes.ToString();
            }
            if (seconds < 10)
            {
                sec = "0" + Mathf.RoundToInt(seconds).ToString();
            }
            else
            {
                sec = Mathf.RoundToInt(seconds).ToString();
            }

            theText.text = min + ":" + sec;
        }
    }

    // Game Over Method
    public void GameOverOnline()
    {
        // Debug
        Debug.Log("Game is over");

        // Stop Game
        photonView.RPC("StopGame", RpcTarget.All);

        // Call every player to send ther data to database
        RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(SEND_DATA_EVENT, null, eventOptions, SendOptions.SendReliable);       

        // Open Gameover Panel
        photonView.RPC("OpenGameOverPanel", RpcTarget.All);
    }
    [PunRPC]
    public void StopGame()
    {
        // Stop the game
        GameIsRolling = false;
    }
    [PunRPC]
    public void OpenGameOverPanel()
    {
        // Find all player
        PlayerManager[] playersOrder = FindObjectsOfType<PlayerManager>();
        // Sort based on king time
        for (int i = 0; i < playersOrder.Length; i++)
        {
            var m = i;

            for (int j = i + 1; j < playersOrder.Length; j++)
            {
                if (playersOrder[i].kingTime < playersOrder[j].kingTime)
                {
                    m = j;
                }
            }

            if (m != i)
            {
                var temp = playersOrder[m];
                playersOrder[m] = playersOrder[i];
                playersOrder[i] = temp;
            }
        }

        // Open panel
        menuPanel.SetActive(false);
        gameOverPanel.SetActive(true);

        // Set value
        GameOverPanel panel = FindObjectOfType<GameOverPanel>();
        for(int i = 0; i < panel.playerName.Length; i++)
        {
            if(i < playersOrder.Length)
            {
                panel.playerName[i].text = (i + 1).ToString() + ". " + playersOrder[i].photonView.Owner.NickName;
                panel.playerKingTime[i].text = playersOrder[i].kingTime.ToString("F0");
            }
            else
            {
                panel.playerName[i].text = "";
                panel.playerKingTime[i].text = "";
            }
        }
    }

    public bool AmIWin(string name)
    {
        // Find all player
        PlayerManager[] playersOrder = FindObjectsOfType<PlayerManager>();
        // Sort based on king time
        for (int i = 0; i < playersOrder.Length; i++)
        {
            var m = i;

            for (int j = i + 1; j < playersOrder.Length; j++)
            {
                if (playersOrder[i].kingTime < playersOrder[j].kingTime)
                {
                    m = j;
                }
            }

            if (m != i)
            {
                var temp = playersOrder[m];
                playersOrder[m] = playersOrder[i];
                playersOrder[i] = temp;
            }
        }

        if (playersOrder[0].photonView.Owner.NickName == name)
        {
            
            return true;
        }
        else
        {
            return false;
        }
    }

    // Return to play menu
    public void ReturnToPlayMenu()
    {
        SceneManager.LoadScene("Scene-0_PlayMenu");
    }

    // Database
    public void SendDatabase()
    {
        dataBaseObject.SetActive(true);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
