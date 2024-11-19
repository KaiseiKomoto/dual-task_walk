using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class gamemodea: MonoBehaviour
{
    private LeapProvider leapProvider;
    private Finger[] fingers;
    private bool[] isExtendedFingers;
    public enum RSP
    {
        Rock, Scissors, Paper
    };
    public RSP playerChoice;
    public RSP computerChoice;

    public Text resultText; // Reference to the UI Text component to display the result
    public Text gameModeText;

    public static int scorea1 = 0;
    public static int scoreb1 = 0;

    //public GameObject enemyHandModel;
    public Animator handAnimator;

    // Define the game states
    private enum GameState
    {
        Idle,
        computerturn,
        Playing,
        Finished
    }
    private enum GameMode
    {
        Win,
        Tie,
        Lose
    }

    private GameState currentState;

    private float stateStartTime;
    private float nextStateDelay = 5f;

    private GameMode currentMode;


    private void Start()
    {
        scorea1 = 0;
        scoreb1 = 0;

        leapProvider = FindObjectOfType<LeapProvider>();
        fingers = new Finger[5];
        isExtendedFingers = new bool[5];
        //Animator handAnimator = enemyHandModel.GetComponent<Animator>();
        //handAnimator = GetComponent<Animator>();

        //ResetGame();
        //SetRandomGameMode();

        // Start in the Idle state
        currentState = GameState.Idle;
        stateStartTime = Time.time;
    }

    private void Update()
    {
        switch (currentState)
        {
            case GameState.Idle:
                UpdateIdleState();
                break;

            case GameState.computerturn:
                UpdatecomputerturnState();
                break;

            case GameState.Playing:
                UpdatePlayingState();
                break;

            case GameState.Finished:
                UpdateFinishedState();
                break;
        }
    }

    private IEnumerator ComputerTurnCoroutine()
    {
        yield return new WaitForSeconds(1f);  // Adjust the delay time as needed

        ComputerGame();

        currentState = GameState.Playing;
        stateStartTime = Time.time;
    }

    private void UpdateIdleState()
    {
        Frame frame = leapProvider.CurrentFrame;
        if (frame.Hands.Count != 0)
        {
            SetRandomGameMode();
            // Transition to the Playing state
            //currentState = GameState.computerturn;
            //stateStartTime = Time.time;

            StartCoroutine(ComputerTurnCoroutine());  // Start the coroutine
        }
        else if (Time.time - stateStartTime >= nextStateDelay)
        {
            // Transition back to the Idle state
            currentState = GameState.Idle;
            stateStartTime = Time.time;
            ResetGame();
        }
    }

    private void UpdatecomputerturnState()
    {
        ComputerGame();
        if (Time.time - stateStartTime >= nextStateDelay)
        {
            currentState = GameState.Playing;
            stateStartTime = Time.time;
        }
    }

    private void UpdatePlayingState()
    {
        if (Time.time - stateStartTime >= 5f)
        {
            Frame frame = leapProvider.CurrentFrame;
            if (frame.Hands.Count != 0)
            {
                Hand hand = frame.Hands[0];
                fingers = hand.Fingers.ToArray();
                isExtendedFingers = fingers.Select(finger => finger.IsExtended).ToArray();
                Debug.Log(isExtendedFingers[0] + "," + isExtendedFingers[1] + "," + isExtendedFingers[2] + "," + isExtendedFingers[3] + "," + isExtendedFingers[4]);

                int extendedFingerCount = isExtendedFingers.Count(isExtended => isExtended);

                if (extendedFingerCount == 0)
                {
                    playerChoice = RSP.Rock;
                    //resultText.text = "Rock";
                }
                else if (extendedFingerCount < 4)
                {
                    playerChoice = RSP.Scissors;
                    //resultText.text = "Scissors";
                }
                else
                {
                    playerChoice = RSP.Paper;
                    //resultText.text = "Paper";
                }

                PlayGame();
            }
            else
            {
                resultText.text = "Waiting for your turn...";

                // Transition to the Finished state
                //currentState = GameState.Finished;
                //stateStartTime = Time.time;
            }
        }
        
    }

    private void UpdateFinishedState()
    {
        if (Time.time - stateStartTime >= nextStateDelay)
        {
            // Transition back to the Idle state
            currentState = GameState.Idle;
            ResetGame();
            stateStartTime = Time.time;
        }
    }

    private void ComputerGame()
    {
        
        computerChoice = GetComputerChoice();
        resultText.text = "Computer select: " + GetChoiceText(computerChoice);

        StartCoroutine(ComputerAnimationCoroutine(computerChoice));
       
        //AnimateEnemyHand();

        // Generate computer hand object based on computer's choice
        //GameObject computerHand = Instantiate(computerHandObject, transform.position, transform.rotation);
        // Optionally, you can animate the computer's hand based on the computer's choice
    }

    private IEnumerator ComputerAnimationCoroutine(RSP choice)
    {
        yield return new WaitForSeconds(0.3f);  // Adjust the delay time as needed

        // Set computer's animation triggers
        switch (choice)
        {
            case RSP.Rock:
                handAnimator.SetBool("zyanken", true);
                handAnimator.SetBool("RockGestureTrigger", true);
                handAnimator.SetBool("ScissorsGestureTrigger", false);
                handAnimator.SetBool("PaperGestureTrigger", false);
                break;

            case RSP.Scissors:
                handAnimator.SetBool("zyanken", true);
                handAnimator.SetBool("RockGestureTrigger", false);
                handAnimator.SetBool("ScissorsGestureTrigger", true);
                handAnimator.SetBool("PaperGestureTrigger", false);
                break;

            case RSP.Paper:
                handAnimator.SetBool("zyanken", true);
                handAnimator.SetBool("RockGestureTrigger", false);
                handAnimator.SetBool("ScissorsGestureTrigger", false);
                handAnimator.SetBool("PaperGestureTrigger", true);
                break;
                
        }
    }

    private void SetRandomGameMode()
    {
        //Array values = Enum.GetValues(typeof(GameMode));
        currentMode = GetGameMode();
        gameModeText.text = "Game Mode: \n" + GetGameModeText(currentMode);
    }


    private void PlayGame()
    {
        resultText.text += "\nYour select: " + GetChoiceText(playerChoice);

        // Determine the winner based on the game mode
        switch (currentMode)
        {
            case GameMode.Win:
                // Logic for the "Player must win" mode
                if (playerChoice == computerChoice)
                {
                    resultText.text += "\nIncorrect";
                    scoreb1++;
                }
                else if (
                    (playerChoice == RSP.Rock && computerChoice == RSP.Scissors) ||
                    (playerChoice == RSP.Scissors && computerChoice == RSP.Paper) ||
                    (playerChoice == RSP.Paper && computerChoice == RSP.Rock)
                )
                {
                    resultText.text += "\nCorrect";
                    scorea1++;
                }
                else
                {
                    resultText.text += "\nIncorrect";
                    scoreb1++;
                }
                break;

            case GameMode.Tie:
                // Logic for the "Player must tie" mode
                if (playerChoice == computerChoice)
                {
                    resultText.text += "\nCorrect";
                    scorea1++;
                }
                else
                {
                    resultText.text += "\nIncorrect";
                    scoreb1++;
                }
                break;

            case GameMode.Lose:
                // Logic for the "Player must lose" mode
                if (playerChoice == computerChoice)
                {
                    resultText.text += "\nIncorrect";
                    scoreb1++;
                }
                else if (
                    (playerChoice == RSP.Rock && computerChoice == RSP.Scissors) ||
                    (playerChoice == RSP.Scissors && computerChoice == RSP.Paper) ||
                    (playerChoice == RSP.Paper && computerChoice == RSP.Rock)
                )
                {
                    resultText.text += "\nIncorrect";
                    scoreb1++;
                }
                else
                {
                    resultText.text += "\nCorrect";
                    scorea1++;
                }
                break;
        }

        currentState = GameState.Finished;
        stateStartTime = Time.time;
    }

    private RSP GetComputerChoice()
    {
        Array values = Enum.GetValues(typeof(RSP));
        return (RSP)values.GetValue(UnityEngine.Random.Range(0, values.Length));
    }

    private GameMode GetGameMode()
    {
        Array values = Enum.GetValues(typeof(GameMode));
        return (GameMode)values.GetValue(UnityEngine.Random.Range(0, values.Length));
    }

    private string GetGameModeText(GameMode choice)
    {
        switch (choice)
        {
            case GameMode.Win:
                return "Please win";

            case GameMode.Tie:
                return "Please draw";

            case GameMode.Lose:
                return "Please lose";

            default:
                return "";
        }
    }


    private string GetChoiceText(RSP choice)
    {
        switch (choice)
        {
            case RSP.Rock:
                return "Rock";

            case RSP.Scissors:
                return "Scissors";

            case RSP.Paper:
                return "Paper";

            default:
                return "";
        }
    }

    private void ResetGame()
    {
        playerChoice = RSP.Rock;
        computerChoice = RSP.Rock;
        resultText.text = "";
        handAnimator.SetBool("zyanken", false);
        handAnimator.SetBool("RockGestureTrigger", false);
        handAnimator.SetBool("ScissorsGestureTrigger", false);
        handAnimator.SetBool("PaperGestureTrigger", false);

        SceneManager.LoadScene("WalkThroughb");

    }

    public static int getscorea1()
    {
        return scorea1;
    }

    public static int getscoreb1()
    {
        return scoreb1;
    }
}
