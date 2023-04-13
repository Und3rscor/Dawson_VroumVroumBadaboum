using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{

    [SerializeField] private Rigidbody sphereRb;
    [SerializeField] private float forwardAccel, reverseAccel, maxSpeed, turnStrenght, gravityForce, dragOnGround;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundRayPoint;
    [SerializeField] private float groundRayLenght;

    private float speedInput;
    private float turnInput;
    private bool grounded;

    private void Start()
    {
        sphereRb.transform.parent = null;
    }

    private void Update()
    {
        speedInput = 0;
        if(Input.GetAxis("Vertical") > 0)
        {
            speedInput = Input.GetAxis("Vertical") * forwardAccel * 1000f;
        }
        else if (Input.GetAxis("Vertical") < 0)
        {
            speedInput = Input.GetAxis("Vertical") * reverseAccel * 1000f;
        }

        turnInput = Input.GetAxis("Horizontal");

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3 (0, turnInput * turnStrenght * Time.deltaTime * Input.GetAxis("Vertical"), 0));

        transform.position = sphereRb.transform.position;
    }

    private void FixedUpdate()
    {
        grounded = false;
        RaycastHit hit;

        if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLenght, groundLayer))
        {
            grounded = true;
            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }

        if(grounded)
        {
            sphereRb.drag = dragOnGround;

            if(Mathf.Abs(speedInput) > 0)
            {
                sphereRb.AddForce(transform.forward * speedInput);
            }
        }
        else
        {
            sphereRb.drag = 0.1f;

            sphereRb.AddForce(Vector3.up * -gravityForce * 100f);
        }
    }

}
