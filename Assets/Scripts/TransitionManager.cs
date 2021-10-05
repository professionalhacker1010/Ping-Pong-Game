using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    private Animator animator;
    public GameObject transitionObject;
    public bool isTransitioning = false; //for pause menu - don't wanna be able to pause while transitioning

    #region
    private static TransitionManager _instance;
    public static TransitionManager Instance
    {
        get
        {
            if (_instance == null) Debug.Log("The TransitionManager is NULL");

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
        animator = transitionObject.GetComponent<Animator>();
    }
    #endregion

    public void StartIn()
    {
        animator.SetTrigger("StartIn");
        StartCoroutine(flipIsTransitioning(true));

        StopAllCoroutines();
        StartCoroutine(DeactivateObject());
    }

    public void QuickIn()
    {
        animator.SetTrigger("QuickIn");
        StartCoroutine(flipIsTransitioning(true));

        StopAllCoroutines();
        StartCoroutine(DeactivateObject());
    }

    public void SlowIn()
    {
        animator.SetTrigger("SlowIn");
        StartCoroutine(flipIsTransitioning(true));

        StopAllCoroutines();
        StartCoroutine(DeactivateObject());
    }

    public void Transparent()
    {
        animator.SetTrigger("Transparent");
        animator.speed = 0f;
    }

    public void QuickOut()
    {
        Debug.Log("quick out called");
        StartCoroutine(QuickOutHelper());
    }

    public IEnumerator QuickOutHelper()
    {
        Debug.Log("Quick out helper");
        animator.speed = 1f;
        yield return new WaitForSecondsRealtime(0.1f);

        animator.SetTrigger("QuickOut");
        yield return flipIsTransitioning(false);
    }

    public void SlowOut()
    {
        animator.speed = 1f;
        animator.SetTrigger("SlowOut");
        StartCoroutine(flipIsTransitioning(false));
    }

    private IEnumerator DeactivateObject()
    {
        yield return new WaitForSecondsRealtime(3.0f);
        yield return new WaitForSecondsRealtime(animator.GetCurrentAnimatorStateInfo(0).length);
        animator.speed = 0f;
    }

    private IEnumerator flipIsTransitioning(bool value)
    {
        yield return new WaitForSecondsRealtime(animator.GetCurrentAnimatorStateInfo(0).length);

        isTransitioning = value;
    }
}
