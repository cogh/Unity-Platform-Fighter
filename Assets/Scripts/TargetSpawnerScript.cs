using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawnerScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Vector3 targetPosition = targetPositions[Random.Range(0, targetPositions.Count - 1)].transform.position;
        currentTarget = Instantiate(targetPrefab, targetPosition, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        /*
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = timerMax;
            Vector3 targetPosition = targetPositions[Random.Range(0, targetPositions.Count - 1)].transform.position;
            Instantiate(targetPrefab, targetPosition, Quaternion.identity);
        }
        */

        if (currentTarget == null)
        {
            Vector3 targetPosition = targetPositions[Random.Range(0, targetPositions.Count - 1)].transform.position;
            currentTarget = Instantiate(targetPrefab, targetPosition, Quaternion.identity);
        }
    }

    public List<GameObject> targetPositions;
    public GameObject targetPrefab;
    public GameObject currentTarget;
    float timer;
    public float timerMax;
}
