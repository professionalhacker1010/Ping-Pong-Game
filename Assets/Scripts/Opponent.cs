using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opponent : MonoBehaviour
{
    
    [Header("Base Opponent")]
    //animation
    [SerializeField] protected Animator animator;
    [SerializeField] public int oppHitFrame; //frame that opponent hits in the animation - will probably need to be about 2 frames after opponent actually hits
    [SerializeField] protected GameObject hitFlash;

    //data on opponent behavior
    [SerializeField] public BallPath opponentPath;
    [SerializeField] protected List<Vector3> predeterminedHits;
    [SerializeField] public Vector3 servePosition;

    //defines behavior of where opponent hits based on where player hits - make custom behavior for each opponent
    virtual public Vector3 GetOpponentBallPath(float X, float Y, bool isServing)
    {
        int i = (int) Random.Range(0, predeterminedHits.Count);
        return predeterminedHits[i];
    }

    virtual public void ChangeOpponentPosition(float startX, float startY, Vector3 end, int hitFrame)
    {
        transform.position = new Vector3(startX, 0f);
    }

    virtual public void HitFlash(float X, float Y)
    {
        hitFlash.transform.position = new Vector3(X + 0.5f, Y);
        hitFlash.SetActive(true);
        StartCoroutine(HitFlashWait());
    }

    private IEnumerator HitFlashWait()
    {
        yield return new WaitForSeconds(6 / 24f);
        hitFlash.SetActive(false);
    }

    virtual public IEnumerator PlayServeAnimation(float waitTime) 
    {
        yield return new WaitForSeconds(waitTime);
    }

    virtual public void PlayWinAnimation()
    {
        animator.SetTrigger("Win");
    }

    virtual public void PlayLoseAnimation()
    {
        animator.SetTrigger("Lose");
    }

    virtual public IEnumerator PlayLoseRoundAnimation()
    {
        yield return new WaitForSeconds(0f);
    }
}
