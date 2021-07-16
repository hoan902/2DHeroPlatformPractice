using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    float lengthX, lengthY, startposX, startposY;
    GameObject cam;
    [SerializeField]
    float ParallaxEffectPointX;
    [SerializeField]
    float ParallaxEffectPointY;
    // Start is called before the first frame update
    void Start()
    {
        startposX = transform.position.x;
        startposY = transform.position.y;
        lengthX = GetComponent<SpriteRenderer>().bounds.size.x;
        lengthY = GetComponent<SpriteRenderer>().bounds.size.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        //Debug.Log(cam.transform.position);

        float tempX = (cam.transform.position.x * (1 - ParallaxEffectPointX));
        float distX = (cam.transform.position.x * ParallaxEffectPointX);

        float tempY = (cam.transform.position.y * (1 - ParallaxEffectPointY));
        float distY = (cam.transform.position.y * ParallaxEffectPointY);

        transform.position = new Vector3(startposX + distX, startposY + distY, transform.position.z);

        if(tempX > startposX + lengthX)
        {
            startposX += lengthX;
        }
        else if(tempX < startposX - lengthX)
        {
            startposX -= lengthX;
        }
        
        if(tempY > startposY + lengthY)
        {
            startposY += lengthY;
        }
        else if(tempY < startposY - lengthY)
        {
            startposY -= lengthY;
        }
    }
}
