using UnityEngine;
using System.Collections;

public class BackgroundScroller : MonoBehaviour 
{
    Material material;
    Transform cameraTransform;

    void Start()
    {
        this.material = this.GetComponent<Renderer>().material;
        this.cameraTransform = this.transform.parent;
    }

	void Update () 
    {
	    var offset = this.material.mainTextureOffset;

        offset.x = this.cameraTransform.position.x * 0.04f;
        offset.y = this.cameraTransform.position.y * 0.04f;

        this.material.mainTextureOffset = offset;

        this.transform.eulerAngles = Vector3.zero;
	}
}
