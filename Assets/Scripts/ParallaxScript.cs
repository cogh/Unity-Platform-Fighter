using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        camera = GameObject.Find("Main Camera");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = camera.transform.position / parallaxFactor;
        transform.position = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);
        
    }

    GameObject camera;
    public float parallaxFactor;
}
