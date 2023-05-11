using System.Collections;
using UnityEngine;

public class SampleMove2D : MonoBehaviour
{
    public float Distance = 0;

    private Movement2D mvnt = null;
    [SerializeField]
    private Animator farmerAnimator;

    private void Start()
    {
        mvnt = GetComponent<Movement2D>();
    }

    private void Update()
    {
        float movementX = Input.GetAxis("Horizontal") * Distance * Time.deltaTime;
        float movementY = Input.GetAxis("Vertical") * Distance * Time.deltaTime;
        mvnt.MoveAlongX(Input.GetAxis("Horizontal") * Distance * Time.deltaTime);
        mvnt.MoveAlongY(Input.GetAxis("Vertical") * Distance * Time.deltaTime);

        if (movementX != 0 || movementY != 0) 
        {
            farmerAnimator.SetBool("isWalking", true);
        }
        else
        {
            farmerAnimator.SetBool("isWalking", false);
        }

    }
}