using UnityEngine;
using System.Collections;

public class BackgroundScroller : MonoBehaviour 
{
    public Transform Transform;

    Material material;

    void Start()
    {
        this.material = this.GetComponent<Renderer>().material;
    }

	void FixedUpdate () 
    {
	    var offset = this.material.mainTextureOffset;

        offset.x = this.Transform.position.x * 0.04f;
        offset.y = this.Transform.position.y * 0.04f;

        this.material.mainTextureOffset = offset;

        this.transform.eulerAngles = Vector3.zero;

        this.transform.position = this.Transform.position;
	}
}
