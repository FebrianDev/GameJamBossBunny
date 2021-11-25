using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    // Panels
    [SerializeField] private GameObject StartPanel;

    // Buttons
    public GameObject JumpButton;
    public GameObject MoveLeftButton;
    public GameObject MoveRightButton;

    // Prefabs
    [SerializeField] private GameObject playerPrefab;

    void Start()
    {
        // Open start panel
        StartPanel.SetActive(true);

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
}
