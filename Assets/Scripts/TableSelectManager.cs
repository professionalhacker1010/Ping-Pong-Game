using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//add this

public class TableSelectManager : MonoBehaviour
{
    #region
    private static TableSelectManager _instance;
    public static TableSelectManager Instance
    {
        get
        {
            if (_instance == null) Debug.Log("The TableSelectManager is NULL");

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }
    #endregion

    [SerializeField] private List<GameObject> tables;
    public static bool firstGameStarted = false;
    public bool selectable = true;
    private int locks = 0;
   
    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(TransitionIn());
    }

    public float TableTransformX()
    {
        return tables[LevelManager.chosenOpponent].transform.position.x;
    }

    public IEnumerator TransitionToGame(int level)
    {
        if (level == 0) firstGameStarted = true;
        TransitionManager.Instance.QuickOut();
        yield return new WaitForSeconds(21 / 24f);
        LevelManager.chosenOpponent = level;
        SceneManager.LoadScene("Game");
    }
   
    public void LockSelection()
    {
        locks++;
        selectable = false;
    }

    public void UnlockSelection()
    {
        locks--;
        if (locks == 0) selectable = true;
    }

    private IEnumerator TransitionIn()
    {
        yield return new WaitForSeconds(0.5f);
        TransitionManager.Instance.QuickIn();
    }
}
