using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    // Panels
    [SerializeField] private GameObject StartPanel;

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
    public float PlayTimeLimit { get; private set; }
    public float playTimeCooldown { get; private set; }

    void Start()
    {
        // Open start panel
        StartPanel.SetActive(true);

        // Setup
        PlayTimeLimit = 180f;
        playTimeCooldown = PlayTimeLimit;
    }

    
    void Update()
    {
        
    }

    // Spawning Player
    public void SpawnPlayer()
    {
        SpawnPlayer(new Vector3(0, 0, 0));
    }
    public void SpawnPlayer(Vector3 position)
    {
        PhotonNetwork.Instantiate(playerPrefab.name, position, Quaternion.identity, 0);
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
