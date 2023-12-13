using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    Vector3 dollyDir;
    float smooth = 10;
    float distance;
    float minDistance = 1;
    float maxDistance = 4;
    // Start is called before the first frame update
    void Awake()
    {
        dollyDir = transform.position.normalized;
        distance = transform.position.magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 desiredPos = dollyDir * maxDistance;
        RaycastHit hit;

        if (Physics.Linecast(transform.position, desiredPos, out hit))
        {
            distance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        }
        else
            distance = maxDistance;
        transform.position = Vector3.Lerp(transform.position, dollyDir * distance, Time.deltaTime * smooth);
    }
}