using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private float speed = 1f;

    private void Start()
    {
        transform.localScale = new Vector3(1.2f, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.GameIsRolling)
        {
            transform.position = new Vector2(transform.position.x + speed * Time.deltaTime, transform.position.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "ArenaWall")
        {
            speed *= -1;
        }
    }
}
