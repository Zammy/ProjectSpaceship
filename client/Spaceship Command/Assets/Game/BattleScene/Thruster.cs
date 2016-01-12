using UnityEngine;
using System.Collections;
using System;

public class Thruster : MonoBehaviour 
{
    //Set through Unity
    public float POWER;

    public ThrusterType ThrusterType;

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


//            this.Particles.emission.enabled = value;

            if (value)
            {
//                this.Particles.emission.enabled = value;
                this.Particles.Play();
            }
            else
            {
                this.Particles.Stop();
            }
        }

    }

	// Use this for initialization
	void Start () 
    {
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
	    if (this.isActive )
        {
            var forward = this.transform.up * POWER;
            this.ApplyForceOn.AddForce(forward); //, this.transform.position);
        }
	}
}
