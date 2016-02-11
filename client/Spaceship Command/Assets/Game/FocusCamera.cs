using UnityEngine;
using System.Collections;

public class FocusCamera : MonoBehaviour 
{
    public GameObject[] FocusOn;

    public Camera Camera;

    float targetOrtho;
    Vector3 targetPos;


    const float BUFFER = 1;
    const float orthoMovePerSec = 10f;
    const float posMovePerSec = 30f;


	// Use this for initialization
	void Start () 
    {
        this.Camera = this.GetComponent<Camera>();

        this.CalculateCameraSizeAndPos();
	}   

	void Update () 
    {
        this.CalculateCameraSizeAndPos();

        if (Mathf.Abs(this.Camera.orthographicSize - this.targetOrtho) > 0.1f)
        {
            float orthoMoveFrame = Time.deltaTime * orthoMovePerSec;
            float orthoDelta = this.targetOrtho - this.Camera.orthographicSize;
            if (Mathf.Abs(orthoDelta) < orthoMoveFrame)
            {
                orthoMoveFrame = orthoDelta;
            }

            if (orthoDelta > 0)
            {
                this.Camera.orthographicSize += orthoMoveFrame;
            }
            else
            {
                this.Camera.orthographicSize -= orthoMoveFrame;
            }
        }

        float distanceToMove = posMovePerSec * Time.deltaTime;
        Vector3 diff = this.targetPos - this.transform.position;
        if (diff.magnitude > 0.025f)
        {
            if (diff.magnitude < distanceToMove)
            {
                distanceToMove = diff.magnitude;
            }

            diff.Normalize();
            diff = diff * distanceToMove;
            this.transform.position += diff;
        }
	}   

    void CalculateCameraSizeAndPos()
    {
        float minHeight;
        float mediumY;
        float minWidth;
        float mediumX;
        CalculateMinHeight( FocusOn, out minHeight, out mediumY );
        CalculateMinWidth( FocusOn, out minWidth, out mediumX );

        float calcHeight = minHeight + BUFFER;
        float calcWidth = calcHeight * this.Camera.aspect;
        if (calcWidth < minWidth)
        {
            calcWidth = minWidth;
            calcHeight = calcWidth / this.Camera.aspect;
        }

        this.targetOrtho = calcHeight / 2;
        this.targetPos = new Vector3(mediumX, mediumY, this.Camera.transform.position.z);
    }

    void CalculateMinWidth(GameObject[] gos, out float minWidth, out float mediumX)
    {
        float minX = float.MaxValue;
        float maxX = float.MinValue;

        foreach(var go in gos)
        {
            float x = GetX(go, min: true);
            if (minX > x)
            {
                minX = x;
            }
            x = GetX(go, min: false);
            if (maxX < x)
            {
                maxX = x;
            }
        }

        minWidth = maxX - minX;
        mediumX = minX + minWidth / 2;
    }

    void CalculateMinHeight(GameObject[] gos, out float minHeight, out float mediumY)
    {
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        foreach (var go in gos)
        {
            float y = GetY(go, min:true);
            if (minY > y)
            {
                minY = y;
            }
            y = GetY(go, min:false);
            if (maxY < y)
            {
                maxY = y;
            }
        }

        minHeight = maxY - minY;
        mediumY = minY + minHeight / 2;
    }

    float GetX(GameObject go, bool min)
    {
        var sprite = go.GetComponent<SpriteRenderer>().sprite;
        float width = sprite.rect.width / sprite.pixelsPerUnit;
        width = width * go.transform.localScale.x;
        if (min) 
        {
            return go.transform.position.x - width/2;
        }
        else
        {
            return go.transform.position.x + width/2;
        }
    }

    float GetY(GameObject go, bool min)
    {
        var sprite = go.GetComponent<SpriteRenderer>().sprite;
        float height = sprite.rect.height / sprite.pixelsPerUnit;
        height = height * go.transform.localScale.y;
        if (min) 
        {
            return go.transform.position.y - height/2;
        }
        else
        {
            return go.transform.position.y + height/2;
        }
    }
}
