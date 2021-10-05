using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBall : MultiBall
{

    protected override void HitBehaviour()
    {
        if (patternStarted)
        {
            lastOpponentHitTime = Time.time;

            transform.position = currArm;
            //base.CalcOpponentBallPath(true, ballPositions[currPosition]);
            //currPosition = (currPosition + 1) % ballPositions.Count;
            patternStarted = false;
        }
        else if (!patternEnded && Time.time - lastOpponentHitTime >= 2.0f / defaultSpeed)
        {
            patternEnded = true;
            ExplodeBall(true);
        }
    }

    public void HitBall()
    {
        CalcOpponentBallPath2(ballPositions[currPosition]);
        currPosition = (currPosition + 1) % ballPositions.Count;
    }

    protected override void SetBallPath(int startFrame, int endFrame, bool playerLose)
    {
        SetBallPath2(startFrame, endFrame, playerLose);
    }
}
