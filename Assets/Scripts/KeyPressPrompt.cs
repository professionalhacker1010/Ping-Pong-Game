using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPressPrompt : MonoBehaviour
{
    private Vector3 hiddenPosition;

    //private bool showCondition = false;
    private Vector3 shownPosition;
    private System.Func<bool> showCondition = null;

    private void Start()
    {
        hiddenPosition = transform.position;
    }

    private void Update()
    {
        if (showCondition != null)
        {
            if (showCondition())
            {
                StartCoroutine(ShowWhile(showCondition, shownPosition));
            }
        }
    }

    public void SetConditions(System.Func<bool> show, Vector3 position)
    {
        showCondition = show;
        shownPosition = position;
    }

    public void RemoveConditions()
    {
        StopAllCoroutines();
        Hide();
        showCondition = null;
    }

    public IEnumerator ShowWhile(System.Func<bool> condition, Vector3 position)
    {
        Show(position);
        yield return new WaitWhile(condition);
        Hide();
    }

    public void Show(Vector3 position)
    {
        transform.position = position;
    }

    public void Hide()
    {
        transform.position = hiddenPosition;
    }
}
