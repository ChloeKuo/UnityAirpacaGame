using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ScoreText : MonoBehaviour
{
    GameManager game;
    Text score;

    // Start is called before the first frame update
    void Start()
    {
        game = GameManager.Instance;
        score = GetComponent<Text>();
        score.text = (game.Score.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        score.text = (game.Score.ToString());
    }

    void OnEnable()
    {
        
    }
}
