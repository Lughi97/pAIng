using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class PaddleAgent : Agent
{
    public Transform diskTransform;
    
    public float speed = 10f;
    public Vector3 startPosition;
    public bool resetFirstHit = true;
    public float proximityThreshold = 0.5f;


    
    void Start()
    {
        startPosition = transform.position;
    }
    public override void OnEpisodeBegin()
    {
        transform.position = startPosition;
        RestDisk();
    }
    /*get the just the x and z position and velocity of the disk in order to match the paddle during training*/
    public override void CollectObservations(VectorSensor sensor)
    {
        // disk & paddle position
        sensor.AddObservation(transform.position.z);
        sensor.AddObservation(diskTransform.position.x);
        sensor.AddObservation(diskTransform.position.z);
        //disk speed
        sensor.AddObservation(diskTransform.GetComponent<Rigidbody>().velocity.x);
        sensor.AddObservation(diskTransform.GetComponent<Rigidbody>().velocity.z);

    }
    // reset disk position calculate a new direction
    private void RestDisk()
    {
       
        diskTransform.GetComponent<Disk>().RestDisk();
    }

    // Set up the agent behavior and 
    public override void OnActionReceived(ActionBuffers actions)
    {
       ;
        //move in z direction
        float moveZ = actions.ContinuousActions[0];
        transform.position += new Vector3(0, 0, moveZ) * Time.deltaTime * speed;
       
        // 
        float zDistanceToDistk = Mathf.Abs(diskTransform.position.z- transform.position.z);
        if(zDistanceToDistk <= proximityThreshold && Mathf.Abs(diskTransform.position.x - transform.position.x)<5)
        {
            SetReward(0.1f);
        }
    }
    private void Update()
    {
        // disk goes out of bounds.
        if (diskTransform.GetComponent<Disk>().OutOfBounds|| Vector3.Distance(transform.position, diskTransform.position) > 13f)
        {
          
            SetReward(-1f);
           // RestDisk();
            EndEpisode();
        }
    
    }
   
    //Check if the paddle manages to hit the dik
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Disk")
        {
            SetReward(1f);
           
        }
    }

}
