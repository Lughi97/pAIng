using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

/// <summary>
/// Class handles:
/// 1- Collecting observation.
/// 2- Define the action needed to reach the goal
/// 3- Set positve or negative reward based on the taken action.
/// 4- Reset the Epsiode when the disk goes out of bounds or the Max setp
///    for the episode is reached.
/// </summary>
public class PaddleAgent : Agent
{
    public Transform diskTransform; 
    
    public float currentSpeed = 0f;
    public float minSpeed = 5f;
    public float maxSpeed = 25f; 
  
    public Vector3 startPosition;
    
    public float zDistanceThreshold = 0.5f; // minimal thershold z cooridnate  in order to get a reward.
    private float optimalSpeed = 2f; // speed ranges in which agent is rewared.

    
    void Start()
    {
        startPosition = transform.position;
    }

    public override void OnEpisodeBegin()
    {
        transform.position = startPosition;
        RestDisk();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
       // Collect position information for both disk and paddle.
        sensor.AddObservation(transform.position.z);
        sensor.AddObservation(diskTransform.position.x);
        sensor.AddObservation(diskTransform.position.z);

        // Collect speed information for both disk and paddle.
        sensor.AddObservation(diskTransform.GetComponent<Rigidbody>().velocity.x);
        sensor.AddObservation(diskTransform.GetComponent<Rigidbody>().velocity.z);
        sensor.AddObservation(currentSpeed);

    }

    private void RestDisk()
    { 
        diskTransform.GetComponent<Disk>().ResetDiskDirection();
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveZ = actions.ContinuousActions[0];
        currentSpeed = Mathf.Clamp(Mathf.Clamp(actions.ContinuousActions[1], 0, 1) * maxSpeed, minSpeed, maxSpeed);

        transform.position += new Vector3(0, 0, moveZ) * currentSpeed * Time.deltaTime;


        float zDistanceToDisk = Mathf.Abs(diskTransform.position.z - transform.position.z);
      
        float speedVariance = Mathf.Abs(diskTransform.GetComponent<Rigidbody>().velocity.magnitude - currentSpeed);
 

        if (zDistanceToDisk <= zDistanceThreshold && Vector3.Distance(diskTransform.position, transform.position)<5)
            SetReward(0.1f); // the agent is close enough to the disk

        if (speedVariance <= optimalSpeed)
            SetReward(0.5f); // the agent almost the same speed of the disk

    }

    private void Update()
    {
        if (diskTransform.GetComponent<Disk>().OutOfBounds || Vector3.Distance(transform.position, diskTransform.position) > 13f)
        {
            SetReward(-1f); // penilize the agent when the disk goes out of bounds.
            EndEpisode();
        }
    
    }
   
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Disk")
        {
            SetReward(1f); // reard the agent when it hits the disk
           
        }
    }


}
