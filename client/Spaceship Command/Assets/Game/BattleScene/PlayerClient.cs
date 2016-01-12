using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public enum ThrusterType
{
    MainLeft,
    MainRight,
    Frontal,
    ControlLeft,
    ControlRight
}

public class PlayerClient : NetworkBehaviour 
{
    Dictionary<ThrusterType, Thruster> thrusters;

	// Use this for initialization
	void Start () 
    {
        if (this.isServer)
        {
            this.thrusters = new Dictionary<ThrusterType, Thruster>();

            Thruster[] thrustersArray = FindObjectsOfType<Thruster>();

            foreach(var thruster in thrustersArray)
            {
                this.thrusters.Add( thruster.ThrusterType, thruster);
            }
        }
	}
   
    //Executed on client
    public void ThrusterActivated(ThrusterType t)
    {
        this.CmdFireThruster(t);
    }

    public void ThrusterDeactivated(ThrusterType t)
    {
        this.CmdStopThruster(t);
    }
    //

    [Command]
    public void CmdFireThruster(ThrusterType t)
    {
        Debug.Log("CmdFireThruster " + t.ToString());

        this.thrusters[t].IsActive = true;
    }

    [Command]
    public void CmdStopThruster(ThrusterType t)
    {
        Debug.Log("CmdStopThruster " + t.ToString());

        this.thrusters[t].IsActive = false;
    }
}
