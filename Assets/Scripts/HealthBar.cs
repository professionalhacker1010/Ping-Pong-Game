using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private RectTransform bar;
    [SerializeField] private bool isPlayerHealth;
    private float points = 0;
    private float smallStarDamage = 0, largeStarDamage = 0, hitOutDamage = 0; //when to trigger stuff based on how filled the bar is
    private float barUnit = 0;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    public void Initialize(float maxPoints, float smallDamage, float largeDamage, float badHitDamage)
    {
        points = maxPoints;
        smallStarDamage = smallDamage;
        largeStarDamage = largeDamage;
        hitOutDamage = badHitDamage;
        barUnit = bar.sizeDelta.y / maxPoints;
    }

    public void Damage(int strength)
    {
        if (strength == 0) points -= smallStarDamage;
        else if (strength == 1) points -= largeStarDamage;
        else if (strength == -1) points -= hitOutDamage;
        bar.sizeDelta = new Vector2(bar.sizeDelta.x, barUnit * points);

        if (points <= 0)
        {
            if (isPlayerHealth)
            {
                //trigger lose game stuff... or reset to last checkpoint
                print("player health is ZERO");
                GameManager.Instance.opponentWins = 1000;
            }
            else
            {
                //trigger win game stuff
                print("opponent health is ZERO");
                GameManager.Instance.playerWins = 1000;
            }
            GameManager.Instance.GameOver();
        }
    }

    public float Points()
    {
        return points;
    }
}
