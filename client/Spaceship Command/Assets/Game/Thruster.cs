using UnityEngine;
using System.Collections;
using System;

public class Thruster : MonoBehaviour 
{
    //Set through Unity
    public float POWER;

    public Rigidbody2D ApplyForceOn;

    public ParticleSystem Particles;
    //

    private bool isActive = false;
    public bool IsActive
    {
        get
        {
            return this.isActive;
        }

        set
        {
            if (this.isActive == value)
                return;

            this.isActive = value;

            this.Particles.gameObject.SetActive(value);
        }

    }

	// Use this for initialization
	void Start () 
    {
        Debug.Log(this.transform.forward);
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
	    if (this.isActive )
        {
            var forward = this.transform.up * POWER;
            this.ApplyForceOn.AddForce(forward);
        }
	}
}
