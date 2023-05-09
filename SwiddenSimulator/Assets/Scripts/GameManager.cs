using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public FarmingSquareManager[] squares;
    public TMP_Text foodLabel;
    public GameObject deathObjects;
    private int[] counters;
    private FarmingSquareManager.State[] prevStates;

    [SerializeField]
    private float foodCount = 10.0f;

    private float farmYield = 3f;
    private float worseYield = 1f;
    private float foodEaten = 5f;
    [SerializeField]
    private int timeCount = 0;
    [SerializeField]
    private bool isDead = false;
    




    // This takes the game through one time step. It tracks food gain, food
    // eaten, sqaure changes due to time, and era changes.
    private void elapseTime()
    {
        int i = 0;
        foreach (FarmingSquareManager square in squares) {
            if (prevStates[i] != square.currState)
            {
                counters[i] = 0;
            }
            counters[i] += 1;
            switch (square.currState)
            {
                case FarmingSquareManager.State.Farm:
                    foodCount += farmYield;
                    if (counters[i] == 2)
                    {
                        // After two time steps, change to worse yield farm.
                        square.changeState(FarmingSquareManager.State.WorseYieldFarm);
                    }
                    
                    break;
                case FarmingSquareManager.State.WorseYieldFarm:
                    foodCount += worseYield;
                    break;
                case FarmingSquareManager.State.Grass:
                    if (counters[i] == 1) 
                    {
                        square.changeState(FarmingSquareManager.State.Forest);
                    }
                    break;
                case FarmingSquareManager.State.Burned:
                    if (counters[i] == 1) 
                    {
                        square.changeState(FarmingSquareManager.State.Forest);
                    }
                    break;
            }
            prevStates[i] = square.currState;
            i++;
        }
        foodCount -= foodEaten;
        foodLabel.SetText("" + foodCount);
        print("Food: " + foodCount);
        if (foodCount <= 0)
        {
            // YOU LOSE!
            isDead = true;
            StartCoroutine("playDeathSequence");


        }

        // Every time step increases this counter by one.
        timeCount++;
    }

    private IEnumerator playDeathSequence()
    {
        deathObjects.SetActive(true);

        yield return new WaitForSeconds(7.5f);

        SceneManager.LoadScene(0);
    }

    // Start is called before the first frame update
    void Start()
    {
        print("Starting GameManager");
        GameObject squaresParent = GameObject.Find("Squares");
        squares = squaresParent.GetComponentsInChildren<FarmingSquareManager>();

        counters = new int[squares.Length];
        prevStates = new FarmingSquareManager.State[squares.Length];
        int i = 0;
        for (i = 0; i < squares.Length; i++)
        {
            // Establish time counters as 0.
            counters[i] = 0;
            prevStates[i] = FarmingSquareManager.State.HouseTile;
        }

        foodLabel.SetText("" + foodCount);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isDead)
        {
            elapseTime();
        }   
    }
}
