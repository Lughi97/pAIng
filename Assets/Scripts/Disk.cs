using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class handels:
/// 1- Collision handeling between disk and wall or paddle
/// </summary>
public class Disk : MonoBehaviour
{
    public float speed = 15;
    public Vector3 direction;
    public Rigidbody rb;
    public bool OutOfBounds;
    private Vector3 startPosition;

    private void Start()
    {
       startPosition = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            direction = Vector3.Reflect(direction, collision.contacts[0].normal);
            float randomDeflectAngle = Random.Range(-5f, 5f)*Mathf.Deg2Rad;
            // calculate the new outgoing direction.
            float directionX = direction.x * Mathf.Cos(randomDeflectAngle) - direction.z * Mathf.Sin(randomDeflectAngle);
            float directionZ = direction.x * Mathf.Sin(randomDeflectAngle) + direction.z * Mathf.Cos(randomDeflectAngle);
            direction = new Vector3(directionX, 0f, directionZ).normalized;
            rb.velocity = direction * speed;
            
        }

        if (collision.gameObject.tag == "Paddle")
        {
            direction = Vector3.Reflect(direction, collision.contacts[0].normal).normalized;
            rb.velocity = direction * speed;
        }
    }

    public void ResetDiskDirection()
    {
        if (OutOfBounds)
            OutOfBounds = false;
        transform.position = startPosition;
        int startRandomDirection = Random.Range(0, 360);
        direction = new Vector3(Mathf.Cos(startRandomDirection * Mathf.Deg2Rad), 0f, Mathf.Sin(startRandomDirection * Mathf.Deg2Rad));
        rb.velocity = direction * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bound")
        {
            OutOfBounds = true;
        }
    }
}
