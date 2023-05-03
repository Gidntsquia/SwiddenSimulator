using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmingSquareManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        print("wassup");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other) {
        print("yo");
    }
}
