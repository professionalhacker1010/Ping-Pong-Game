using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakePlayer : MonoBehaviour
{
    [SerializeField] private GameObject playerLeft, playerRight, playerLeftArm, playerRightArm;
    private Animator playerSwipeAnimator;

    // Start is called before the first frame update
    void Start()
    {
        playerSwipeAnimator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (KeyCodes.Left())
        {
            print("face left");
            playerLeft.SetActive(true);
            playerRight.SetActive(false);
            playerSwipeAnimator.SetBool("faceLeft", true);
            playerSwipeAnimator.SetBool("faceRight", false);
        }
        else if (KeyCodes.Right())
        {
            print("face right");
            playerLeft.SetActive(false);
            playerRight.SetActive(true);
            playerSwipeAnimator.SetBool("faceLeft", false);
            playerSwipeAnimator.SetBool("faceRight", true);
        }

        if (KeyCodes.Hit())
        {
            StartCoroutine(FakePlayerSwipe());
        }
    }

    private IEnumerator FakePlayerSwipe()
    {
        int arm = -1;
        if (playerLeftArm.activeInHierarchy)
        {
            playerLeftArm.SetActive(false);
            arm = 0;
        }
        else if (playerRightArm.activeInHierarchy)
        {
            playerRightArm.SetActive(false);
            arm = 1;
        }

        playerSwipeAnimator.SetTrigger("swipe");
        yield return new WaitForSeconds(6 / 24f);

        if (arm == 0)
            playerLeftArm.SetActive(true);
        else if (arm == 1)
            playerRightArm.SetActive(true);
    }
}
