using Newtonsoft.Json.Bson;
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

    [Header("Front Flip Stuff")]
    [SerializeField] private KeyCode flipKey = KeyCode.Space;
    [SerializeField] private float flipBoost;
    private GameObject model;
    private Animator modelAnimator;
    private bool flip;

    private float speedInput;
    private float turnInput;
    private bool grounded;

    //Car info to relay to UI
    private float speedometer;

    public float Speedometer { get { return speedometer; } }

    private void Start()
    {
        sphereRb.transform.parent = null;

        model = transform.Find("Ambulance Model").gameObject;
        modelAnimator = model.GetComponent<Animator>();
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

        //Speedometer calculator
        speedometer = Mathf.Abs(sphereRb.velocity.x) + Mathf.Abs(sphereRb.velocity.z);
        GameManager.Instance.SpeedometerGM = speedometer;

        //Flip stuff
        SpinController();
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

    private void SpinController()
    {
        if (Input.GetKeyDown(flipKey) && !grounded && !flip)
        {
            flip = true;
            Invoke("InterruptFlip", 0.5f);
        }

        modelAnimator.SetBool("FrontFlip", flip);
    }

    private void InterruptFlip()
    {
        flip = false;
        sphereRb.AddForce(transform.forward * flipBoost, ForceMode.Impulse);
    }
}
