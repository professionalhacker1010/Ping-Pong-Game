using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleControls : MonoBehaviour
{
    //for movement
    [SerializeField] private float normalSpeed;
    [SerializeField] private float hardHitSpeed;
    [SerializeField] private int hardHitMaxFrames;
    [SerializeField] private Rigidbody2D rb;
    private bool movementHalted;

    //for messaging other scripts about movement and hits
    private bool lockedInputs = false, /*playerHasHit = false, playerHasHardHit = false,*/ playerIsHoldingSpace = false;
    private int locks = 0;
    private float holdSpaceTime;
    private int holdSpaceFrames = 0;

    //custom edits for different opponents lol
    public static bool hardHitActivated = false;
    public bool invertArrowKeys = false;
    public bool multiBall = false;
    [HideInInspector] public List<MultiBall> multiBalls; //for working around the locked inputs thing?

    //for hitting the ball
    [SerializeField] private Pingpong pingpong;
 

    //for indicating hard hit
    [SerializeField] private HardHitIndicator hardHitIndicator;

    //player swipe animations
    [SerializeField] private Animator swipeAnimation;
    private Animator paddlePreviewAnimation;

    void Start()
    {
        holdSpaceTime = Time.time;
        paddlePreviewAnimation = GetComponent<Animator>();
    }

    void Update()
    {
        //move paddle with keyboard
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (invertArrowKeys)
        {
            horizontalInput *= -1;
            verticalInput *= -1;
        }

        if (!movementHalted)
        {
            //move paddle at different speeds depending on whether they are attempting to hard hit or not
            if (hardHitActivated && playerIsHoldingSpace) MovePaddle(horizontalInput, verticalInput, hardHitSpeed);
            else MovePaddle(horizontalInput, verticalInput, normalSpeed);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }

        //hit ball with spacebar
        if (KeyCodes.Hit() && !playerIsHoldingSpace)
        {
            //Debug.Log("inputs locked: " + lockedInputs.ToString() + " locks: " + locks.ToString());
            playerIsHoldingSpace = true;
        }

        if (!PauseMenu.gameIsPaused)
        {
            if (multiBall && KeyCodes.Hit())
            {
                //check which ball is interactable, hit the first one and exit loop
                foreach (var ball in multiBalls)
                {
                    if (ball.thisBallInteractable)
                    {
                        //print("hit interactable");
                        ball.PlayerHit();
                        return;
                    }
                }
            }
            else if (!lockedInputs)
            {
                if (hardHitActivated)
                {
                    HardHitBehaviour();
                }
                else if (KeyCodes.Hit())
                {
                    Debug.Log("normal hit");
                    //StartCoroutine(SetBoolTrue(true));
                    pingpong.PlayerHit();
                    StartCoroutine(WaitForHitAnimation()); //lock inputs until animation is done - prevents player from spamming space
                }
            }

        }
    }

    private void HardHitBehaviour()
    {
        if (KeyCodes.HitGetUp() || holdSpaceFrames >= hardHitMaxFrames)
        {
            //reset any hard hit stuff
            //Debug.Log("Hard hit reset after frames: " + holdSpaceFrames.ToString());
            //StartCoroutine(SetBoolTrue(false));

            if (holdSpaceFrames >= 1) pingpong.ResumeGame();

            holdSpaceFrames = 0;
            playerIsHoldingSpace = false;
            hardHitIndicator.StopAllCoroutines();
            hardHitIndicator.FadeToOpaque();
            pingpong.PlayerHit();
            StartCoroutine(WaitForHitAnimation());
        }
        //hard hit: only for SPEEDY oppponent. fade to black lasts 24 frames
        else if (KeyCodes.HitHoldDown() && (Time.time - holdSpaceTime) > (1 / 24f) && playerIsHoldingSpace)
        {
            holdSpaceTime = Time.time;
            holdSpaceFrames++;
            if (holdSpaceFrames == 1)
            {
                pingpong.PauseGame();
                hardHitIndicator.FadeToBlack(hardHitMaxFrames);
            }
        }
    }

    private void MovePaddle(float horizontalInput, float verticalInput, float speed)
    {
        // velocity calculation method to prevent bugginess while running into walls
        rb.velocity = new Vector2(horizontalInput * speed, verticalInput * speed);
    }

    public void LockInputs()
    {
        //Debug.Log("lock inputs");
        lockedInputs = true;
        locks++;
        //Debug.Log("locks " + locks.ToString());
    }

    public void UnlockInputs()
    {
        //Debug.Log("unlock inputs");
        locks--;
        if (locks == 0)
            lockedInputs = false;// Debug.Log("inputs unlocked");
    }

    //hit animations
    #region
    public void HitRight()
    {
        swipeAnimation.SetTrigger("hitRight");
        //StartCoroutine(HidePaddlePreview());
    }

    public void HitLeft()
    {
        swipeAnimation.SetTrigger("hitLeft");
        //StartCoroutine(HidePaddlePreview());
    }

    private IEnumerator HidePaddlePreview()
    {
        yield return new WaitForSeconds(7 / 24f);
        paddlePreviewAnimation.SetTrigger("hitDone");
    }

    private IEnumerator WaitForHitAnimation()
    {
        //Debug.Log("wait for hit");
        LockInputs();
        yield return new WaitForSeconds(5/24f); //swipe animation lasts 5 frames
        UnlockInputs();
        //Debug.Log("hit done");
    }
    #endregion

    //signalling to other scripts for player hits and hard hits
    #region
    /*private IEnumerator SetBoolTrue(bool x)
    {
        if (x) playerHasHit = true;
        else playerHasHardHit = true;
        yield return new WaitForSeconds(0.1f);
        if (x) playerHasHit = false;
        else playerHasHardHit = false;
    }

    public bool PlayerHasHit() //put this in the update of any script that does something once the player has hit
    {
        return playerHasHit;
    }
    
    public bool PlayerHasHardHit()
    {
        return playerHasHardHit;
    }

    public bool PlayerIsHoldingSpace()
    {
        return playerIsHoldingSpace;
    }*/
    #endregion
}
