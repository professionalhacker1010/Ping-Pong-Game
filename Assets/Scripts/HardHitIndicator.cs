using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardHitIndicator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer blackScreen;
    [SerializeField] private int fadeOutFrames;
    
    public void FadeToBlack(int frames)
    {
        StartCoroutine(FadeToBlackHelper(frames));
    }

    private IEnumerator FadeToBlackHelper(int frames)
    {
        //Debug.Log("fade in");
        for (int i = 0; i < frames-1; i++)
        {
            blackScreen.color = new Color(255, 255, 255, blackScreen.color.a + ((1 - blackScreen.color.a) / 3)); //ease in to opaque, half chunks at a time
            frames--;
            yield return new WaitForSeconds(1 / 24f);
        }

        blackScreen.color = new Color(255, 255, 255, 1f);
    }

    public void FadeToOpaque()
    {
        StartCoroutine(FadeToOpaqueHelper());
    }

    private IEnumerator FadeToOpaqueHelper()
    {
        //Debug.Log("fade out");
        int frames = fadeOutFrames;
        for (int i = 0; i < frames-1; i++)
        {
            blackScreen.color = new Color(255, 255, 255, blackScreen.color.a - (blackScreen.color.a / 2)); //ease in to transparency, half chunks at a time
            frames--;
            yield return new WaitForSeconds(1 / 24f);
        }
        blackScreen.color = new Color(255, 255, 255, 0);
    }
}
