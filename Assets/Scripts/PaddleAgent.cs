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


    // Start is called before the first frame update
    void Start()
    {
      //  RestDisk();
       
        startPosition = transform.position;
    }
    public override void OnEpisodeBegin()
    {
        //Debug.Log("BEGIN EPSIODE");
        transform.position = startPosition;
        RestDisk();
        // Debug.Log("Position" + transform.position);
    }
    /*get the just the x and z position and velocity of the disk in order to match the paddle during training*/
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position.z);
        sensor.AddObservation(diskTransform.position.x);
        sensor.AddObservation(diskTransform.position.z);
        sensor.AddObservation(diskTransform.GetComponent<Rigidbody>().velocity.x);
        sensor.AddObservation(diskTransform.GetComponent<Rigidbody>().velocity.z);

    }
    private void RestDisk()
    {
       
        diskTransform.GetComponent<Disk>().RestDisk();
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Debug.Log(actions.ContinuousActions[0]);
        //move in vertical direction
        float moveZ = actions.ContinuousActions[0];
        transform.position += new Vector3(0, 0, moveZ) * Time.deltaTime * speed;
        //Debug.Log(zPaddlePosition);
       // transform.position = new Vector3(transform.position.x, transform.position.y, zPaddlePosition);
        float zDistanceToDistk = Mathf.Abs(diskTransform.position.z- transform.position.z);
        if(zDistanceToDistk <= proximityThreshold && Mathf.Abs(diskTransform.position.x - transform.position.x)<5)
        {
            SetReward(0.1f);
        }
    }
    private void Update()
    {
        if (diskTransform.GetComponent<Disk>().OutOfBounds|| Vector3.Distance(transform.position, diskTransform.position) > 13f)
        {
            //Debug.Log("BALL OUT");
            SetReward(-1f);
            RestDisk();
            //  transform.position = startPosition;

            EndEpisode();
        }
    
    }
   /* private bool CanSeeDisk()
    {
        float sphereRadious = 1f;
        Vector3 directionToDisk = diskTransform.position - transform.position;
        float distanceToDisk = directionToDisk.magnitude;
        if (Physics.SphereCast(transform.position, sphereRadious, directionToDisk.normalized, out RaycastHit hit, 5f))
        {
            if (hit.collider.tag == "Disk")
            {
                //Debug.Log("SEE");
                return true;
            }
        }
        //Debug.Log("To Faar");
        return false;
    }*/
    private void OnCollisionEnter(Collision collision)
    {


        if(collision.gameObject.tag == "Disk")
        {

            // Debug.Log("HIT BALL");
            SetReward(1f);
            if (resetFirstHit == true)
            {
                RestDisk();
                EndEpisode();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.tag == "BoundPaddle")
        {
            
            SetReward(-1f);
            RestDisk();
            EndEpisode();
          
            
        }
      
    
    }


}
