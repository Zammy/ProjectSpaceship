using UnityEngine;
using System.Collections;

public class InputScript : MonoBehaviour 
{

    //Set through Unity
    public Thruster Front;
    public Thruster BackLeft;
    public Thruster BackRight;
    //

	
    void Update()
    {
        if (this.Front == null)
            return;

        this.Front.IsActive = Input.GetKey(KeyCode.W);
        this.BackLeft.IsActive = Input.GetKey(KeyCode.A);
        this.BackRight.IsActive = Input.GetKey(KeyCode.D);
    }
}
