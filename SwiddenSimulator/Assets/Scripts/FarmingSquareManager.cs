using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FarmingSquareManager : MonoBehaviour
{
    enum State {
        HouseTile,
        Forest, 
        Grass,
        Farm,
        Burned
    }

    State currState = State.Forest;

    // Start is called before the first frame update
    void Start()
    {
        print("Start " + this.name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (other.tag == "Player")
        {
            // This is basically my update.
            
            if (Input.GetKey(KeyCode.F)) {
                if (currState != State.Burned)
                {
                    print("Burn " + this.name);
                }
                currState = State.Burned;
            }
            else if (Input.GetMouseButton(0)) {
                if (currState != State.Farm)
                {
                    print("Till " + this.name);
                }
                currState = State.Farm;
            }
            else if (Input.GetMouseButton(1)) {
                if (currState != State.Grass)
                {
                    print("Destroy " + this.name);
                }
                currState = State.Grass;
            }

        }
    }

}
