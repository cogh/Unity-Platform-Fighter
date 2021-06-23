using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 difference = player.transform.position + Vector3.up - transform.position;
        transform.position += new Vector3(difference.x, difference.y, 0.0f) / 10.0f;
    }

    GameObject player;
}
