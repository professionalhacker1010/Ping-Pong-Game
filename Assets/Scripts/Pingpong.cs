using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pingpong : MonoBehaviour
{
    //variables
    #region
    [SerializeField] private PaddleControls paddleControls;

    //colliders
    [SerializeField] private CircleCollider2D pingpongCollider;
    [SerializeField] private CircleCollider2D paddleCollider;
    [SerializeField] private BoxCollider2D paddleCenter;

    //animations
    [SerializeField] public Animator ballAnimation;
    [SerializeField] public Animator shadow;
    [SerializeField] protected Animator explodeAnimation;
    [SerializeField] protected CameraShake cameraShaker;

    //player behaviors
    [SerializeField] private Player player;
    public Opponent opponent;
    protected BallPath currBallPath;

    //starting transform values - change opponent and start during game's runtime
    //paddle X, Y = distances from paddle center    opponent X, Y = ending X position and highest Y(relative to first position of next hit)
    //start X, Y = end of last hit, start of next
    [HideInInspector] public float paddleY, opponentY, startY;
    [HideInInspector] public float paddleX, opponentX, startX;

    //scaling factors. for factorPaddleY/X, enter desired max adjustment in inspector (as fraction of max), code will create true factor from that
    [SerializeField] private float factorPaddleY, factorPaddleX;

    //list of tableY-trueMaxY ranges
    [SerializeField] private List<float> trueMaxY;
    [SerializeField] private List<float> tableY;
    [SerializeField] private float normalizer;

    //final transform calculations - change during game's runtime
    private float maxY; //this is a raw maxY
    private float endX;
    [HideInInspector] public List<float> finalY;
    [HideInInspector] public List<float> finalX;

    //out of bounds values
    [SerializeField] private float edgeNet;
    [SerializeField] private PolygonCollider2D tableCollider;
    protected bool netBall = false, edgeBall = false;

    //window of frames player has to hit back
    [SerializeField] public int hitBackFrames, hitBackLeeway;
    private int frameCount = 0; //keeps track of the frames from the opponent's hit only
    public bool thisBallInteractable = false;

    //who's serving
    public bool playerServing = true;
    [SerializeField] private Vector3 playerServePosition;

    //ball speed - can be manipulated by opponent scripts in Start
    public float ballSpeed = 1.0f;
    #endregion

    protected virtual void Start()
    {
        //get current opponent
        
        opponent = LevelManager.Instance.GetChosenOpponent(); //get opponent info

        //normalize the heights...
        for (int i = 0; i < trueMaxY.Count; i++)
        {
            trueMaxY[i] += (normalizer);
            trueMaxY[i] -= 0.3f; //moved the table down 0.3 units
        }
        for (int i = 0; i < tableY.Count; i++) tableY[i] += normalizer;
        edgeNet += normalizer;

        //temporary opponent start transforms for now
        opponentY = normalizer;
        opponentX = 0.0f;
        startY = transform.position.y + normalizer;
        startX = transform.position.x;
        //creating true factors
        factorPaddleY = factorPaddleY * (trueMaxY[0] - tableY[0]) / paddleCollider.radius;
        factorPaddleX = factorPaddleX * 9.5f / paddleCollider.radius; //9.5 is Unity's distance from origin to sides of view

        //static animation based on whos serving
        if (playerServing) ballAnimation.SetTrigger("playerWaitServe");
        else ballAnimation.SetTrigger("opponentWaitServe");

    }

    public void PlayerHit()
    {
        if (!gameObject.activeInHierarchy) return;
        //Debug.Log("plauerhit");
        if (paddleCollider.IsTouching(pingpongCollider))
        {
            //set the ball speed
            ballAnimation.speed = ballSpeed;
            shadow.speed = ballSpeed;
            //print("opponent X: " + opponentX.ToString() + " Y: " + opponentY.ToString());
            //lock paddle inputs
            paddleControls.LockInputs();
            thisBallInteractable = false;

            //stop any coroutines
            StopAllCoroutines();

            //get player's ball path
            currBallPath = player.playerPath;
            CalcPlayerBallPath();

            //start paddle swipe animations
            if (paddleX >= 0) paddleControls.HitRight();
            else paddleControls.HitLeft();

            //start ball animations, updating positions while animation is playing with setballpath coroutine
            shadow.SetTrigger("hitBall");
            ballAnimation.SetTrigger("hitBall");
            bool playerLose = true;
            if (netBall)
            {
                Debug.Log("You hit a net ball!");
                SetBallPath(0, currBallPath.endFrame, playerLose);
            }
            else if (edgeBall)
            {
                Debug.Log("You hit out of bounds!");
                SetBallPath(0, currBallPath.endFrame, playerLose);
            }
            else {
                //Debug.Log("reached this block");
                playerLose = false;
                SetBallPath(0, currBallPath.endFrame, playerLose);
                StartCoroutine(MoveOpponent(false));
            }
        }
        else
        {
            Debug.Log("You missed the ball!");
            paddleControls.HitLeft();
        }
    }

    //Coroutines
    #region
    protected virtual void SetBallPath(int startFrame, int endFrame, bool playerLose)
    {
        StartCoroutine(SetBallPathHelper(startFrame, endFrame, playerLose));
    }
    private IEnumerator SetBallPathHelper(int startFrame, int endFrame, bool playerLose)
    {
        //print("Set ball path");
        while (startFrame < endFrame)
        {
            //translate the ball
            ballAnimation.transform.position = new Vector3(finalX[startFrame], finalY[startFrame]);
            shadow.transform.position = new Vector3(finalX[startFrame], shadow.transform.position.y);
            startFrame++;

            //if player hit out of bounds, explode the ball on the frame that it bounces on
            if (startFrame == currBallPath.bounceFrame && edgeBall)
            {
                //scaling size of explosion based on who hit
                if (playerLose) //then the opponent wins
                {
                    print("out of bounds explosion");
                    explodeAnimation.transform.localScale = new Vector3(0.6f, 0.6f);
                    ExplodeBall(false); //wait for explode ball animation to finish to restart
                }
                else //otherwise the player wins
                {
                    explodeAnimation.transform.localScale = new Vector3(0.75f, 0.75f);
                    ExplodeBall(true);
                }
                startFrame = endFrame;
            }
            else if (startFrame == (currBallPath.endFrame / 2) && netBall)
            {
                explodeAnimation.transform.localScale = new Vector3(0.85f, 0.85f);
                if (playerLose) ExplodeBall(false);
                else ExplodeBall(true);
            }
            //reset lose flags
            else if (startFrame == currBallPath.endFrame - hitBackFrames && !playerLose)
            {
                netBall = false;
                edgeBall = false;
            }

            yield return new WaitForSeconds(1f / (24f * ballSpeed));
        }
    }

    private IEnumerator MoveOpponent(bool ballOut) //move opponent and call calculation of opponent ball
    {
        //Debug.Log("move opponent");
        //this is called after player hits no matter what. Opponent class will take care of what animations to play and when
        Vector3 opponentBallEnd = opponent.GetOpponentBallPath(startX, startY - normalizer, false);
        opponent.ChangeOpponentPosition(startX, startY - normalizer, opponentBallEnd, currBallPath.endFrame);

        yield return new WaitForSeconds(currBallPath.endFrame / (24f * ballSpeed));
        
        bool opponentHasHit = true;
        CalcOpponentBallPath(opponentHasHit, opponentBallEnd);
    }

    private IEnumerator PlayerHitBackWindow(int startFrame, int endFrame)
    {
        frameCount = startFrame;
        while (frameCount < endFrame + hitBackLeeway)
        {
            if (frameCount == endFrame - hitBackFrames)
            {
                paddleControls.UnlockInputs();
                thisBallInteractable = true;
            }
            frameCount++;
            yield return new WaitForSeconds(1 / (24f * ballSpeed));
        }

        if (frameCount >= endFrame + hitBackLeeway)
        {
            paddleControls.LockInputs();
            thisBallInteractable = false;
            Debug.Log("You're too late!");
            ExplodeBall(false);
        }
    }

    public IEnumerator WaitForOpponentServe()
    {
        print("Waitforopponentserve");
        startX = opponent.servePosition.x;
        startY = opponent.servePosition.y + normalizer;
        ballAnimation.transform.position = opponent.servePosition;
        Vector3 opponentBallPath = opponent.GetOpponentBallPath(startX, startY-normalizer, true);
        yield return opponent.PlayServeAnimation(2.0f);

        CalcOpponentBallPath(true, opponentBallPath);
    }

    protected virtual void ExplodeBall(bool playerWin)
    {
        StartCoroutine(ExplodeBallHelper(playerWin));
    }

    private IEnumerator ExplodeBallHelper(bool playerWin)
    {
        //Debug.Log("Explode ball");
        explodeAnimation.transform.position = ballAnimation.transform.position;
        ballAnimation.SetTrigger("explodeBall");
        explodeAnimation.SetTrigger("explodeBall");
        shadow.SetTrigger("explodeBall");
        yield return new WaitForSeconds(0.25f);

        cameraShaker.ShakeCamera(0);
        yield return new WaitForSeconds(.75f);

        //this is after the ball explosion animation is finished
        if (playerWin)
        {
            GameManager.Instance.AddPlayerWin();
            if (GameManager.Instance.playerWins != GameManager.Instance.winRounds) StartCoroutine(opponent.PlayLoseRoundAnimation());
        }
        else
        {
            GameManager.Instance.AddOpponentWin();
        }
        ResetRound();
        explodeAnimation.transform.localScale = new Vector3(1.0f, 1.0f); //reset explosion scale
    }

    #endregion

    public virtual void CalcOpponentBallPath(bool opponentHasHit, Vector3 opponentBallPath)
    {
        //Debug.Log("calc opponent ball path");
        //temporarily reverse trueMaxY and tableY
        trueMaxY.Reverse(); tableY.Reverse();
        currBallPath = opponent.opponentPath;

        //get end and max from opponent variable
        endX = opponentBallPath.x;
        maxY = opponentBallPath.y + normalizer;

        //predetermined hits will input the end point of the ball - however the calcopponentballpath function take the Y as the max height, not the end point
        //so I'm fixing that here lol. AND ASSUMING THE SECOND BALL FALLS 75% OF MAX HEIGHT. THAT MIGHT CHANGE IN THE FUTURE.

        //LOW ball or HIGH ball?
        bool lowBall;
        if (((GetScaledY(0, tableY.Count - 1, maxY) - tableY[0]) * 4/3f)+tableY[0] <= startY)
        {
            lowBall = true;
            currBallPath.SwitchPath("low"); //JUST TESTING
            //Debug.Log("LOW BALL------------");
        }
        else
        {
            lowBall = false;
            currBallPath.SwitchPath("high");
            //Debug.Log("HIGH BALL------------");
        }

        //Debug.Log("Start: " + startY.ToString() + " Max: " + GetScaledY(0, tableY.Count - 1, (maxY * 4/3f)).ToString());

        //calculate ball heights for each frame
        finalY = new List<float>(currBallPath.endFrame);
        CalcAllYOpponent(currBallPath.highestFrame, currBallPath.endFrame, lowBall);

        //calculate X position for each frame
        finalX = new List<float>(currBallPath.endFrame);
        CalcAllX(currBallPath.endFrame);

        //opponent misses when Z = 2
        if (opponentBallPath.z == 2.0f)
        {
            Debug.Log("ExplodeBall called");
            explodeAnimation.transform.localScale = new Vector3(0.5f, 0.5f);
            ExplodeBall(true);
        }
        //opponent hits net ball when Z = -1
        else
        {
            //play ball animations
            shadow.SetTrigger("opponentHitBall");
            shadow.SetTrigger("opponentHitInBounds");
            ballAnimation.SetTrigger("opponentHitBall");
            //play hit flash animation
            opponent.HitFlash(startX, startY - normalizer);

            bool playerLose = false;
            if (opponentBallPath.z == -1.0f)
            {
                Debug.Log("Opponent hit net ball!");
                netBall = true;
                SetBallPath(0, currBallPath.endFrame, playerLose);
            }
            //opponent htis out of bounds when z = 1
            else if (opponentBallPath.z == 1.0f)
            {
                Debug.Log("Opponent hit out of bounds!");
                edgeBall = true;
                SetBallPath(0, currBallPath.endFrame, playerLose);
            }
            else
            {
                //print("normal hit");
                playerLose = true;
                SetBallPath(0, currBallPath.endFrame, playerLose);
                StartCoroutine(PlayerHitBackWindow(0, currBallPath.endFrame));
                //update for next hit
                UpdateStartAndOpponentXY();
                
            }
        }
        //revert trueMaxY and tableY
        trueMaxY.Reverse(); tableY.Reverse();
    }

    private void CalcPlayerBallPath()
    {
        //calculate max ball height
        paddleY = pingpongCollider.transform.position.y - (paddleCenter.transform.position.y + paddleCenter.offset.y); //where did ball hit paddle on Y?
        maxY = CalcMaxY();

        //LOW ball or HIGH ball?
        if (maxY <= startY)
        {
            maxY = startY;
            currBallPath.SwitchPath("low");
        }
        else
        {
            currBallPath.SwitchPath("high");
        }

        //calculate ball heights for each frame
        finalY = new List<float>(currBallPath.endFrame);
        CalcAllY(currBallPath.highestFrame, currBallPath.endFrame);

        //if ball height is less than net on frame where ball is directly over net
        if (finalY[12] <= edgeNet-normalizer)
        {
            netBall = true;
        }

        //calculate end X
        paddleX = pingpongCollider.transform.position.x - (paddleCenter.transform.position.x + paddleCenter.offset.x); //where did ball hit paddle on X?
        endX = CalcEndX();

        //calculate X position for each frame
        finalX = new List<float>(currBallPath.endFrame);
        CalcAllX(currBallPath.endFrame);

        //if ball bounces out of bounds on x2 frame
        if (!tableCollider.OverlapPoint(new Vector2(finalX[(int)currBallPath.bounceFrame], finalY[(int)currBallPath.bounceFrame])))
        {
            //Debug.Log("edgeball is true");
            edgeBall = true;
        }

        //update for next hit
        UpdateStartAndOpponentXY();
    }
    
    //Arc calculations
    #region
    private void CalcAllY(int highestFrame, int endFrame)
    {
        //assumes maxY and startY are RELATIVE TO FIRST FRAME
        //before reaching max height
        for (int i = 0; i < highestFrame; i++)
        {
            float range = GetScaledY(i, 0, maxY) - GetScaledY(i, 0, startY); //range from the starting height of hit to max height
            float height = (CalcArc1(i) * range) + GetScaledY(i, 0, startY); //scale the height to the arc, then add back starting height
            finalY.Add(height-normalizer); //subtract normalizer for final Y coordinate
        }
        //after max height, before bounce
        for (int i = highestFrame; i < currBallPath.bounceFrame; i++)
        {
            float range = GetScaledY(i, 0, maxY) - tableY[i]; //range from max height to table
            float height = (CalcArc1(i) * range) + tableY[i]; //scale the height to the arc, then add back table height
            finalY.Add(height-normalizer);
        }
        //after bounce
        for (int i = (int)currBallPath.bounceFrame; i < endFrame; i++)
        {
            float range = GetScaledY(i, 0, maxY) - tableY[i]; //range from max height to table
            float height = (CalcArc2(i) * range) + tableY[i];
            finalY.Add(height-normalizer);
        }
    }

    private void CalcAllYOpponent(int highestFrame, int endFrame, bool lowBall)
    {
        //Debug.Log("End Y before scaled: " + maxY.ToString());
        float endY = maxY;
        if (!lowBall) maxY = ((GetScaledY(0, tableY.Count - 1, maxY) - tableY[0]) * 4/3f) + tableY[0];
        else
        {
            maxY = startY;
            //endY = startY * .75f;
        }

        if (maxY > trueMaxY[0])
        {
            maxY = trueMaxY[0];
            endY = maxY * .75f;
        }

        //assumes maxY and startY are RELATIVE TO FIRST FRAME
        //before reaching max height
        for (int i = 0; i < highestFrame; i++)
        {
            float range = GetScaledY(i, 0, maxY) - GetScaledY(i, 0, startY); //range from the starting height of hit to max height
            float height = (CalcArc1(i) * range) + GetScaledY(i, 0, startY); //scale the height to the arc, then add back starting height
            finalY.Add(height - normalizer); //subtract normalizer for final Y coordinate
        }
        //after max height, before bounce
        for (int i = highestFrame; i < currBallPath.bounceFrame; i++)
        {
            float range = GetScaledY(i, 0, maxY) - tableY[i]; //range from max height to table
            float height = (CalcArc1(i) * range) + tableY[i]; //scale the height to the arc, then add back table height
            finalY.Add(height - normalizer);
        }
        //after bounce
        for (int i = (int)currBallPath.bounceFrame; i < endFrame; i++)
        {
            float range = GetScaledY(i, endFrame-1, endY) - tableY[i];
            float height = (CalcArc2(i) * range) + tableY[i];
            finalY.Add(height - normalizer);
        }
    }

    private void CalcAllX(int endFrame)
    {
        //just calculating a constant translation
        float increment = (endX - startX)/(endFrame);
        for (int i = 0; i < endFrame; i++)
        {
            finalX.Add(startX + (increment * i));
        }
    }
    private float CalcMaxY() // calculate max height of ball, relative to first position in animation
    {
        float max = opponentY + (paddleY * factorPaddleY); //GetScaledY(0, finalY.Count-1, finalY[finalY.Count-1]) 
        if (max <= trueMaxY[0]) return max;
        else return trueMaxY[0];
    }
    private float CalcEndX()
    {
        float end = opponentX + (paddleX * factorPaddleX);
        return end;
    }
    private float CalcArc1(int frame)
    {
        return currBallPath.a1 * (frame - currBallPath.x1) * (frame - currBallPath.x2);
    }
    private float CalcArc2(int frame)
    {
        return currBallPath.a2 * (frame - currBallPath.x2) * (frame - currBallPath.x3);
    }
    private float GetScaledY(int frame, int rawFrame, float raw)
    {
        //what is the % height of raw?
        float percentY = (raw - tableY[rawFrame]) / (trueMaxY[rawFrame] - tableY[rawFrame]);
        //apply that percent to desired frame
        float Y = (percentY * (trueMaxY[frame] - tableY[frame])) + tableY[frame];
        return Y;
    }
    #endregion

    private void UpdateStartAndOpponentXY() //updating variables for continuing rallies
    {
        //print("update start and opponent XY");
        //update start positions
        startX = finalX[finalX.Count - 1];
        startY = finalY[finalY.Count - 1] + normalizer;//GetScaledY(0, finalX.Count - 1, finalY[finalY.Count - 1]+normalizer);

        //update opponent X and Y values
        opponentX = startX; //todo: does X need scaling?
        opponentY = GetScaledY(finalY.Count - 1, 0, maxY);

        //update ball speed in case opponent changed it
        if (ballSpeed < 0) ballSpeed *= -1;
        ballAnimation.speed = ballSpeed;
        shadow.speed = ballSpeed;
    }

    //checks if anyone's won or lost the game, then triggers appropriate results. resets variables and determines server. aOnly called after someone's scored a point
    public void ResetRound()
    {
        //print("resetround");
        //reset variables
        startX = 0.0f;
        startY = normalizer;
        opponentY = normalizer;
        opponentX = 0.0f;
        netBall = false;
        edgeBall = false;

        //check if game over
        if (GameManager.Instance.GameIsLost())
        {
            Debug.Log("You Lose!");
            //trigger game lost stuff
            paddleControls.LockInputs();
            thisBallInteractable = false;
            GameManager.Instance.GameOver();
            ballAnimation.transform.position = new Vector3(0.0f, 0.0f);
            opponent.PlayWinAnimation();
        }
        else if (GameManager.Instance.GameIsWon())
        {
            Debug.Log("You Win!");
            //trigger game won stuff
            paddleControls.LockInputs();
            thisBallInteractable = false;
            GameManager.Instance.GameOver();
            ballAnimation.transform.position = new Vector3(0.0f, 0.0f);
            opponent.PlayLoseAnimation();
        }
        else
        {
            //switch server every round
            playerServing = !playerServing;
            if (!playerServing)
            {
                ballAnimation.SetTrigger("opponentWaitServe");
                StartCoroutine(WaitForOpponentServe());
            }
            else if (playerServing)
            {
                frameCount = finalY.Count;
                paddleControls.UnlockInputs();
                thisBallInteractable = true; //this should never happen tho
                ballAnimation.SetTrigger("playerWaitServe");
                ballAnimation.transform.position = playerServePosition;
            }
        
        }
    }

    public void PauseGame() //for speedy only lol
    {
        StopAllCoroutines();
        ballAnimation.speed = 0f;
        shadow.speed = 0f;
        //Debug.Log("game paused");
    }

    public void ResumeGame() //for speedy only
    {
        ballAnimation.speed = ballSpeed;
        shadow.speed = ballSpeed;
        SetBallPath(frameCount, currBallPath.endFrame, false);
        if (!paddleCollider.IsTouching(pingpongCollider)) ExplodeBall(false);

        //Debug.Log("game resumed");
    }
}
