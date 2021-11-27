using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    // Player Movement
    Joystick joystick;
    Button jumpButton, hitButton, leftButton, rightButton;
    Image hitButtonLoading;
    public static bool isJoystick;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    public bool isJump { get; private set; }
    private bool isFacingRight = true;

    // This player Rigid Body
    [SerializeField] private Rigidbody2D rigidbody;

    // Manager
    GameManager manager;

    // Hit Method
    [SerializeField] private Transform hitPos;
    private float hitRange = .5f;
    [SerializeField] private LayerMask playerLayerMask;
    private float hitForce = 2000;
    private float hitCooldownTime = 1.5f;
    private float hitCountdown;
    private bool hitIsCooldown;

    // King Mechanic
    private float MaxKingTime = 60f;
    public bool IsKing { get; set; }
    public float kingTime { get; private set; }

    void Start()
    {
        manager = FindObjectOfType<GameManager>();

        if (photonView.IsMine)
        {
            // Set all Button
            joystick = FindObjectOfType<Joystick>();
            jumpButton = manager.JumpButton.GetComponent<Button>();
            hitButton = manager.HitButton.GetComponent<Button>();
            leftButton = manager.MoveLeftButton.GetComponent<Button>();
            rightButton = manager.MoveRightButton.GetComponent<Button>();

            // UI
            hitButtonLoading = manager.HitButtonLoading;
            hitButtonLoading.fillAmount = 0;

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
            // Player Movement
            if (isJoystick)
            {
                rigidbody.velocity = new Vector2(joystick.Horizontal * moveSpeed, rigidbody.velocity.y);
                if(joystick.Horizontal < 0 && isFacingRight)
                {
                    isFacingRight = false;
                    transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                }
                else if (joystick.Horizontal > 0 && !isFacingRight)
                {
                    isFacingRight = true;
                    transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                }
            }
            else
            {
                if (leftButton.isPressed)
                {
                    rigidbody.velocity = new Vector2(-1 * moveSpeed, rigidbody.velocity.y);

                    if (isFacingRight)
                    {
                        isFacingRight = false;
                        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                    }
                }
                else if (rightButton.isPressed)
                {
                    rigidbody.velocity = new Vector2(1 * moveSpeed, rigidbody.velocity.y);

                    if (!isFacingRight)
                    {
                        isFacingRight = true;
                        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                    }
                }
            }
            // Player Jump
            if (!isJump && jumpButton.isPressed)
            {
                isJump = true;
                rigidbody.AddForce(new Vector2(0, jumpForce));
            }
            // Player Hit
            if (hitButton.isPressed)
            {
                HitPlayer();
            }
        }
        
    }

    // Hit Player
    public void HitPlayer()
    {
        if (!hitIsCooldown)
        {
            hitIsCooldown = true;
            hitCountdown = hitCooldownTime;

            // Hit player here
            Collider2D[] targetHit = Physics2D.OverlapCircleAll(hitPos.position, hitRange, playerLayerMask);
            foreach(Collider2D target in targetHit)
            {
                if(target.attachedRigidbody != rigidbody)
                {
                    target.GetComponent<Rigidbody2D>().AddForce(new Vector2(hitForce, 0));
                }
            }

            // Start cooldown
            StartCoroutine(HitPlayerCooldown());
        }
    }
    private IEnumerator HitPlayerCooldown()
    {
        while(hitCountdown > 0)
        {
            yield return new WaitForFixedUpdate();

            hitButtonLoading.fillAmount = (hitCountdown / hitCooldownTime);

            hitCountdown -= Time.deltaTime;
            if (hitCountdown <= 0)
            {
                hitCountdown -= 1;
                hitIsCooldown = false;
            }
        }   
    }
    

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
