using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{

    public Vector3[] points;
    public int pointNumber = 0;
    private Vector3 currentTarget;

    public float minDistance;
    public float speed;
    public float waitTime;
    public float waitStart;
    public Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        if (points.Length > 0)
        {
            currentTarget = points[0];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position != currentTarget)
        {
            Move();
        }
        else
        {
            ChangeTarget();
        }
          
        
    }

    private void Move()
    {
        velocity = currentTarget - transform.position;
        transform.position += (velocity / velocity.magnitude) * speed * Time.deltaTime;
        
        if (velocity.magnitude < minDistance)
        {
            transform.position = currentTarget;
            waitStart = Time.time;

            if (Time.time == waitStart + waitTime)
            {
                ChangeTarget();
            }
        }
    }

    public void ChangeTarget()
    {
        pointNumber++;
        if(pointNumber >= points.Length)
        {
            pointNumber = 0;
        }

        currentTarget = points[pointNumber];
    }

    private void OnTriggerEnter(Collider other)
    {
        other.transform.parent = transform;
    }

    private void OnTriggerExit(Collider other)
    {
        other.transform.parent = null;
    }
}
