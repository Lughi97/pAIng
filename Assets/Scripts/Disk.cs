using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disk : MonoBehaviour
{
    public float speed = 15;
    public Vector3 direction;
    private Rigidbody rb;
    public bool OutOfBounds;
    private Vector3 startPosition;
  

    //Start the disk movement at a random direction.
    void Start()
    {
        
        rb = GetComponent<Rigidbody>();
        int startRandomDirection = Random.Range(0, 360);
        direction = new Vector3(Mathf.Cos(startRandomDirection * Mathf.Deg2Rad), 0f, Mathf.Sin(startRandomDirection * Mathf.Deg2Rad));
        startPosition = transform.position;
        //rb.velocity = rb.velocity.normalized * speed;
        rb.velocity = direction * speed;
    }

    private void SpeedCheck()
    {
        if (rb.velocity.magnitude < speed)
        {
            rb.velocity = direction * speed;
        }
    }
    // check the collision between the wall or the paddle.
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {

            BounceOffWall(collision.contacts[0].normal);
        }
        if (collision.gameObject.tag == "Paddle")
        {
            BounceOffPaddle(collision.contacts[0].normal);
        }
    }
    // calculate the new outgoing direction based on the normal of the collision contact with the wall.
    private void BounceOffWall(Vector3 normal)
    {
        Vector3 incomingDirection = direction;
        direction = Vector3.Reflect(direction, normal);
        float incomingAngle = Mathf.Atan2(incomingDirection.z, incomingDirection.x) * Mathf.Rad2Deg;
        // Calculate the new outgoing direction of the disk.
        float randomDeflectAngle = Random.Range(-5f, 5f) * Mathf.Deg2Rad;
        // use the 2d rotation matrix.
        float directionX = direction.x * Mathf.Cos(randomDeflectAngle) - direction.z * Mathf.Sin(randomDeflectAngle);
        float directionZ = direction.x * Mathf.Sin(randomDeflectAngle) + direction.z * Mathf.Cos(randomDeflectAngle);

        direction = new Vector3(directionX, 0f, directionZ).normalized;
        float outgoingAngle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
        //apply the new direction with the same speed in order keep energy.
        rb.velocity = direction * speed;
        SpeedCheck();
        // Log the incoming and outgoing angles
       // Debug.Log($"Incoming angle: {incomingAngle:F2} degrees");
       // Debug.Log($"Outgoing angle: {outgoingAngle:F2} degrees");
    }

    // calcualte the new outgoing direction based on the normal of the collision contact with the paddle.
    private void BounceOffPaddle(Vector3 normal)
    {
       
        direction = Vector3.Reflect(direction, normal).normalized;
        rb.velocity = direction * speed;
        SpeedCheck();

    }

    // Function called when the enviroment is reseted. The disk goes out of bounds.
    // is selected a new direction of the disk.
    public void RestDisk()
    {
        if (OutOfBounds)
            OutOfBounds = false;
        transform.position = startPosition;
        int startRandomDirection = Random.Range(0, 360);
        direction = new Vector3(Mathf.Cos(startRandomDirection * Mathf.Deg2Rad), 0f, Mathf.Sin(startRandomDirection * Mathf.Deg2Rad));
        rb.velocity = direction * speed;
    }

    // trigger that checks if the disk is out of bounds.
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bound" || other.gameObject.tag == "BoundPaddle")
        {
            OutOfBounds = true;
        }
    }
}
