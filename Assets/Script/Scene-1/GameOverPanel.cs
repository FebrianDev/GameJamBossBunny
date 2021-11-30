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

    public GameObject button;
    public GameObject title;

    private void Start()
    {
        button.SetActive(false);
        title.SetActive(true);
        StartCoroutine(RevealContinue());
    }

    private IEnumerator RevealContinue()
    {
        yield return new WaitForSeconds(5.5f);
        button.SetActive(true);
        title.SetActive(false);
    }
}
