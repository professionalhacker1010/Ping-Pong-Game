using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonArea : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private Animator playerAnimator;

    public bool inMoonArea;
    private bool spiderRevealed = false;
    private void Awake()
    {
        playerAnimator = player.GetComponent<Animator>();
    }
    void Start()
    {
        /*if (inMoonArea)
        {
            playerAnimator.SetBool("Moon", true);
        }
        else playerAnimator.SetBool("Moon", false);

        if (spiderRevealed)
        {
            //make sure spider animator is playing revealed spider
        }*/
    }
}
