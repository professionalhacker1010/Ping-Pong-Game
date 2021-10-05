using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kevin : Opponent
{
    //if 0 player wins, follow the circular pattern, which should be the first 0-5 items
    //if 1 player wins, follow the left/right pattern, which should be items 6-10
    //if 2 player wins, follow the up/down pattern, which should be items 11-14
    //if 3 player wins, follow the psuedo-random pattern, which should be items 15-29..?
    //the last hit (30) is the HARD HIT!, which goes right down the middle

    private int currHit = 0, currPattern = 0;
    private int prevKevinWins = 0;
    [SerializeField] private List<int> patternStarts;
    [SerializeField] private Vector3 kevinMisses;
    [SerializeField] private Pingpong pingPong;
    [SerializeField] private PaddleControls paddleControls;
    [SerializeField] private GameObject tutorialUI, holdSpace, releaseToSlam;

    public override Vector3 GetOpponentBallPath(float X, float Y, bool isServing)
    {
        //reset pattern if player lost
        if (playerLost())
        {
            currHit = patternStarts[currPattern];
        }
        //if currHit is at the start of the next pattern, then the player has successfully played through the current pattern
        //Kevin should miss and currHit should NOT increment
        else if (currHit == patternStarts[currPattern + 1])
        {
            currPattern++;
            return kevinMisses;
        }
        return predeterminedHits[currHit++]; //get the current hit, then increment
    }

    //we need a way to know when to reset the pattern - this checks if the player has lost and also updates Kevin's variables accordingly
    private bool playerLost()
    {
        if (prevKevinWins < GameManager.Instance.opponentWins)
        {
            prevKevinWins++;
            return true;
        }
        return false;
    }

    public override void ChangeOpponentPosition(float startX, float startY, Vector3 end, int hitFrame)
    {
        //determine which animation to play
        string animation;
        if (end.z != 0) //hesitate and return out of function when end Z is not zero
        {
            animator.SetTrigger("Hesitate");
            StartCoroutine(HesistateFrames()); //hesitate only for a certain amount of frames
            return;
        }
        else if (animator.transform.position.x <= end.x) //fronthand called when current X is less than or equal to endX
        {
            animation = "Backhand";
        }
        else //backhand called when current X is greater than endX
        {
            animation = "Fronthand";
        }

        StartCoroutine(ChangeOpponentPositionOffset((startX - animator.transform.position.x)/4, 4, hitFrame, animation));
    }

    private IEnumerator ChangeOpponentPositionOffset(float Xincrement, int translateFrames, int hitFrame, string animation)
    {
        for (int i = 0; i < hitFrame - oppHitFrame; i++)
        {
            //translate Kevin for 4 even frames, starting 4 frames after ball is hit
            if (translateFrames > 0 && i > 3)
            {
                animator.transform.position = new Vector3(animator.transform.position.x + Xincrement, animator.transform.position.y);
                translateFrames--;
            }
            
            yield return new WaitForSeconds(1 / 24f);
        }

        //play animation
        animator.SetTrigger(animation);
    }

    public override void HitFlash(float X, float Y)
    {
        base.HitFlash(X, Y);
    }

    public override IEnumerator PlayServeAnimation(float waitTime)
    {
        yield return ChangeOpponentPositionOffset((servePosition.x - animator.transform.position.x) / 4, 4, (int)waitTime * 24, "Fronthand");
    }

    private IEnumerator HesistateFrames()
    {
        yield return new WaitForSeconds(2.0f);
        animator.SetTrigger("Idle");
    }
}
