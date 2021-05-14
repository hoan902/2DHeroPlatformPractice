using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    float length, startposX;
    GameObject cam;
    [SerializeField]
    float ParallaxEffectPoint;
    // Start is called before the first frame update
    void Start()
    {
        startposX = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        //Debug.Log(cam.transform.position);

        float temp = (cam.transform.position.x * (1 - ParallaxEffectPoint));
        float dist = (cam.transform.position.x * ParallaxEffectPoint);

        transform.position = new Vector3(startposX + dist, transform.position.y, transform.position.z);

        if(temp > startposX + length)
        {
            startposX += length;
        }
        else if(temp < startposX - length)
        {
            startposX -= length;
        }
    }
}
