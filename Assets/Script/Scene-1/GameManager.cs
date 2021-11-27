using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    // Panels
    [SerializeField] private GameObject StartPanel;

    // Players UI
    public GameObject[] PlayersUI;

    // Buttons
    public GameObject JumpButton;
    public GameObject HitButton;
    public GameObject MoveLeftButton;
    public GameObject MoveRightButton;

    // UI
    public Image HitButtonLoading;

    // Prefabs
    [SerializeField] private GameObject playerPrefab;

    // Timer
    public static bool GameIsRolling;
    public float PlayTimeLimit { get; private set; }
    public float playTimeCooldown { get; private set; }
    [SerializeField] private Text timerText;


    void Start()
    {
        // Open start panel
        StartPanel.SetActive(true);

        // Setup
        PlayTimeLimit = 180f;
        playTimeCooldown = PlayTimeLimit;
        ChangeTimeUI(playTimeCooldown, timerText);

        // Turn off all UI
        foreach(GameObject a in PlayersUI)
        {
            a.SetActive(false);
        }
    }

    
    void Update()
    {
        if (GameIsRolling && playTimeCooldown > 0)
        {
            playTimeCooldown -= Time.deltaTime;
            ChangeTimeUI(playTimeCooldown, timerText);
        }
        else if(playTimeCooldown <= 0)
        {
            // Time out

        }
    }

    // Spawning Player
    public void SpawnPlayer()
    {
        SpawnPlayer(new Vector3(0, 0, 0));
    }
    public void SpawnPlayer(Vector3 position)
    {
        GameObject temp = PhotonNetwork.Instantiate(playerPrefab.name, position, Quaternion.identity, 0);
        temp.GetComponent<PlayerManager>().ID = PhotonNetwork.CurrentRoom.PlayerCount;
    }

    // Set the first King
    public void SetFirstKing()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Find all players
            PlayerManager[] players = FindObjectsOfType<PlayerManager>();
            // Randomize
            int randKing = Random.Range(0, players.Length);
            // Set to king
            players[randKing].SetToKingOnline(); Debug.Log("King set to : " + randKing);
        }
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
}
