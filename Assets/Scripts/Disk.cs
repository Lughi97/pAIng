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
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        int startRandomDirection = Random.Range(0, 360);
        direction = new Vector3(Mathf.Cos(startRandomDirection), 0f, Mathf.Sin(startRandomDirection));
        direction = Random.onUnitSphere;
        direction.y = 0;
        startPosition = transform.position;
        rb.velocity = direction * speed;
    }

    private void Update()
    {
      //  Debug.Log(rb.velocity.magnitude);
    }
    private void FixedUpdate()
    {
        rb.velocity = direction * speed;
        if (rb.velocity.magnitude != 15)
        {
            rb.velocity = direction * speed;
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        ////  Debug.log("COLLSIION");
        if (collision.gameObject.tag == "Wall")
        {
            BounceWall(collision.contacts[0].normal);
        }
        if (collision.gameObject.tag == "Paddle")
        {
            BounceOffPaddle(collision.contacts[0].normal);
        }
    }

    private void BounceWall(Vector3 normal)
    {
        direction = Vector3.Reflect(direction, normal);
        float changeAngleRate = Random.Range(-5f, 5f);
        Quaternion rotation = Quaternion.AngleAxis(changeAngleRate, Vector3.up);
        direction = rotation * direction;
        direction = direction.normalized;
        float angle = Vector3.Angle(direction, Vector3.right);
        rb.velocity = direction * speed;

        // Determine the quadrant based on the direction vector


        //  Debug.log("Angle: " + angle + " degrees");
    }
    private void BounceOffPaddle(Vector3 normal)
    {
       
        direction = Vector3.Reflect(direction, normal).normalized;
        rb.velocity = direction * speed;

    }

    public void RestDisk()
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
        if (other.gameObject.tag == "Bound" || other.gameObject.tag == "BoundPaddle")
        {
            OutOfBounds = true;
        }
    }
}
