using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class DialogueBubble : MonoBehaviour
{
    //text and dialogue
    [SerializeField] private GameObject text;
    private TMPro.TMP_Text shownText;
    [SerializeField] private TMPro.TMP_Text preText, preText2;
    private DialogueUI dialogueUI;
    private DialogueRunner dialogueRunner;

    //bubble
    [SerializeField] private RectTransform upLeft, upRight, bottomRight, bottomLeft, width, height, tail, textTransform, preTextTransform, preText2Transform, bubbleTransform, bubbleTextTransform;
    [SerializeField] private UnityEngine.UI.ContentSizeFitter contentSizeFitter_preText2;
    [SerializeField] private GameObject bubbleStatic, bubbleEnterExit;
    private Animator bubbleEnterExitAnimator;
    public bool isChangingSpeaker = true;

    //dimensions and frame data
    [SerializeField] private float oneLineHeight, twoLineHeight, threeLineHeight, fourLineHeight, fiveLineHeight, bubbleHeight, overshootHeight;
    [SerializeField] private Vector2 oneLinePosition, multiLinePosition;
    [SerializeField] private int resizeFrames;
    [SerializeField] private float enterExitSpeed = 1f;
    private float ULx, URx, BRx, BLx, Wx, Hx;

    [SerializeField] private GameObject mainCamera;

    private void Start()
    {
        dialogueUI = FindObjectOfType<DialogueUI>();
        dialogueRunner = FindObjectOfType<DialogueRunner>();
        shownText = text.GetComponent<TMPro.TMP_Text>();
        bubbleEnterExit.SetActive(false);

        ULx = -150f;
        URx = 150f;
        BLx = -150f;
        BRx = 150f;
        Wx = 180f;
        Hx = 425f;
    }

    public IEnumerator ChangeSpeaker(string name)
    {
        isChangingSpeaker = true;
        StartCoroutine(ExitBubbleHelper());
        yield return new WaitForSeconds(8 / (30 * enterExitSpeed));
        MoveBubbleToSpeaker(name);
    }

    public void MoveBubbleToSpeaker(string name)
    {
        GameObject speaker = GameObject.Find(name);
        float height = ((speaker.transform.position.y + bubbleHeight) / 5.344528f) * 500; //subtract offset from camera position, convert height to rectangle numbers
        float horizontal = ((speaker.transform.position.x - mainCamera.transform.position.x) / 10) * 1000;
        bubbleTextTransform.anchoredPosition = new Vector3(horizontal, height);
    }

    public void ExitBubble()
    {
        StartCoroutine(ExitBubbleHelper());
    }

    public IEnumerator ExitBubbleHelper()
    {
        //play exit bubble animation
        TransparentStaticBubble();
        //tail.anchoredPosition = new Vector2(tail.anchoredPosition.x, tail.anchoredPosition.y - overshootHeight); //tail
        bubbleEnterExit.SetActive(true);
        bubbleEnterExitAnimator = bubbleEnterExit.GetComponent<Animator>();
        bubbleEnterExitAnimator.SetTrigger("exit");
        yield return new WaitForSeconds(4 / (30 * enterExitSpeed));

        bubbleEnterExit.SetActive(false);
    }

    private void TransparentStaticBubble()
    {
        upLeft.gameObject.GetComponent<UnityEngine.UI.Image>().color = Color.clear;
        upRight.gameObject.GetComponent<UnityEngine.UI.Image>().color = Color.clear;
        bottomRight.gameObject.GetComponent<UnityEngine.UI.Image>().color = Color.clear; 
        bottomLeft.gameObject.GetComponent<UnityEngine.UI.Image>().color = Color.clear; 
        width.gameObject.GetComponent<UnityEngine.UI.Image>().color = Color.clear;
        height.gameObject.GetComponent<UnityEngine.UI.Image>().color = Color.clear;
        tail.gameObject.GetComponent<UnityEngine.UI.Image>().color = Color.clear;
    }

    private void OpaqueStaticBubble()
    {
        upLeft.gameObject.GetComponent<UnityEngine.UI.Image>().color = Color.white;
        upRight.gameObject.GetComponent<UnityEngine.UI.Image>().color = Color.white;
        bottomRight.gameObject.GetComponent<UnityEngine.UI.Image>().color = Color.white;
        bottomLeft.gameObject.GetComponent<UnityEngine.UI.Image>().color = Color.white;
        width.gameObject.GetComponent<UnityEngine.UI.Image>().color = Color.white;
        height.gameObject.GetComponent<UnityEngine.UI.Image>().color = Color.white;
        tail.gameObject.GetComponent<UnityEngine.UI.Image>().color = Color.white;
    }

    public void AdjustBubble()
    {
        if (!isChangingSpeaker)
            StartCoroutine(ABHelper());
        else
        {
            StartCoroutine(EnterBubbleHelper());
            isChangingSpeaker = false;
        }
    }

    private IEnumerator ABHelper()
    {
        print("adjust bubble");
        yield return SetUpText();
        float widthAdjustment = calcWidthAdjustment();
        float newHeight = calcHeight();

        float curveIncrement = 1f / resizeFrames;
        Vector2 startBL = bottomLeft.anchoredPosition, endBL = new Vector2(BLx - widthAdjustment/2, newHeight);
        Vector2 startBR = bottomRight.anchoredPosition, endBR = new Vector2(BRx + widthAdjustment/2, newHeight);
        Vector2 startUL = upLeft.anchoredPosition, endUL = new Vector2(ULx - widthAdjustment/2, upLeft.anchoredPosition.y);
        Vector2 startUR = upRight.anchoredPosition, endUR = new Vector2(URx + widthAdjustment/2, upRight.anchoredPosition.y);
        Vector2 startW = width.sizeDelta, endW = new Vector2(Wx + widthAdjustment, (-1 * newHeight) + 80);
        Vector2 startH = height.sizeDelta, endH = new Vector2(Hx + widthAdjustment, (-1 * newHeight) - 76);
        for (int i = 0; i <= resizeFrames; i++)
        {
            bottomLeft.anchoredPosition = Vector2.Lerp(startBL, endBL, calcCurve(curveIncrement * i));
            bottomRight.anchoredPosition = Vector2.Lerp(startBR, endBR, calcCurve(curveIncrement * i));
            upLeft.anchoredPosition = Vector2.Lerp(startUL, endUL, calcCurve(curveIncrement * i));
            upRight.anchoredPosition = Vector2.Lerp(startUR, endUR, calcCurve(curveIncrement * i));
            width.sizeDelta = Vector2.Lerp(startW, endW, calcCurve(curveIncrement * i));
            height.sizeDelta = Vector2.Lerp(startH, endH, calcCurve(curveIncrement * i));
            yield return new WaitForSeconds(1 / 30f);
        }

        ResetText();
        text.SetActive(true);
    }

    public void EnterBubble()
    {
        StartCoroutine(EnterBubbleHelper());
    }

    public IEnumerator EnterBubbleHelper()
    {
        //print("enter bubble");
        TransparentStaticBubble();
        yield return SetUpText();
        float widthAdjustment = calcWidthAdjustment();
        float newHeight = calcHeight();

        //play enter bubble animation
        bubbleEnterExit.SetActive(true);
        bubbleEnterExitAnimator = bubbleEnterExit.GetComponent<Animator>();
        bubbleEnterExitAnimator.SetTrigger("enter");
        yield return new WaitForSeconds(6 / (30 * enterExitSpeed));

        bubbleEnterExit.SetActive(false);
        OpaqueStaticBubble();

        //overshoot frames
        bottomLeft.anchoredPosition = new Vector2(BLx - widthAdjustment/2, newHeight - overshootHeight);
        bottomRight.anchoredPosition = new Vector2(BRx + widthAdjustment/2, newHeight - overshootHeight);
        upLeft.anchoredPosition = new Vector2(ULx - widthAdjustment / 2, upLeft.anchoredPosition.y);
        upRight.anchoredPosition = new Vector2(URx + widthAdjustment / 2, upRight.anchoredPosition.y);
        width.sizeDelta = new Vector2(Wx + widthAdjustment, (-1 * newHeight) + 80 + overshootHeight);
        height.sizeDelta = new Vector2(Hx + widthAdjustment, (-1 * newHeight) - 76 + overshootHeight);
        tail.anchoredPosition = new Vector2(tail.anchoredPosition.x, tail.anchoredPosition.y + overshootHeight); //tail

        ResetText();
        text.SetActive(true);
        yield return new WaitForSeconds(2 / (30 * enterExitSpeed));

        //final bubble size
        bottomLeft.anchoredPosition = new Vector2(BLx - widthAdjustment/2, newHeight);
        bottomRight.anchoredPosition = new Vector2(BRx + widthAdjustment/2, newHeight);
        width.sizeDelta = new Vector2(Wx + widthAdjustment, (-1 * newHeight) + 80);
        height.sizeDelta = new Vector2(Hx + widthAdjustment, (-1 * newHeight) - 76);
        tail.anchoredPosition = new Vector2(tail.anchoredPosition.x, tail.anchoredPosition.y - overshootHeight); //tail
    }

    private IEnumerator SetUpText()
    {
        textTransform.anchoredPosition = new Vector2(20f, textTransform.anchoredPosition.y);
        yield return new WaitForSeconds(0.02f);
        preText.text = dialogueUI.preText;
        preText2.text = dialogueUI.preText;
        yield return new WaitForSeconds(0.02f);
        yield return AdjustPretext();
    }

    private float calcWidthAdjustment() //w = 457 for preText2 is same as the default x = 160 for text
    {
        int lines = preText.textInfo.lineCount;
        float w = preTextTransform.sizeDelta.x, h = preTextTransform.sizeDelta.y;

        if (lines == 1)
        {
            textTransform.sizeDelta = new Vector2(preText2Transform.sizeDelta.x - 297f, textTransform.sizeDelta.y);
            w = preText2Transform.sizeDelta.x - 457;
        }
        else
        {
            w = preTextTransform.sizeDelta.x - 160;
        }

        return w/2f;
    }

    private float calcHeight()
    {
        int lines = preText.textInfo.lineCount;
        float h = -1f;

        if (lines == 1)
        {
            textTransform.anchoredPosition = oneLinePosition;
            h *= oneLineHeight;
        }
        else
        {
            textTransform.anchoredPosition = multiLinePosition;
            if (lines == 2)
            {
                h *= twoLineHeight;
            }
            else if (lines == 3)
            {
                h *= threeLineHeight;
            }
            else if (lines == 4)
            {
                h *= fourLineHeight;
            }
            else if (lines == 5)
            {
                h *= fiveLineHeight;
            }
        }

        return h;
    }


    private IEnumerator AdjustPretext()
    {
        int lines = preText.textInfo.lineCount;
        int lastLineChars = preText.textInfo.lineInfo[lines-1].characterCount;
        if (lines > 1 && lastLineChars < 7)
        {
            while (preText.textInfo.lineCount == lines)
            {
                preTextTransform.sizeDelta = new Vector2(preTextTransform.sizeDelta.x + 30, preTextTransform.sizeDelta.y);
                yield return new WaitForSeconds(.02f);
            }
        }
        textTransform.sizeDelta = preTextTransform.sizeDelta;
    }

    private void ResetText()
    {
        preTextTransform.sizeDelta = new Vector2(160f, preTextTransform.sizeDelta.y);
    }

    private void OnEnable()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    private float calcCurve(float x)
    {
        if (x <= 0.5)
        {
            return 2 * Mathf.Pow(x, 2f);
        }
        else
        {
            return (-2 * Mathf.Pow(x - 1f, 2f)) + 1f;
        }
    }
}
