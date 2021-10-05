using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//add this
using Yarn.Unity;

public class GameDev : Opponent
{
    private bool introPlayed = false;

    [Header("Base GameDev")]
    [SerializeField] private List<int> hitsPerGlitch; //should have 4 values
    [SerializeField] private Vector3 hitPlayerScore;
    [SerializeField] private BoxCollider2D playerScoreCollider, opponentScoreCollider;
    [SerializeField] private Vector2 minCollider, maxCollider;

    //keeping track of glitches
    private int hits = 0, currGlitch = 0; //0 = flickering, 1 = inversion, 2 = trail, 3 = window minimized
    private bool flicker = false, trail = false;

    //gameobjects
    [SerializeField] private GameObject ball, shadow, paddle, playerSwipes, playerScore, opponentScore, globalVolume, buttonMash, desktopScreen, glitchOutAnimation, opponentSprite;

    //intro
    [SerializeField] YarnProgram introDialogue;
    private Pingpong pingpong;

    //Post processing effects
    [SerializeField] private List<UnityEngine.Rendering.VolumeProfile> postProcessEffects;
    private UnityEngine.Rendering.Volume volume;

    //UI glitches
    [Header("Sprites")]
    [SerializeField] private SpriteRenderer BG; [SerializeField] private SpriteRenderer table;
    [SerializeField] private Animator playerPaddleAnimation;
    [SerializeField] private List<Sprite> pixellatedSprites;
    private bool playerHit = false;
    private int playedRounds = 0;

    //flickering
    [Header("Flicker")]
    [SerializeField] private int flickerMinFrequency;   [SerializeField] private int flickerMaxFrequency, flickerLength, minX, maxX, minY, maxY; //in 24fps frames

    //inversion
    private Camera cam;

    //trail
    [Header("Trail")]
    [SerializeField] private int trailTime;     [SerializeField] private int trailFrameInterval;

    //button mash
    private PaddleControls paddleControls;

    // Start is called before the first frame update
    void Start()
    {
        pingpong = ball.GetComponent<Pingpong>();
        volume = globalVolume.GetComponent<UnityEngine.Rendering.Volume>();
        cam = FindObjectOfType<Camera>();
        paddleControls = paddle.GetComponent<PaddleControls>();

        //switch out to more pixellated sprites and change layering
        ScoreDatabase.Instance.SwapToPixellated();
        BG.sprite = pixellatedSprites[0];
        table.sprite = pixellatedSprites[1];
        playerPaddleAnimation.SetTrigger("pixellated");

        GameManager.Instance.winRounds = 10;
    }

    private void Update()
    {
        int updatedRounds = GameManager.Instance.opponentWins + GameManager.Instance.playerWins;
        if (playedRounds < updatedRounds && introPlayed)
        {
            playedRounds = updatedRounds;
            if (playedRounds % 2 == 0) //player is serving
            {
                StartCoroutine(UIMoveWhilePlayerServe());
            }
        }
    }

    public override Vector3 GetOpponentBallPath(float X, float Y, bool isServing)
    {
        playerHit = true;
        if (!introPlayed)
        {
            Vector3 UIpos = new Vector3(X, Y);
            playerScore.transform.position = UIpos; //move player UI to wherever the ball is headed
            playerScoreCollider.gameObject.transform.position = UIpos;
            playerScore.transform.localScale = new Vector2(0.75f, 0.75f);

            return hitPlayerScore;
        }
        else //otherwise change the UI position
        {
            //normal UI behaviour for this opponent 
            StartCoroutine(GlitchUI(opponentScore, opponentScoreCollider));
            StartCoroutine(GlitchUI(playerScore, playerScoreCollider));
        }

        Vector3 hit = new Vector3(X, Y);

        if (!isServing) //check if player hit UI only when opponent not serving
        {
            if (playerScoreCollider.OverlapPoint(hit)) //start button mash when player score gets to 7
            {
                print("hit player ui");
                if (GameManager.Instance.playerWins == 7)
                {
                    StartCoroutine(StartButtonMash());
                }
                else StartCoroutine(HitPlayerUI());
                //else return hitPlayerScore;
            }
            else if (opponentScoreCollider.OverlapPoint(hit)) //start button mash when opponent score gets to 7
            {
                print("hit opp ui");
                if (GameManager.Instance.opponentWins >= 7) StartCoroutine(StartButtonMash());
                else StartCoroutine(HitOpponentUI());
            }
        }

        return base.GetOpponentBallPath(X, Y, isServing);
    }

    private IEnumerator HitPlayerUI()
    {
        yield return new WaitForSeconds(1f);
        GameManager.Instance.AddPlayerWin();
    }

    private IEnumerator HitOpponentUI()
    {
        yield return new WaitForSeconds(1f);
        GameManager.Instance.AddOpponentWin();
    }

    public override void ChangeOpponentPosition(float startX, float startY, Vector3 end, int hitFrame) //this is where glitches are called
    {
        if (!introPlayed)
        {
            StartCoroutine(Intro());
            return;
        }

        hits++;
        if (hits >= hitsPerGlitch[GameManager.Instance.playerWins]) //change glitch
        {
            TurnOnGlitch();
        }
    }

    private IEnumerator Intro()
    {
        currGlitch = 3;
        desktopScreen.SetActive(true);
        StartCoroutine(PlayGlitchOutAnimation());
        opponentSprite.SetActive(false);
        //switch player UI into place
        yield return new WaitForSeconds(2.2f); //wait until ball reaches UI and explodes
        pingpong.PauseGame();
        paddleControls.LockInputs();
        print("locked");
        DialogueManager.Instance.StartDialogue(introDialogue);
        yield return new WaitUntil(() => DialogueManager.Instance.DialogueRunning());
        yield return new WaitUntil(() => !DialogueManager.Instance.DialogueRunning());

        paddleControls.UnlockInputs();
        introPlayed = true;
        pingpong.playerServing = true;
        pingpong.ResetRound();
        print("unlocked");
    }

    //rotating glitches
    #region

    private void TurnOnGlitch()
    {
        print("turn on glitch");
        //reset stuff
        hits = 0;
        TurnOffGlitch(currGlitch);

        int glitch = Random.Range(0, 3); //get random glitch
        if (glitch == currGlitch) currGlitch = (currGlitch + 1) % 4; //this is so that the same glitch doesn't play twice
        else currGlitch = glitch;
        volume.profile = postProcessEffects[currGlitch]; //turn on post process effect
        StartCoroutine(PlayGlitchOutAnimation());
        paddleControls.invertArrowKeys = false;

        if (currGlitch == 0) //turn on glitch
        {
            flicker = true;
            StartCoroutine(Flicker(ball));
            StartCoroutine(Flicker(shadow));
            StartCoroutine(Flicker(paddle));
            StartCoroutine(FlickerPostProcessing());
        }
        else if (currGlitch == 1)
        {
            paddleControls.invertArrowKeys = true;
            cam.orthographicSize = -5.375873f;
        }
        else if (currGlitch == 2)
        {
            trail = true;
            StartCoroutine(Trail(ball));
            StartCoroutine(Trail(shadow));
            StartCoroutine(Trail(paddle));
            StartCoroutine(Trail(playerSwipes));
        }
        else if (currGlitch == 3)
        {
            desktopScreen.SetActive(true);
        }
    }

    private IEnumerator PlayGlitchOutAnimation()
    {
        glitchOutAnimation.SetActive(true);
        yield return new WaitForSeconds(20 / 30f);
        glitchOutAnimation.SetActive(false);
    }

    private void TurnOffGlitch(int glitch)
    {
        globalVolume.SetActive(true);
        paddleControls.invertArrowKeys = false;

        if (glitch == 0)
        {
            flicker = false;
        }
        else if (glitch == 1)
        {
            cam.orthographicSize = 5.375873f;
        }
        else if (glitch == 2)
        {
            trail = false;
        }
        else if (glitch == 3)
        {
            desktopScreen.SetActive(false);
        }
    }
   
    private IEnumerator Flicker(GameObject gameObj)
    {
        SpriteRenderer originalSpriteRenderer = gameObj.GetComponent<SpriteRenderer>();
        GameObject flickerGameObj = new GameObject(gameObj.name + "_flicker", typeof(SpriteRenderer));
        SpriteRenderer flickerSpriteRenderer = flickerGameObj.GetComponent<SpriteRenderer>();
        flickerSpriteRenderer.sortingLayerID = originalSpriteRenderer.sortingLayerID;

        while (flicker)
        {
            flickerSpriteRenderer.sprite = originalSpriteRenderer.sprite;
            originalSpriteRenderer.color = Color.clear;
            flickerGameObj.transform.position = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
            flickerGameObj.SetActive(true);
            yield return new WaitForSeconds(flickerLength / 24f); //time for flicker to be active


            flickerGameObj.SetActive(false);
            originalSpriteRenderer.color = Color.white;
            int frames = Random.Range(flickerMinFrequency, flickerMaxFrequency);
            yield return new WaitForSeconds(frames / 24f); //time to next flicker
        }

        Destroy(flickerGameObj);
    }

    private IEnumerator FlickerPostProcessing()
    {
        while (flicker)
        {
            globalVolume.SetActive(true);
            yield return new WaitForSeconds(flickerLength / 24f); //time for flicker to be active

            globalVolume.SetActive(false);
            int frames = Random.Range(flickerMinFrequency, flickerMaxFrequency);
            yield return new WaitForSeconds(frames / 24f); //time to next flicker
        }
    }

    private IEnumerator Trail(GameObject gameObj)
    {
        SpriteRenderer originalSpriteRenderer = gameObj.GetComponent<SpriteRenderer>();
        int counter = 0;

        while (trail)
        {
            StartCoroutine(CreateDestroyTrailObject(gameObj, originalSpriteRenderer, counter++));
            yield return new WaitForSeconds(trailFrameInterval / 24f);
        }
    }

    private IEnumerator CreateDestroyTrailObject(GameObject gameObj, SpriteRenderer originalSpriteRenderer, int num)
    {
        //print("creatdestroytobj");
        GameObject trailObject = new GameObject(gameObj.name + num.ToString(), typeof(SpriteRenderer));
        SpriteRenderer trailObjectSpriteRenderer = trailObject.GetComponent<SpriteRenderer>();

        trailObject.transform.position = gameObj.transform.position;
        trailObjectSpriteRenderer.sprite = originalSpriteRenderer.sprite;
        trailObjectSpriteRenderer.sortingLayerID = originalSpriteRenderer.sortingLayerID;
        yield return new WaitForSeconds(trailTime / 24f);

        Destroy(trailObject);
    }
    #endregion

    private IEnumerator GlitchUI(GameObject UI, BoxCollider2D collider) //wait until opponent hits to move ui?
    {
        //wait for a random amount of time to switch positions - but enough for player to react
        int wait = Random.Range(6, 12);
        Vector3 newPos = new Vector3(Random.Range(minCollider.x, maxCollider.x), Random.Range(minCollider.y, maxCollider.y));
        collider.gameObject.transform.position = newPos;

        yield return new WaitForSeconds((wait / 24f) + 1.0f); //time it takes for ball to travel + wait time
        UI.transform.position = newPos;
        UI.transform.localScale = new Vector2(0.75f, 0.75f);
    }

    private IEnumerator StartButtonMash()
    {
        yield return new WaitForSeconds(23 / 24f); //wait until ball is just about to hit the UI
        yield return PlayGlitchOutAnimation();
        yield return new WaitForSeconds(0.5f);
        buttonMash.SetActive(true);
        TurnOffGlitch(currGlitch); //turn off glitch
        paddleControls.LockInputs(); //pause game
        pingpong.PauseGame();
        globalVolume.SetActive(false);
    }

    public IEnumerator UIMoveWhilePlayerServe()
    {
        print("ui move while player serves");
        playerHit = false;
        while (!playerHit)
        {
            yield return GlitchUI(playerScore, playerScoreCollider);
            StartCoroutine(GlitchUI(opponentScore, opponentScoreCollider));
        }
        print("end ui moves while player serves");
    }

    public override IEnumerator PlayServeAnimation(float waitTime)
    {
        //yield return new WaitForSeconds(waitTime);
        print("play serve animation");
        yield return GlitchUI(playerScore, playerScoreCollider);
        StartCoroutine(GlitchUI(opponentScore, opponentScoreCollider));
        yield return new WaitForSeconds(0.5f);
        print("end of serve animation");
    }

    //other overrides
    #region
    public override IEnumerator PlayLoseRoundAnimation()  //kinda JANKY but using this function to move UI while player is serving
    {
        yield return new WaitForSeconds(0f);
    }

 

    public override void PlayWinAnimation()
    {
    }

    public override void PlayLoseAnimation()
    {
    }
    #endregion
}
