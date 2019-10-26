using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // if GameManager.Instance is invoked, can access public members of this class

    public Camera cam;

    public delegate void GameDelegate(); //allows creation of events for other scripts to be notified of
    public static event GameDelegate OnGameStarted;
    public static event GameDelegate OnInitGameOverConfirmed;
    //public static event GameDelegate OnIncreaseSpeed; //increase speed of blocks falling

    public GameObject mainGamePage;
    public GameObject gameOverPage;
    public GameObject startPage;
    public GameObject countdownPage;
    public Text scoreText;

    static int score = 0;
    bool inGame = false;
    bool initGameOver = false;
    bool finalGameOver = false;

    public bool InGame { get { return inGame; } } //start game on first tap
    public bool InitGameOver { get { return initGameOver; } } //player falls
    public bool FinalGameOver { get { return finalGameOver; } } //player hits bottom of screen
    public int Score { get { return score; } }

    void OnEnable()
    {
        AirpacaControl.OnPlayerDied += OnPlayerDied;
        AirpacaControl.OnPlayerScored += OnPlayerScored;
        AirpacaControl.OnPlayerHitBottom += OnPlayerHitBottom;
        CountdownText.OnCountdownFinished += StartGame;
    }

    void OnDisable()
    {
        AirpacaControl.OnPlayerDied -= OnPlayerDied;
        AirpacaControl.OnPlayerScored -= OnPlayerScored;
        AirpacaControl.OnPlayerHitBottom -= OnPlayerHitBottom;
        CountdownText.OnCountdownFinished -= StartGame;
    }

    void Awake()
    {
        Instance = this;
        initGameOver = false;
        PlayerPrefs.SetInt("High Score", 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        //SetPageState(PageState.Main);
        SetPageState(PageState.Start);

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnPlayerDied()
    {
        initGameOver = true;
        inGame = false;
    }

    void OnPlayerHitBottom()
    {
        int savedScore = PlayerPrefs.GetInt("High Score");
        if (score > savedScore)
        {
            PlayerPrefs.SetInt("High Score", score); // sets new high score
        }
        SetPageState(PageState.GameOver);
        finalGameOver = true;
        initGameOver = true;
        inGame = false;
    }

    void OnPlayerScored()
    {
        score += 1;
        //Debug.Log(score);
    }

    enum PageState // enum creates custom type; type in this case is PageState
    {
        Main,
        Start,
        GameOver,
        Countdown,
    }

    void SetPageState(PageState state)
    {
        switch (state)
        {
            case PageState.Main:
                mainGamePage.SetActive(true);
                gameOverPage.SetActive(false);
                startPage.SetActive(false);
                countdownPage.SetActive(false);
                break;
            case PageState.Start:
                mainGamePage.SetActive(false);
                gameOverPage.SetActive(false);
                startPage.SetActive(true);
                countdownPage.SetActive(false);
                break;
            case PageState.GameOver:
                mainGamePage.SetActive(false);
                gameOverPage.SetActive(true);
                startPage.SetActive(false);
                countdownPage.SetActive(false);
                break;
            case PageState.Countdown:
                mainGamePage.SetActive(false);
                gameOverPage.SetActive(false);
                startPage.SetActive(false);
                countdownPage.SetActive(true);
                break;
        }
    }

    public void StartCountdown()
    {
        initGameOver = false;
        finalGameOver = false;
        score = 0;
        SetPageState(PageState.Countdown);
    }

    void StartGame()
    {
        inGame = true;
        SetPageState(PageState.Main);
    }
}
