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
    public int PlayerUID { get; set; }

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
    [SerializeField] private GameObject myCrown;

    // Player UI
    PlayerUIGroub uIGroub;
    [SerializeField] private Text nameText;

    // Just in case leave room
    private bool isDestroy;

    // Animation
    Animator animator;

    // Canvas
    [SerializeField] private Canvas myCanvas;

    void Start()
    {
        onGround = true;
        manager = FindObjectOfType<GameManager>();

        // Set data
        kingTime = 0;

        if (photonView.IsMine)
        {
            PlayerUID = 0;
            GameManager.PlayerUIisSelected[0] = true;
        }
        else
        {
            for(int i = 0; i < 3; i++)
            {
                if(!GameManager.PlayerUIisSelected[i + 1])
                {
                    PlayerUID = i + 1;
                    GameManager.PlayerUIisSelected[i + 1] = true;
                }
            }
        }

        // Set UI
        Debug.Log(photonView.Owner.NickName + " " + (PlayerUID).ToString());
        GameObject uiTemp = manager.PlayersUI[PlayerUID];
        uiTemp.SetActive(true);
        uIGroub = uiTemp.GetComponent<PlayerUIGroub>();
        uIGroub.playerImage.sprite = manager.avatar[manager.selectedAvatar];
        uIGroub.playerNameText.text = photonView.Owner.NickName;
        uIGroub.kingLogo.SetActive(false);
        uIGroub.kingProgressText.text = "0 %";
        uIGroub.kingProgressSlider.value = 0;

        gameObject.GetComponentInChildren<Canvas>().worldCamera = FindObjectOfType<Camera>();
        nameText.text = photonView.Owner.NickName;

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

            // Sync data
            StartCoroutine(SyncUI());
        }

        animator = GetComponent<Animator>();

        transform.position = new Vector3(transform.position.x, transform.position.y, -2);
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
                    myCanvas.GetComponent<RectTransform>().localScale = new Vector3(-.001f, .001f, .001f);
                }
                else if (joystick.Horizontal > 0 && !isFacingRight)
                {
                    isFacingRight = true;
                    transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                    myCanvas.GetComponent<RectTransform>().localScale = new Vector3(.001f, .001f, .001f);
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
                        myCanvas.GetComponent<RectTransform>().localScale = new Vector3(-.001f, .001f, .001f);
                    }
                }
                else if (rightButton.isPressed)
                {
                    rigidbody.velocity = new Vector2(1 * moveSpeed, rigidbody.velocity.y);

                    if (!isFacingRight)
                    {
                        isFacingRight = true;
                        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                        myCanvas.GetComponent<RectTransform>().localScale = new Vector3(.001f, .001f, .001f);
                    }
                }
            }
            animator.SetFloat("Speed", Mathf.Abs(rigidbody.velocity.x));
            if(Mathf.Abs(rigidbody.velocity.x) > .1f)
            {
                photonView.RPC("Play", RpcTarget.All, "Walk");
            }
            else
            {
                photonView.RPC("Stop", RpcTarget.All, "Walk");
            }

            // Player Jump
            if (!isJump && jumpButton.isPressed)
            {
                animator.SetTrigger("Jump");
                photonView.RPC("Play", RpcTarget.All, "Jump");

                isJump = true;
                onGround = false;
                rigidbody.AddForce(new Vector2(0, jumpForce));
            }
            // Player Hit
            if (hitButton.isPressed)
            {
                HitPlayer();
            }

            // King time
            if (IsKing)
            {
                kingTime += Time.deltaTime;
            }
        }

        if (IsKing)
        {
            myCrown.SetActive(true);
        }
        else
        {
            myCrown.SetActive(false);
        }
            
        photonView.RPC("SetUI", RpcTarget.All);
        
        // Check king value
        if(kingTime >= MaxKingTime && GameManager.GameIsRolling && !isDestroy)
        {
            // End games
            manager.GameOverOnline();
        }
    }

    [PunRPC]
    public void SetUI()
    {
        // Set UI
        if (IsKing && GameManager.GameIsRolling)
        {
            uIGroub.kingLogo.SetActive(true);
        }
        else if (!IsKing)
        {
            uIGroub.kingLogo.SetActive(false);
        }
        uIGroub.kingProgressSlider.value = kingTime / MaxKingTime;
        uIGroub.kingProgressText.text = (kingTime / MaxKingTime * 100).ToString("F0") + " %";
    }
    public IEnumerator SyncUI()
    {
        while (true)
        {
            if (GameManager.GameIsRolling)
            {
                photonView.RPC("SyncUIOnline", RpcTarget.All, kingTime);
            }

            yield return new WaitForSeconds(.5f);
        }
    }
    [PunRPC]
    public void SyncUIOnline(float value)
    {
        kingTime = value;
    }

    // Hit Player
    public void HitPlayer()
    {
        if (!hitIsCooldown)
        {
            animator.SetTrigger("Hit");
            photonView.RPC("Play", RpcTarget.All, "Hit");

            hitIsCooldown = true;
            hitCountdown = hitCooldownTime;

            // Hit player here
            Collider2D[] targetHit = Physics2D.OverlapCircleAll(hitPos.position, hitRange, playerLayerMask);
            for (int i = 0; i < targetHit.Length; i++)
            {
                Debug.Log("Hit Target : " + targetHit[i].name);
                if (targetHit[i].attachedRigidbody != rigidbody)
                {
                    // Push them
                    targetHit[i].gameObject.GetComponent<PlayerManager>().photonView.RPC("PushPlayer", RpcTarget.All, isFacingRight);
                    targetHit[i].gameObject.GetComponent<PlayerManager>().photonView.RPC("GetHit", RpcTarget.All);

                    // Take the King
                    if (targetHit[i].GetComponent<PlayerManager>().IsKing)
                    {
                        targetHit[i].gameObject.GetComponent<PlayerManager>().photonView.RPC("ResetFromKing", RpcTarget.All);
                        photonView.RPC("SetToKing", RpcTarget.All);
                    }
                }
            }
            // Start cooldown
            StartCoroutine(HitPlayerCooldown());
        }
    }
    private IEnumerator HitPlayerCooldown()
    {
        Debug.Log("Hit cooldown");
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

    [PunRPC]
    public void PushPlayer(bool isRight)
    {
        if (isRight)
        {
            rigidbody.AddForce(new Vector2(hitForce, 0));
        }
        else
        {
            rigidbody.AddForce(new Vector2(-hitForce, 0));
        }
    }
    [PunRPC]
    public void GetHit()
    {
        animator.SetTrigger("Ketindih");
        getHit = true;
        StartCoroutine(GetHitCooldown());
    }
    private IEnumerator GetHitCooldown()
    {
        yield return new WaitForSeconds(.5f);
        getHit = false;
    }

    // King method
    [PunRPC]
    public void SetToKing()
    {
        IsKing = true;
    }
    [PunRPC]
    public void ResetFromKing()
    {
        IsKing = false;
    }

    // Detect Collision -------------------------------------------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Crown")
        {
            collision.GetComponent<CrownManager>().photonView.RPC("DiactivateObject", RpcTarget.All);
            photonView.RPC("SetToKing", RpcTarget.All);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Only call this if this player is local
        if (collision.collider.tag == "PlayerHead" && collision.gameObject.GetComponentInParent<PlayerManager>().IsKing)
        {
            collision.gameObject.GetComponent<PlayerManager>().photonView.RPC("GetHit", RpcTarget.All);

            // Take the King
            collision.gameObject.GetComponentInParent<PlayerManager>().photonView.RPC("ResetFromKing", RpcTarget.All);
            photonView.RPC("SetToKing", RpcTarget.All);
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
        Debug.Log("New Event");

        if (photonEvent.Code == GameManager.SEND_DATA_EVENT && photonView.Owner.IsLocal)
        {
            // Build the data
            AfterMatchData.playerName = PlayerPrefs.GetString(Constant.KEY_NAME);
            AfterMatchData.kingTime = kingTime;
            if (manager.AmIWin(photonView.Owner.NickName))
            {
                AfterMatchData.win = 1; Debug.Log("Win");
                AfterMatchData.lose = 0;
            }
            else
            {
                AfterMatchData.win = 0; Debug.Log("Lose");
                AfterMatchData.lose = 1;
            }
            AfterMatchData.match = 1;

            // Send the data
            manager.SendDatabase();
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
