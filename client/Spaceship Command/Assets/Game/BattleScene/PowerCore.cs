using UnityEngine;
using System.Collections;
using Networking;
using System.Collections.Generic;
using System;

public interface IEnergyConsumator
{
    double Consumption { get; }
    bool IsActive { get; }
}

public class PowerCore : MonoBehaviour, IMessageReceiver 
{
    Allegiance allegiance;

    public double energy;
    const double ENERGY_RECHARGE = 0.5f;

    public Thruster[] Thrusters;


	// Use this for initialization
	void Start () 
    {
        this.energy = 100;

        this.allegiance = this.GetComponent<AllegianceDesignator>().Allegiance;

        CoreNetwork.Instance.Subscribe(this);
    }

    void OnDestroy()
    {
        CoreNetwork.Instance.Unsubscribe(this);
    }
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        this.energy += ENERGY_RECHARGE * Time.fixedDeltaTime;

        double thrustersTotalConsumtion = 0;
        foreach (var thruster in this.Thrusters)
        {
            thrustersTotalConsumtion += thruster.Consumption * Time.fixedDeltaTime;
        }

        this.energy -= thrustersTotalConsumtion;

        if (this.energy < 0)
        {
            this.energy = 0;
            foreach (var thruster in this.Thrusters)
            {
                thruster.IsActive = false;
            }
        }

        if (this.energy > 100)
        {
            this.energy = 100;
        }
	}

    #region IMessageReceiver implementation

    public void ReceiveMsg(int connectionId, INetMsg msg)
    {
    }

    #endregion
}
