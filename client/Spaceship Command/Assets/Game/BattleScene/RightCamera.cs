using UnityEngine;
using System.Collections;

public class RightCamera : MonoBehaviour 
{
    public Camera SecurityShip;
    public Camera PiratesShip;

	void Awake()
    {
        if (Lobby.Allegiance == Allegiance.Security)
        {
            this.PiratesShip.enabled = false;
        }
        else
        {
            this.SecurityShip.enabled = false;
        }
    }
}
