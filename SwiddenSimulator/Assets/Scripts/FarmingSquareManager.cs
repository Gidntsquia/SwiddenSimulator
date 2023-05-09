using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FarmingSquareManager : MonoBehaviour
{
    public enum State {
        HouseTile,
        Forest, 
        Grass,
        Farm,
        Burned,
        WorseYieldFarm
    }

    public State currState = State.Forest;
    public SpriteRenderer mySprite;
    public Sprite[] sprites;
    private bool containsPlayer = false;
    private Dictionary<State, int> stateMap = new Dictionary<State, int>();

    // Start is called before the first frame update
    void Start()
    {
        stateMap.Add(State.HouseTile, 0);
        stateMap.Add(State.Forest, 1);
        stateMap.Add(State.Grass, 2);
        stateMap.Add(State.Farm, 3);
        stateMap.Add(State.Burned, 4);
        stateMap.Add(State.WorseYieldFarm, 5);

    }

    // Update is called once per frame
    void Update()
    {
        if (containsPlayer)
        {    
            // Only burn if there is a forest.
            if (Input.GetKey(KeyCode.F) && currState == State.Forest) 
            {
                print("Burn " + this.name);
                currState = State.Burned;
             //   mySprite.
            }
            
            else if (Input.GetMouseButton(0) && currState == State.Burned) 
            {
                print("Till " + this.name);
                currState = State.Farm;
            }
            else if (Input.GetMouseButton(0) && currState == State.Grass)
            {
                print("Till, but it's used up :( -- " + this.name);
                currState = State.WorseYieldFarm;
            }
            else if (Input.GetMouseButton(1) && (currState == State.HouseTile 
                                            || currState == State.WorseYieldFarm
                                            || currState == State.Farm
                                            || currState == State.Burned)) 
            {
                print("Destroy " + this.name);
                currState = State.Grass;
            }

            mySprite.sprite = sprites[stateMap[currState]];

        }

    }

    public void changeState(State state) 
    {
        currState = state;
        mySprite.sprite = sprites[stateMap[currState]];
    }
    
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag == "Player")
        {
            containsPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player")
        {
            containsPlayer = false;
        }
    }


}
