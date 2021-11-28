using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

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
    public bool onGround { get; private set; }
    private bool isFacingRight = true;

    // This player Rigid Body
    [SerializeField] private Rigidbody2D rigidbody;

    // Manager
    GameManager manager;

    // ID
    public int ID { get; set; }

    // Hit Method
    [SerializeField] private Transform hitPos;
    private float hitRange = .5f;
    [SerializeField] private LayerMask playerLayerMask;
    private float hitForce = 3000;
    private bool getHit;
    private float hitCooldownTime = 1.5f;
    private float hitCountdown;
    private bool hitIsCooldown;

    // King Mechanic
    private float MaxKingTime = 2f;
    public bool IsKing { get; private set; }
    public float kingTime { get; private set; }

    // Player UI
    PlayerUIGroub uIGroub;

    void Start()
    {
        onGround = true;
        manager = FindObjectOfType<GameManager>();

        // Set data
        kingTime = 0;

        // Set UI
        GameObject uiTemp = manager.PlayersUI[ID - 1];
        uiTemp.SetActive(true);
        uIGroub = uiTemp.GetComponent<PlayerUIGroub>();
        uIGroub.playerNameText.text = photonView.Owner.NickName;
        uIGroub.kingLogo.SetActive(false);
        uIGroub.kingProgressText.text = "0 %";
        uIGroub.kingProgressSlider.value = 0;

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
        // Only local player
        if (photonView.IsMine && !getHit)
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
                onGround = false;
                rigidbody.AddForce(new Vector2(0, jumpForce));
            }
            // Player Hit
            if (hitButton.isPressed)
            {
                HitPlayerOnline();
            }
        }

        // Set UI
        if (IsKing && GameManager.GameIsRolling)
        {
            uIGroub.kingLogo.SetActive(true);
            kingTime += Time.deltaTime;
        }
        else if (!IsKing)
        {
            uIGroub.kingLogo.SetActive(false);
        }
        uIGroub.kingProgressSlider.value = kingTime / MaxKingTime;
        uIGroub.kingProgressText.text = (kingTime / MaxKingTime * 100).ToString("F0") + " %";
        
        // Check king value
        if(kingTime >= MaxKingTime && GameManager.GameIsRolling)
        {
            // End games
            manager.GameOverOnline();
        }
    }

    // Hit Player
    public void HitPlayerOnline()
    {
        photonView.RPC("HitPlayer", RpcTarget.All);
    }
    [PunRPC]
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
                    // Push them
                    target.GetComponent<Rigidbody2D>().AddForce(new Vector2(hitForce, 0));
                    target.GetComponent<PlayerManager>().GetHit();

                    // Take the King
                    target.GetComponent<PlayerManager>().ResetFromKing();
                    SetToKing();
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
    public void GetHit()
    {
        getHit = true;
        StartCoroutine(GetHitCooldown());
    }
    private IEnumerator GetHitCooldown()
    {
        yield return new WaitForSeconds(.5f);
        getHit = false;
    }
    
    // King method
    public void SetToKingOnline()
    {
        photonView.RPC("SetToKing", RpcTarget.All);
    }
    [PunRPC]
    public void SetToKing()
    {
        IsKing = true;
    }
    public void ResetFromKingOnline()
    {
        photonView.RPC("ResetFromKing", RpcTarget.All);
    }
    [PunRPC]
    public void ResetFromKing()
    {
        IsKing = false;
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
            // Take the King
            collision.collider.GetComponent<PlayerManager>().ResetFromKingOnline();
            SetToKingOnline();
        }
        else if (collision.collider.tag == "Platform")
        {
            isJump = false;
            onGround = true;
        }
    }

    // Database ---------------------------------------------------------------------------------------------------
    public void NetworkingClient_EventReceived(EventData photonEvent)
    {
        if (photonEvent.Code == GameManager.SEND_DATA_EVENT && photonView.Owner.IsLocal)
        {
            // Build the data
            AfterMatchData afterMatch = new AfterMatchData();
            afterMatch.playerName = photonView.Owner.NickName;
            afterMatch.kingTime = kingTime;
            if (manager.AmIWin(photonView.Owner.NickName))
            {
                afterMatch.win = 1;
                afterMatch.lose = 0;
            }
            else
            {
                afterMatch.win = 0;
                afterMatch.lose = 1;
            }
            afterMatch.match = 1;

            // Send the data
            GameManager.afMData = afterMatch;
            manager.SendDatabase();

            // Debug
            Debug.Log("Data Sended");
        }
    }
    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
    }
    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
    }
}
