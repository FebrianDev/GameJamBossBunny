using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoingBoing : MonoBehaviour
{
    [SerializeField] private float boingForce;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            collision.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, boingForce));
        }   
    }
}
