using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPath : MonoBehaviour
{
    //private Ball ball;

    //constants for LOW BALL
        //before bounce
        [SerializeField] public float A1, X1, X2; // x2 doubles as the frame ball bounces on
        //after bounce
        [SerializeField] public float A2, X3;
        //number of frames in the ball animation
        [SerializeField] public int highestFrameLow, endFrameLow, bounceFrameLow;

    //quadratic equation constants for HIGH BALL. These equations calculate the PERCENT OF MAX HIEGHT for each of the 24 frames
        //before bounce
        [SerializeField] public float A3, X4, X5; // x2 doubles as the frame ball bounces on
        //after bounce
        [SerializeField] public float A4, X6;
        //number of frames in the ball animation
        [SerializeField] public int highestFrameHigh, endFrameHigh, bounceFrameHigh;

    //constants for CURRENT BALL
        //before bounce
        [HideInInspector] public float a1, x1, x2; // x2 doubles as the frame ball bounces on
        //after bounce
        [HideInInspector] public float a2, x3;
        //number of frames in the ball animation
        [HideInInspector] public int highestFrame, endFrame, bounceFrame;

    public void SwitchPath(string path)
    {
        if (path == "high")
        {
            a1 = A3; a2 = A4;
            x1 = X4; x2 = X5; x3 = X6;
            highestFrame = highestFrameHigh; endFrame = endFrameHigh; bounceFrame = bounceFrameHigh;
        }
        else if (path == "low")
        {
            a1 = A1; a2 = A2;
            x1 = X1; x2 = X2; x3 = X3;
            highestFrame = highestFrameLow; endFrame = endFrameLow; bounceFrame = bounceFrameLow;
        }
    }

    public void CopyBallPath(BallPath desired)
    {
        A1 = desired.A1;
        A2 = desired.A2;
        A3 = desired.A3;
        A4 = desired.A4;

        X1 = desired.X1;
        X2 = desired.X2;
        X3 = desired.X3;
        X4 = desired.X4;
        X5 = desired.X5;
        X6 = desired.X6;

        highestFrameHigh = desired.highestFrameHigh;
        endFrameHigh = desired.endFrameHigh;
        bounceFrameHigh = desired.bounceFrameHigh;

        highestFrameLow = desired.highestFrameLow;
        endFrameLow = desired.endFrameLow;
        bounceFrameLow = desired.bounceFrameLow;
    }
}
