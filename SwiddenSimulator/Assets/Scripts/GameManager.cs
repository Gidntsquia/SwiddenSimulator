using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private FarmingSquareManager[] squares;
    [SerializeField]
    private TMP_Text foodLabel;
    [SerializeField]
    private GameObject deathObjects;
    [SerializeField]
    private GameObject sleepObjects;
    [SerializeField]
    private TMP_Text dayLabel;
    [SerializeField]
    private TMP_Text lossLabel;
    [SerializeField]
    private TMP_Text gainLabel;
    [SerializeField]
    private TMP_Text eventLabel;
    
    [SerializeField]
    private GameObject[] protectedForestSquares;
    [SerializeField]
    private Transform player;
    private int[] counters;
    private FarmingSquareManager.State[] prevStates;

    [SerializeField]
    private float foodCount = 10.0f;

    private float farmYield = 3f;
    private float worseYield = 1f;
    private float foodEaten = 5f;
    private float tax = 0f;
    private float swiddenTax = 0f;

    [SerializeField]
    private int timeCount = 1;
    [SerializeField]
    private bool isDead = false;
    private bool isInBetweenDays = false;
    private float foodTaken = 0f;





    // This takes the game through one time step. It tracks food gain, food
    // eaten, sqaure changes due to time, and era changes.
    private void elapseTime()
    {
        int i;
        float foodGained = 0f;
        float foodLost = 0f;
        // Set Day
        dayLabel.SetText("Day " + timeCount);
        gainLabel.SetText("");
        lossLabel.SetText("");
        eventLabel.SetText("");
        switch (timeCount)
        {
            case 6:
                eventLabel.SetText("A colonial government has taken charge!");
                tax = 0.1f;
                break;
            case 7:
                for (i = 0; i < 2; i++)
                {
                    // First french colonial impact -- classify bottom left two forest
                    // tiles as "protected" if they are forest tiles.
                    if (squares[i].currState == FarmingSquareManager.State.Forest
                        || squares[i].currState == FarmingSquareManager.State.Grass)
                    {
                        protectedForestSquares[i].SetActive(true);
                    }
                }
                eventLabel.SetText("The colonial government marked some forest areas as \"protected\"!");
                break;
            case 9:
                eventLabel.SetText("The colonial government banned swidden! You will be punished if you do it.");
                // Second french colonial impact -- punishment on swidden on swidden
                swiddenTax = 5.0f;
                break;
            
            case 11:
                // Increase tax

                 eventLabel.SetText("The colonial government increased taxes!");
                tax = 0.15f;
                break;

            case 13:
                eventLabel.SetText("War has broken loose! There is no one to regulate swidden, so it is allowed again.");
                // Revolutionary / Japanese war breaks out, food seized
                foodCount -= 13f;
                foodLabel.SetText("" + foodCount);
                lossLabel.text = lossLabel.text + "-" + (13) + " lost in war\n";
                foodLost += 13;
                tax = 0f;

                swiddenTax = 0.0f;

                for (i = 0; i < 2; i++)
                {
                    // French rule ends, so remove their classifications
                    protectedForestSquares[i].SetActive(false);
                }
                break;
            
            case 20:
                eventLabel.SetText("A new revolutionary government ousted the colonial government!");
                tax = 0.1f;
                break;

            case 21:

                // int count = 0;
                // foreach (FarmingSquareManager square in squares)
                // {
                //     if (square.currState == FarmingSquareManager.State.WorseYieldFarm
                //         || square.currState == FarmingSquareManager.State.Farm) 
                //     {
                //         count += 1;
                //     }
                // }
                // if (count >= 7)
                // {
                //     eventLabel.SetText("You have too much farm land, some of it will be seized!");
                // }
                eventLabel.SetText("The revolutionary government seized your food for redistribution!");
                foodTaken = Mathf.Floor(foodCount / 2);
                foodCount -= foodTaken;
                foodLost += foodTaken;

                lossLabel.text = lossLabel.text + "-" + (foodTaken) + " taken by gov't\n";


                break;
                
            case 23:
                eventLabel.SetText("The revolutionary government banned swidden!");
                swiddenTax = 2.0f;
                break;
            case 25:
                eventLabel.SetText("Food has been redistributed to you!");
                foodCount += foodTaken;
                gainLabel.text = gainLabel.text + "+" + foodTaken + " food redistributed\n";
                foodGained += foodTaken;

                foodTaken = 0f;
                
                break;

        }
        
        i = 0;
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
                    gainLabel.text = gainLabel.text + "+" + farmYield + " Farm\n";
                    foodGained += farmYield;

                    if (counters[i] == 2)
                    {
                        // After two time steps, change to worse yield farm.
                        square.changeState(FarmingSquareManager.State.WorseYieldFarm);
                    }
                    
                    break;
                case FarmingSquareManager.State.WorseYieldFarm:
                    foodCount += worseYield;
                    gainLabel.text = gainLabel.text + "+" + worseYield + " Farm\n";
                    foodGained += worseYield;
                    break;
                case FarmingSquareManager.State.Grass:
                    if (counters[i] == 2) 
                    {
                        square.changeState(FarmingSquareManager.State.Forest);
                    }
                    break;
                case FarmingSquareManager.State.Burned:
                    if (counters[i] == 2) 
                    {
                        square.changeState(FarmingSquareManager.State.Forest);
                    }
                    break;
            }
            prevStates[i] = square.currState;
            i++;
        }
        if (tax > 0)
        {
            float postTaxAmount = Mathf.Floor(foodCount * (1 - tax));
            lossLabel.text = lossLabel.text + "-" + (foodCount - postTaxAmount) + " tax\n";
            foodLost += foodCount - postTaxAmount;
            foodCount = postTaxAmount;
        }



        foodCount -= foodEaten;
        lossLabel.text = lossLabel.text + "-" + (foodEaten) + " eaten\n";
        foodLost += foodEaten;

        foodLabel.SetText("" + foodCount);
        print("Food: " + foodCount);

        float netFood = foodGained - foodLost;
        if (netFood > 0) 
        {
            dayLabel.text = dayLabel.text + ": +" + netFood;
        }
        else
        {
            dayLabel.text = dayLabel.text + ": " + netFood;
        }

        if (foodCount <= 0)
        {
            // YOU LOSE!
            isDead = true;
            sleepObjects.SetActive(false);
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

    private IEnumerator goToNextDay() 
    {
        isInBetweenDays = true;
        sleepObjects.SetActive(true);
        elapseTime();
        yield return new WaitForSeconds(1.5f);
        while (!Input.anyKeyDown)
        {
            yield return null;
        }
        sleepObjects.SetActive(false);
        isInBetweenDays = false;

    }

    private IEnumerator startRestart()
    {

        yield return new WaitForSeconds(2.0f);
        if (Input.GetKey(KeyCode.R)) 
        {
            SceneManager.LoadScene(0);
        }
    }

    public void applySwiddenTax()
    {
        print("TAX!");
        foodCount -= swiddenTax;
        foodLabel.SetText("" + foodCount);
        if (foodCount <= 0)
        {
            // YOU LOSE!
            isDead = true;
            sleepObjects.SetActive(false);
            StartCoroutine("playDeathSequence");
        }
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
        if (Input.GetKeyDown(KeyCode.Space) && !isDead && !isInBetweenDays)
        {
            StartCoroutine("goToNextDay");
            player.position = new Vector3(7.5f, 12f, 0f);
        }   
        else if (Input.GetKeyDown(KeyCode.R)) 
        {
            StartCoroutine("startRestart");
        }
        
    }
}
