using Newtonsoft.Json.Bson;
using PowerslideKartPhysics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class ArcadeVehicleController : MonoBehaviour
{
    public LayerMask drivableSurface;

    public float MaxSpeed, accelaration, turn, gravity = 7f;
    private float baseAccelaration;
    public Rigidbody rb, carBody;
    
    [HideInInspector] public RaycastHit hit;
    public AnimationCurve frictionCurve;
    public AnimationCurve turnCurve;
    public PhysicMaterial frictionMaterial;

    [Header("Visuals")]
    public Transform BodyMesh;
    public Transform[] FrontWheels = new Transform[2];
    public Transform[] RearWheels = new Transform[2];
    [HideInInspector] public Vector3 carVelocity;
    [Range(0, 10)] public float BodyTilt;
    [SerializeField] private GameObject explosionParticleFX;
    private Light[] brakeLights;

    [Header("Audio settings")]
    public AudioSource engineSound;
    [Range(0, 1)] public float minPitch;
    [Range(1, 3)] public float MaxPitch;
    public AudioSource SkidSound;

    //Car variables
    [HideInInspector] public float skidWidth;
    private float radius;
    private Vector3 origin;

    //Fetch Setup
    private CameraExtras camExtras;
    private PlayerInput playerInput;
    private UI ui;

    //Inputs
    private float horizontalInput, verticalInput; //Movement Input


    //Nos stuff
    [Header("Nos Stuff")]
    [SerializeField] private float nosSpeedBoost;
    [SerializeField] private int nosCapacity;
    private float currentNos;
    public float CurrentNos { get { return currentNos; } }
    private GameObject nosFX;

    //Flip stuff
    [Header("Front Flip Stuff")]
    [SerializeField] private float flipBoost;
    private GameObject model;
    private Animator modelAnimator;
    public bool Flip { get { return flip; } }
    private bool flip = false;
    private bool flipAvailable;

    //Spin stuff
    [Header("Spin Stuff")]
    [SerializeField] private float spinSpeedDebuff;
    public bool Spin { get { return spin; } }
    private bool spin = false;

    private void Start()
    {
        radius = rb.GetComponent<SphereCollider>().radius;
        ui = GetComponentInChildren<UI>();
        playerInput = GetComponent<PlayerInput>();

        //Keeps value in mind for ease of return
        baseAccelaration = accelaration;

        //Nos Stuff
        nosFX = transform.Find("Mesh/Body/Hatchback/Exhaust/NOS").gameObject;
        nosFX.SetActive(false);
        currentNos = nosCapacity;
        NosToUI();

        //Reverse Stuff
        brakeLights = transform.GetComponentsInChildren<Light>();
        ManageBrakeLights(false);

        //Animation stuff
        model = transform.Find("Mesh").gameObject;
        modelAnimator = model.GetComponent<Animator>();

        //Camera stuff
        camExtras = GetComponentInChildren<CameraExtras>();
    }

    private void Update()
    {
        InputManager();

        Visuals();
        AudioManager();

        //Speedometer calculator
        ui.speedometer = Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z);

        //Nos stuff
        NosController();

        //Flip stuff
        FlipController();
        modelAnimator.SetBool("FrontFlip", flip);
        if (grounded())
        {
            flipAvailable = true;
        }        
    }

    private void InputManager()
    {
        //Move
        horizontalInput = playerInput.actions["Move"].ReadValue<Vector2>().x;   //turning input
        verticalInput = playerInput.actions["Move"].ReadValue<Vector2>().y;     //accelaration input

        //Spin
        if (playerInput.actions["Spin"].WasPressedThisFrame())
        {
            SpinAction(true);
        }

        if (playerInput.actions["Spin"].WasReleasedThisFrame())
        {
            SpinAction(false);
        }

        ///Nos input is handled in the NosController
        ///Flip input is handles in the FlipController
    }

    public void AudioManager()
    {
        engineSound.pitch = Mathf.Lerp(minPitch, MaxPitch, Mathf.Abs(carVelocity.z) / MaxSpeed);
        if (Mathf.Abs(carVelocity.x) > 10 && grounded())
        {
            SkidSound.mute = false;
        }
        else
        {
            SkidSound.mute = true;
        }
    }


    void FixedUpdate()
    {
        carVelocity = carBody.transform.InverseTransformDirection(carBody.velocity);
        
        if (Mathf.Abs(carVelocity.x) > 0)
        {
            //changes friction according to sideways speed of car
            frictionMaterial.dynamicFriction = frictionCurve.Evaluate(Mathf.Abs(carVelocity.x/100)); 
        }
        
        
        if (grounded())
        {
            //turnlogic
            float sign = Mathf.Sign(carVelocity.z);
            float TurnMultiplyer = turnCurve.Evaluate(carVelocity.magnitude/ MaxSpeed);
            if (verticalInput > 0.1f || carVelocity.z >1)
            {
                carBody.AddTorque(Vector3.up * horizontalInput * sign * turn*100* TurnMultiplyer);
            }
            else if (verticalInput < -0.1f || carVelocity.z < -1)
            {
                carBody.AddTorque(Vector3.up * horizontalInput * sign * turn*100* TurnMultiplyer);
            }

            //brakelogic
            if (Input.GetAxis("Jump") > 0.1f)
            {
                rb.constraints = RigidbodyConstraints.FreezeRotationX;
            }
            else
            {
                rb.constraints = RigidbodyConstraints.None;
            }

            //accelaration logic

            if (Mathf.Abs(verticalInput) > 0.1f && Input.GetAxis("Jump") < 0.1f)
            {
                rb.velocity = Vector3.Lerp(rb.velocity, carBody.transform.forward * verticalInput * MaxSpeed, accelaration / 10 * Time.deltaTime);
            }

            //body tilt
            carBody.MoveRotation(Quaternion.Slerp(carBody.rotation, Quaternion.FromToRotation(carBody.transform.up, hit.normal) * carBody.transform.rotation, 0.12f));

            //allow flipping again once the car lands
            flip = false;
        }
        else
        {
            carBody.MoveRotation(Quaternion.Slerp(carBody.rotation, Quaternion.FromToRotation(carBody.transform.up, Vector3.up) * carBody.transform.rotation, 0.02f));
            rb.velocity = Vector3.Lerp(rb.velocity, rb.velocity + Vector3.down* gravity, Time.deltaTime * gravity);
        }

    }
    public void Visuals()
    {
        //tires
        foreach (Transform FW in FrontWheels)
        {
            FW.localRotation = Quaternion.Slerp(FW.localRotation, Quaternion.Euler(FW.localRotation.eulerAngles.x,
                               30 * horizontalInput, FW.localRotation.eulerAngles.z), 0.1f);
            FW.GetChild(0).localRotation = rb.transform.localRotation;
        }
        RearWheels[0].localRotation = rb.transform.localRotation;
        RearWheels[1].localRotation = rb.transform.localRotation;

        //Body
        if(carVelocity.z > 1)
        {
            BodyMesh.localRotation = Quaternion.Slerp(BodyMesh.localRotation, Quaternion.Euler(Mathf.Lerp(0, -5, carVelocity.z / MaxSpeed),
                               BodyMesh.localRotation.eulerAngles.y, BodyTilt * horizontalInput), 0.05f);
        }
        else
        {
            BodyMesh.localRotation = Quaternion.Slerp(BodyMesh.localRotation, Quaternion.Euler(0,0,0) , 0.05f);
        }
        

    }

    public bool grounded() //checks for if vehicle is grounded or not
    {
        origin = rb.position + rb.GetComponent<SphereCollider>().radius * Vector3.up;
        var direction = -transform.up;
        var maxdistance = rb.GetComponent<SphereCollider>().radius + 0.2f;

        if (Physics.Raycast(rb.position, Vector3.down, out hit, maxdistance, drivableSurface))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnDrawGizmos()
    {
        //debug gizmos
        radius = rb.GetComponent<SphereCollider>().radius;
        float width = 0.02f;
        if (!Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(rb.transform.position + ((radius + width) * Vector3.down), new Vector3(2 * radius, 2*width, 4 * radius));
            if (GetComponent<BoxCollider>())
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(transform.position, GetComponent<BoxCollider>().size);
            }
            
        }

    }

    private void NosController()
    {
        if (playerInput.actions["Boost"].WasPerformedThisFrame() && !spin)
        {
            if (currentNos > 0)
            {
                accelaration = accelaration * nosSpeedBoost;
                nosFX.SetActive(true);
            }
        }

        if (playerInput.actions["Boost"].IsPressed() && currentNos > 0 && !spin)
        {
            currentNos -= 10.0f * Time.deltaTime;
            NosToUI();
        }

        if (playerInput.actions["Boost"].WasReleasedThisFrame() || accelaration != baseAccelaration && currentNos <= 0)
        {
            accelaration = baseAccelaration;
            nosFX.SetActive(false);
        }
    }

    private void NosToUI()
    {
        ui.nosCounter = Mathf.FloorToInt(currentNos);
    }

    private void FlipController()
    {        
        if (playerInput.actions["Flip"].WasPressedThisFrame() && !grounded() && !flip && flipAvailable)
        {
            flip = true;
            flipAvailable = false;
        }

        if (playerInput.actions["Flip"].WasReleasedThisFrame() && !grounded() && flip)
        {
            flip = false;

            Invoke("FlipBoost", 0.3f);
        }
    }

    private void FlipBoost()
    {
        rb.AddForce(transform.forward * flipBoost, ForceMode.Impulse);
    }

    //Controls the spin move
    private void SpinAction(bool startSpinning)
    {
        //Starts the first 180
        if (startSpinning)
        {
            //Sets spin bool for other controls
            spin = true;

            //Debuffs acceleration while reversing
            accelaration = accelaration * spinSpeedDebuff;
        }

        //Does the second 180 to finish the sequence
        else 
        {
            //Sets spin bool for other controls
            spin = false;

            //Puts the acceleration back to normal
            accelaration = baseAccelaration;
        }

        //Turns the car and the camera
        modelAnimator.SetBool("Spin", spin);
        camExtras.TurnCamCinematic(spin);

        //Toggles the brake lights
        ManageBrakeLights(startSpinning);
    }

    private void ManageBrakeLights(bool on)
    {
        if (on)
        {
            //Make the brake light red
            brakeLights[0].gameObject.GetComponent<Renderer>().material.color = Color.red;
            brakeLights[1].gameObject.GetComponent<Renderer>().material.color = Color.red;

            //Turn on the lights
            brakeLights[0].enabled = true;
            brakeLights[1].enabled = true;
        }
        else
        {
            //Make the brake light grey
            brakeLights[0].gameObject.GetComponent<Renderer>().material.color = Color.grey;
            brakeLights[1].gameObject.GetComponent<Renderer>().material.color = Color.grey;

            //Turn off the lights
            brakeLights[0].enabled = false;
            brakeLights[1].enabled = false;
        }
    }

    public void BlowUp()
    {
        Instantiate(explosionParticleFX, transform.position, Quaternion.identity, null);
        GameManager.Instance.GameOverDelay();
        Destroy(gameObject);
    }

    public void RefillNos()
    {
        currentNos = nosCapacity;
        NosToUI();
    }
}
