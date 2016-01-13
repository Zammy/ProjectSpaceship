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
    public static PlayerClient Instance { get; private set;}

    Dictionary<ThrusterType, Thruster> thrusters;

    void Awake()
    {
        Instance = this;

//        #if UNITY_EDITOR
//        this.ExtractThrusters();
//        #endif
    }

    void Start()
    {
        Debug.Log("PlayerClient Start()");
        Debug.Log("isServer " + this.isServer);
        Debug.Log("isClient " + this.isClient);
        Debug.Log("isLocalPlayer" + this.isLocalPlayer);

        if (!this.isClient)
        {
            this.ExtractThrusters();
        }
    }

    void ExtractThrusters()
    {
        Debug.Log("Thruster loading...");
        this.thrusters = new Dictionary<ThrusterType, Thruster>();
        Thruster[] thrustersArray = FindObjectsOfType<Thruster>();
        if (thrustersArray.Length == 0)
        {
            Debug.LogError("No thrusters found!");
        }
        foreach (var thruster in thrustersArray)
        {
            Debug.Log("Adding " + thruster.ThrusterType.ToString());
            this.thrusters.Add(thruster.ThrusterType, thruster);
        }
    }
   
    //Executed on client
    public void FireThruster(ThrusterType t)
    {
        this.thrusters[t].IsActive = true;
    }

    public void StopThruster(ThrusterType t)
    {
        this.thrusters[t].IsActive = false;
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
