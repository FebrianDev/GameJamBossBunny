using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    // Player Movement
    Joystick joystick;
    Button jumpButton, leftButton, rightButton;
    public static bool isJoystick;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    public bool isJump { get; private set; }

    // This player Rigid Body
    [SerializeField] private Rigidbody2D rigidbody;

    // Manager
    GameManager manager;
    
    void Start()
    {
        manager = FindObjectOfType<GameManager>();

        if (photonView.IsMine)
        {
            // Set all Button
            joystick = FindObjectOfType<Joystick>();
            jumpButton = manager.JumpButton.GetComponent<Button>();
            leftButton = manager.MoveLeftButton.GetComponent<Button>();
            rightButton = manager.MoveRightButton.GetComponent<Button>();

            // Deactivate joystick or button
            if (isJoystick)
            {
                leftButton.gameObject.SetActive(false);
                rightButton.gameObject.SetActive(false);
            }
            else 
            {
                joystick.gameObject.SetActive(false);
            }
        }       
    }
    
    void Update()
    {
        if (photonView.IsMine)
        {
            if (isJoystick)
            {
                rigidbody.velocity = new Vector2(joystick.Horizontal * moveSpeed, rigidbody.velocity.y);
            }
            else
            {
                if (leftButton.isPressed)
                {
                    rigidbody.velocity = new Vector2(-1 * moveSpeed, rigidbody.velocity.y);
                }
                else if (rightButton.isPressed)
                {
                    rigidbody.velocity = new Vector2(1 * moveSpeed, rigidbody.velocity.y);
                }
            }
            if (!isJump && jumpButton.isPressed)
            {
                isJump = true;
                rigidbody.AddForce(new Vector2(0, jumpForce));
            }
        }
        
    }

    // Button Control ---------------------------------------------------------------------------------------------

    

    // Detect Collision -------------------------------------------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Only call this if this player is local
        if (collision.collider.tag == "PlayerHead")
        {
            Debug.Log("Player Kill");
        }
        else if (collision.collider.tag == "Platform")
        {
            isJump = false;
        }
    }
}
