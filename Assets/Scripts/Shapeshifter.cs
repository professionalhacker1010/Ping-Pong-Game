using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shapeshifter : Opponent
{
    [SerializeField] float startIntroWaitTime;

    //shifts through 4 different phases - hitting the weak spot makes the shapeshifter mess up, and the player progresses to next pattern
    [SerializeField] private List<GameObject> forms;
    [SerializeField] private Animator poofAnimator;
    [SerializeField] private List<Vector3> badHits;
    [SerializeField] private List<int> hitOffsets;

    private int currPattern = 0, currWeakPoint = 0, shifter;
    private CircleColliderList currColliders;

    //cat hit animators
    [SerializeField] private Animator catHitAnimator;

    //drum hit length
    [SerializeField] private int drumHitFrames;

    //janky bird stuff
    [SerializeField] private List<GameObject> birds;
    private List<BoxCollider2D> birdBoxColliders;
    private List<Animator> birdAnimators;
    private int birdHitter = 0;

    //next position of the transition circle
    [SerializeField] private List<Vector3> transitionCircleTransforms; //ordered from lowest X to highest X
    private int currTransitionCircleTransform = 1;

    //player info
    [SerializeField] private PaddleControls paddleControls;
    [SerializeField] private BallPath playerBallPath;

    //should the opponent hit or not
    private bool shouldHit = true;

    //private bool isServing = false;

    private void Start()
    {
        birdBoxColliders = new List<BoxCollider2D>();
        birdAnimators = new List<Animator>();
        foreach (var bird in birds)
        {
            birdBoxColliders.Add(bird.GetComponent<BoxCollider2D>());
            birdAnimators.Add(bird.GetComponent<Animator>());
        }

        shifter = forms.Count - 1;
        forms[shifter].SetActive(true);
        animator.SetTrigger("idle");
        StartCoroutine(Shapeshift());
    }
    public override Vector3 GetOpponentBallPath(float X, float Y, bool isServing)
    {
        Vector2 hit = new Vector2(X, Y);
   
        if (currPattern == 0) //----------------------------------------cute cate - boop the nose 3 times - non consecutive                                               
        {
            if (Overlaps(currWeakPoint, hit))
            {
                shouldHit = false;
                currWeakPoint++;
                StartCoroutine(WeakSpotAnimation(currWeakPoint.ToString())); //update animation
                if (currWeakPoint == 3) //player has booped 3 times = lose
                {
                    currPattern++;
                    StartCoroutine(Shapeshift());
                    return badHits[currPattern-1];
                }
            }
        }
        else if (currPattern == 1) //-------------------------------------------------------------------drums - do the BA DUM TSHH - consecutive hits required
        {
            if ((currWeakPoint <= 1 && (Overlaps(0, hit) || Overlaps(1, hit))) ||
                (currWeakPoint == 2 && (Overlaps(2, hit) || Overlaps(4, hit) || Overlaps(5,hit))))
            {
                currWeakPoint++;
                StartCoroutine(WeakSpotAnimation(currWeakPoint.ToString())); //update animation
            }
            else //reset drums
            {
                currWeakPoint = 0;
                animator.SetBool("3", false); animator.SetBool("2", false); animator.SetBool("1", false);
                StartCoroutine(HitBack("hitDone", animator));
            }

            if (currWeakPoint == 3)
            {
                shouldHit = false;
                currPattern++;
                StartCoroutine(Shapeshift());
                return badHits[currPattern-1];
            }
        }
        else if (currPattern == 2) //------------------------------------------------------------------------THIS ONE IS SO JANKY the birds. Hit enough birds to create an opening.
        {
            //check that ball hit a bird
            if (!isServing)
            {
                for (int i = 0; i < currColliders.list.Count; i++)
                {
                    if (Overlaps(i, hit))
                    {
                        shouldHit = false;
                        StartCoroutine(DeactivateBird(i)); //todo: bird getting hit animation? Or just a sound effect is good enough lol
                        return base.GetOpponentBallPath(X, Y, isServing);
                    }
                }
            }

            //otherwise the birds hit the ball back
            for (int i = 0; i < birdBoxColliders.Count; i++)
            {
                if (birdBoxColliders[i].OverlapPoint(hit) && birds[i].activeSelf)
                {
                    //make the first bird hit it. Otherwise make the other bird hit it
                    birdHitter = i;
                    return base.GetOpponentBallPath(X, Y, isServing);
                }
            }

            //otherwise yay u won
            currPattern++;
            StartCoroutine(Shapeshift());
            return badHits[currPattern-1];
        }
        else if (currPattern == 3)
        {
            int prevTransform = currTransitionCircleTransform;

            forms[currPattern].transform.position = transitionCircleTransforms[prevTransform];

            //check if player hit circle collider
            if (Overlaps(currWeakPoint, hit))
            {
                currPattern++;
                StartCoroutine(Shapeshift());
                return badHits[currPattern - 1];
            }

            //the transition circle moves to the opposite side of the ball - ball on left side of table -> circle on right side and vice versa
            //y value is randomly picked from predeterminedHits
            else if (X <= 0f)
            {
                currTransitionCircleTransform++;
                if (currTransitionCircleTransform >= transitionCircleTransforms.Count) currTransitionCircleTransform = transitionCircleTransforms.Count - 1;
            }
            else
            {
                currTransitionCircleTransform--;
                if (currTransitionCircleTransform < 0) currTransitionCircleTransform = 0;
            }

            
        }

        return base.GetOpponentBallPath(X, Y, isServing);
    }

    public override void ChangeOpponentPosition(float startX, float startY, Vector3 end, int hitFrame)
    {
        StartCoroutine(ChangeOpponentPositionHelper(startX, startY));
    }

    private IEnumerator ChangeOpponentPositionHelper(float startX, float startY)
    {
        Debug.Log("change opp pos");
        yield return new WaitForSeconds(0.1f);
        Vector3 hit = new Vector3(startX, startY);
        if (shouldHit)
        {
            if (currPattern == 0)
            {
                if (startX <= 0) StartCoroutine(HitBack("hitLeft", catHitAnimator)); //play swipe animation
                else StartCoroutine(HitBack("hitRight", catHitAnimator));
            }
            else if (currPattern == 1)
            {
                //beat the correct drum
                for (int i = 0; i < currColliders.list.Count; i++)
                {
                    if (Overlaps(i, hit))
                    {
                        StartCoroutine(HitBack(("drum" + (i + 1).ToString()), animator));
                        break;
                    }
                }
            }
            else if (currPattern == 2)
            {
                StartCoroutine(HitBack("hit", birdAnimators[birdHitter]));
            }
            else if (currPattern == 3 || currPattern == 4)
            {
                Debug.Log("set trigger in");
                animator.SetTrigger("In");
                animator.SetTrigger("Out");
            }
        }
        else
        {
            shouldHit = true;
        }
    }

    private IEnumerator Shapeshift()
    {
        Debug.Log("shapeshift");
        //reset counter for how many weak points you've hit
        currWeakPoint = 0;

        paddleControls.LockInputs();

        if (currPattern == 0) //intro shapeshift animation
        {
            yield return new WaitForSeconds(startIntroWaitTime);
        }
        else //all other shapeshift animations
        {
            yield return new WaitForSeconds(2.0f); //have to wait for player's hit to reach opponent and for ball to finish exploding

            yield return ShapeshiftAnimation(currPattern - 1, shifter);

            //set expression of shapeshifter here
            if (currPattern == 4) animator.SetTrigger("frown");
            else animator.SetTrigger("idle");

            yield return new WaitForSeconds(0.5f); //linger on idle pose of shifter for a little
        }

        if (currPattern < 4) //don't shift to new form after last phase
        {
            animator.SetTrigger("shapeshift"); //start shapeshift animation
            yield return new WaitForSeconds(1 / 24f);
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length - (3 / 24f)); // wait for length of animation -2 frames

            yield return ShapeshiftAnimation(shifter, currPattern);
        }

        paddleControls.UnlockInputs();
    }

    private IEnumerator ShapeshiftAnimation(int startForm, int endForm)
    {
        Debug.Log("Shapeshift anim");
        poofAnimator.SetTrigger("poof"); //start poof animation
        yield return new WaitForSeconds(4/24f);

        forms[startForm].SetActive(false); //swap out game objects
        forms[endForm].SetActive(true);
        animator = forms[endForm].GetComponent<Animator>();
        currColliders = forms[endForm].GetComponent<CircleColliderList>();
    }

    //updating animation for when player hits a weak spot - BOOLS
    private IEnumerator WeakSpotAnimation(string trigger) 
    {
        Debug.Log("weak spot animation");
        yield return new WaitForSeconds(playerBallPath.endFrame / 24f);
        animator.SetBool(trigger, true);
    }

    //updating animation for opponent hitting ball - TRIGGERS
    private IEnumerator HitBack(string trigger, Animator anim) 
    {
        yield return new WaitForSeconds((playerBallPath.endFrame - hitOffsets[currPattern]) / 24f) ;
        Debug.Log(trigger);
        anim.SetTrigger(trigger);

        if (currPattern == 1)
        {
            yield return new WaitForSeconds(drumHitFrames / 24f);
            anim.SetTrigger("hitDone");
        }
    }

    private IEnumerator DeactivateBird(int bird)
    {
        yield return new WaitForSeconds(playerBallPath.endFrame / 24f);
        birds[bird].SetActive(false);
    }

    private bool Overlaps(int collider, Vector2 hit)
    {
        return currColliders.list[collider].OverlapPoint(hit);
    }

    public override IEnumerator PlayServeAnimation(float waitTime)
    {
        //isServing = true;

        //need to wait a bit longer if serving right after a shapeshift...
        yield return new WaitForSeconds(waitTime - (hitOffsets[currPattern]) / 24f);
        if (currPattern == 0) animator.SetTrigger("hitRight");
        else if (currPattern == 1) animator.SetTrigger("drum4");
        else if (currPattern == 2)
        {
            List<int> servers = new List<int> { 4, 5, 2, 3 };
            foreach (var server in servers)
            {
                if (birds[server].activeSelf)
                {
                    birdAnimators[server].SetTrigger("hit");
                    break;
                }
            }
        }
        else if (currPattern == 3)
        {
            animator.SetTrigger("In");
            animator.SetTrigger("Out");
        }

        //isServing = false;
    }
    public override void PlayWinAnimation()
    {

    }

    public override void PlayLoseAnimation()
    {
    }
    
}
