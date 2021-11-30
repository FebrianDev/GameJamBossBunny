using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CrownManager : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        transform.localScale = new Vector3(.2f, .2f, 1);
    }

    [PunRPC]
    public void DiactivateObject()
    {
        Debug.Log("Destroyed");
        gameObject.SetActive(false);
    }
}
