using UnityEngine;
using System.Collections;

public class InputScript : MonoBehaviour 
{

    //Set through Unity
//    public Thruster FrontLeft;
//    public Thruster FrontRight;
    public Thruster Front;
    public Thruster BackLeft;
    public Thruster BackRight;
    //

	
    void Update()
    {
//        this.FrontLeft.IsActive = Input.GetKey(KeyCode.Q);
//        this.FrontRight.IsActive = Input.GetKey(KeyCode.E);
        this.Front.IsActive = Input.GetKey(KeyCode.W);
        this.BackLeft.IsActive = Input.GetKey(KeyCode.A);
        this.BackRight.IsActive = Input.GetKey(KeyCode.D);

//        if (this.Thrusters.Length == 0)
//        {
//            return;
//        }
//
//        this.Thrusters[0].IsActive = Input.GetKey(KeyCode.Q);
//
//        if (this.Thrusters.Length == 1)
//        {
//            return;
//        }
//
//        this.Thrusters[1].IsActive = Input.GetKey(KeyCode.W);
    }
}
