using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreDatabase : MonoBehaviour
{
    #region
    private static ScoreDatabase _instance;
    public static ScoreDatabase Instance
    {
        get
        {
            if (_instance == null) Debug.Log("The ScoreDatabase is NULL");

            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
    }
    #endregion

    [Header("Default UI")]
    [SerializeField] private List<Sprite> numbers;
    [SerializeField] private List<Sprite> bars;
    [SerializeField] private SpriteRenderer playerScore, playerBar, YOU;
    [SerializeField] private SpriteRenderer opponentScore, opponentBar, THEM;
    [SerializeField] private GameObject player, opponent;

    [Header("Pixellated UI")]
    [SerializeField] private List<Sprite> pixellatedNumbers;
    [SerializeField] private List<Sprite> pixellatedBars;
    [SerializeField] private List<Sprite> pixellatedNames;


    [SerializeField] private int enlargeScoreFrames;
    private Vector3 defaultPosition, defaultPositionOpponent;
    private Vector3 defaultScale, defaultScaleOpponent;

    private void Start()
    {
        defaultPosition = player.transform.localPosition;
        defaultPositionOpponent = opponent.transform.localPosition;
        defaultScale = player.transform.localScale;
        defaultScaleOpponent = opponent.transform.localScale;
    }

    public void addPlayerScoreboard()
    {
        //print("player score +1");
        int score = GameManager.Instance.playerWins;
        playerScore.sprite = numbers[score % 10];
        playerBar.sprite = bars[score % 5];
        StartCoroutine(EnlargePlayerScore());
    }

    private IEnumerator EnlargePlayerScore()
    {
        //player.transform.localPosition = new Vector3(1.7f, -1f);
        player.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
        yield return new WaitForSeconds(enlargeScoreFrames / 24f);

        //player.transform.localPosition = defaultPosition;
        player.transform.localScale = defaultScale;
    }

    public void addOpponentScoreboard()
    {
        //print("opponent score +1");
        int score = GameManager.Instance.opponentWins;
        opponentScore.sprite = numbers[score % 10];
        opponentBar.sprite = bars[score % 5];
        StartCoroutine(EnlargeOpponentScore());
    }

    private IEnumerator EnlargeOpponentScore()
    {
        //opponent.transform.localPosition = new Vector3(-1.7f, -1f);
        opponent.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
        yield return new WaitForSeconds(enlargeScoreFrames / 24f);

       // opponent.transform.localPosition = defaultPositionOpponent;
        opponent.transform.localScale = defaultScaleOpponent;
    }

    public void SwapToPixellated()
    {
        numbers = pixellatedNumbers;
        bars = pixellatedBars;

        playerScore.sprite = numbers[0];
        opponentScore.sprite = numbers[0];
        playerBar.sprite = bars[0];
        opponentBar.sprite = bars[0];
        YOU.sprite = pixellatedNames[0];
        THEM.sprite = pixellatedNames[1];

        playerScore.sortingLayerName = "Opponent Paddle";
        opponentScore.sortingLayerName = "Opponent Paddle";
        playerBar.sortingLayerName = "Opponent Paddle";
        opponentBar.sortingLayerName = "Opponent Paddle";
        YOU.sortingLayerName = "Opponent Paddle";
        THEM.sortingLayerName = "Opponent Paddle";
    }
}
