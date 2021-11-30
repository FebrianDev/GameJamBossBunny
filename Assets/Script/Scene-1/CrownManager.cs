using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CrownManager : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        MovingPlatform movingPlatform = FindObjectOfType<MovingPlatform>();
        transform.parent = movingPlatform.transform;
        transform.position = new Vector2(movingPlatform.transform.position.x, movingPlatform.transform.position.y + 1);
    }

    [PunRPC]
    public void DiactivateObject()
    {
        Debug.Log("Destroyed");
        gameObject.SetActive(false);
    }
}
