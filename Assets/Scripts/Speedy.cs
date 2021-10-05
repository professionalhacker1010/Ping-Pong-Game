using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speedy : Opponent
{
    [SerializeField] private Pingpong pingPong;
    [SerializeField] private Animator ballAnimation;
    [SerializeField] private PaddleControls paddleControls;
    [SerializeField] private List<float> ballSpeeds;
    [SerializeField] private float hitOutDistance;
    [SerializeField] private Vector3 leftOut;
    [SerializeField] private Vector3 rightOut;
    //private int rounds = 0;
    private float prevX;
    //private bool left = false;
    //private bool right = false;

    private void Start()
    {
        prevX = servePosition.x;
        StartCoroutine(SpeedyServe());
        PaddleControls.hardHitActivated = true;
    }

    //speedy hits out if you hit the ball really far apart, one end then the other
    //player can tell because their hits mirror yours at a slightly wider angle ALWAYS.
    public override Vector3 GetOpponentBallPath(float X, float Y, bool isServing)
    {
        Debug.Log((X - prevX).ToString());

        //hit out and play lose animation when player hits wide enough
        if (X - prevX >= hitOutDistance)
        {
            return leftOut; //player hit wide to the right -> opponent hits wide to the left
        }
        else if (X - prevX <= -1 * hitOutDistance)
        {
            return rightOut; //player hit wide to the left -> opponent hits wide to the right
        }
        prevX = X;

        return new Vector3(X * -1.25f, Y);
    }

    public override void ChangeOpponentPosition(float startX, float startY, Vector3 end, int hitFrame)
    {
        StartCoroutine(ChangeSpeedyPosition(startX, hitFrame));
    }

    private IEnumerator ChangeSpeedyPosition(float startX, float hitFrame)
    {
        float speed = ballSpeeds[GameManager.Instance.playerWins];
        //float fps = 24f * speed;
        float frames = hitFrame / speed;

        //wait 6 frames
        yield return new WaitForSeconds((frames * 0.5f)/ 24f);

        //split remaining time into 3 chunks
        //float timeChunk = (((hitFrame - 6 - oppHitFrame))/ (fps * 3f));
        float framesPerIncrement = (((frames * 0.5f) - (oppHitFrame / speed)) / 4f);

        //split X movement into 3 chunks
        float moveChunk = (startX - transform.position.x) / 4f;

        yield return Move(4, moveChunk, framesPerIncrement);

        //hit the ball
        animator.SetTrigger("hit");
        transform.position = new Vector3(startX, transform.position.y); //final move
        yield return new WaitForSeconds(oppHitFrame / 24f);

        //increase speed if needed (ie if player's score has increased)
        pingPong.ballSpeed = ballSpeeds[GameManager.Instance.playerWins];
    }

    public override IEnumerator PlayServeAnimation(float waitTime)
    {
        float speed = ballSpeeds[GameManager.Instance.playerWins];
        float fps = 24 * speed;

        yield return new WaitForSeconds(2f);

        //split X movement into 3 chunks
        float moveChunk = (servePosition.x - transform.position.x) / 3f;

        //move for half a second - scales faster each time player scores point
        yield return Move(4, moveChunk, 2);

        animator.SetTrigger("hit");
        yield return new WaitForSeconds(oppHitFrame / 24f);
    }

    public override IEnumerator PlayLoseRoundAnimation()
    {
        print("speedy lose round anim");
        animator.SetTrigger("energize");
        yield return new WaitForSeconds(1.5f);

        float moveChunk = (transform.position.x * -1) / 3;
        yield return Move(3, moveChunk, 3);
        animator.SetTrigger("idle");
    }

    //for starting the game
    private IEnumerator SpeedyServe()
    {
        paddleControls.LockInputs();
        pingPong.gameObject.SetActive(false);
        yield return new WaitForSeconds(2f); //wait for round number info to display
        pingPong.gameObject.SetActive(true);
        pingPong.ResetRound();
        pingPong.ballSpeed = ballSpeeds[GameManager.Instance.playerWins];
    }

    private IEnumerator Move(int increments, float moveChunk, float framesPerIncrement)
    {
        //float fps = 24 * ballSpeeds[GameManager.Instance.playerWins];

        animator.SetTrigger("move");
        for (int i = 0; i < increments; i++)
        {
            transform.position = new Vector3(transform.position.x + moveChunk, transform.position.y);
            yield return new WaitForSeconds(framesPerIncrement / 24f);
        }
    }
}
