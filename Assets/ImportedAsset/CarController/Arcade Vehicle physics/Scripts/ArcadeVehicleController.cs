using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ArcadeVehicleController : MonoBehaviour
{
    public enum groundCheck { rayCast, sphereCaste };
    public enum MovementMode { Velocity, AngularVelocity };
    public MovementMode movementMode;
    public groundCheck GroundCheck;
    public LayerMask drivableSurface;

    public float MaxSpeed, accelaration, turn, gravity = 7f;
    private float baseAccelaration;
    public Rigidbody rb, carBody;
    
    [HideInInspector]
    public RaycastHit hit;
    public AnimationCurve frictionCurve;
    public AnimationCurve turnCurve;
    public PhysicMaterial frictionMaterial;
    [Header("Visuals")]
    public Transform BodyMesh;
    public Transform[] FrontWheels = new Transform[2];
    public Transform[] RearWheels = new Transform[2];
    [HideInInspector]
    public Vector3 carVelocity;
    
    [Range(0,10)]
    public float BodyTilt;
    [Header("Audio settings")]
    public AudioSource engineSound;
    [Range(0, 1)]
    public float minPitch;
    [Range(1, 3)]
    public float MaxPitch;
    public AudioSource SkidSound;

    [HideInInspector]
    public float skidWidth;

    private float radius, horizontalInput, verticalInput;
    private Vector3 origin;

    //Car info to relay to UI
    private float speedometer;
    public float Speedometer { get { return speedometer; } }

    //Camera stuff
    private CameraExtras camExtras;

    // Debug Stuff
    [HideInInspector][SerializeField] private KeyCode gameOverKey = KeyCode.R;
    [HideInInspector][SerializeField] private KeyCode refillNosKey = KeyCode.N;

    //Nos stuff
    [Header("Nos Stuff")]
    [SerializeField] private KeyCode nosKey = KeyCode.E;
    [SerializeField] private float nosSpeedBoost;
    [SerializeField] private int nosCapacity;
    private float currentNos;
    public float CurrentNos { get { return currentNos; } }
    private GameObject nosFX;

    //Flip stuff
    [Header("Front Flip Stuff")]
    [SerializeField] private KeyCode flipKey = KeyCode.Space;
    [SerializeField] private float flipBoost;
    private GameObject model;
    private Animator modelAnimator;
    private bool flip = false;

    //Spin stuff
    [Header("Spin Stuff")]
    [SerializeField] private KeyCode spinKey = KeyCode.LeftShift;
    [SerializeField] private float spinSpeedDebuff;
    private bool spin = false;

    private void Start()
    {
        radius = rb.GetComponent<SphereCollider>().radius;
        if (movementMode == MovementMode.AngularVelocity)
        {
            Physics.defaultMaxAngularSpeed = 100;
        }

        //Keeps value in mind for ease of return
        baseAccelaration = accelaration;

        //Nos Stuff
        nosFX = transform.Find("Mesh/Body/Hatchback/Body/Exhaust/NOS").gameObject;
        nosFX.SetActive(false);
        currentNos = nosCapacity;
        NosToGM();

        //Animation stuff
        model = transform.Find("Mesh").gameObject;
        modelAnimator = model.GetComponent<Animator>();

        //Camera stuff
        camExtras = GetComponentInChildren<CameraExtras>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal"); //turning input
        verticalInput = Input.GetAxis("Vertical");     //accelaration input
        Visuals();
        AudioManager();

        //Debug stuff
        DebugController();

        //Speedometer calculator
        speedometer = Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z);
        GameManager.Instance.SpeedometerGM = speedometer;

        //Nos stuff
        NosController();

        //Flip stuff
        FlipController();
        modelAnimator.SetBool("FrontFlip", flip);

        //Spin stuff
        SpinController();
        modelAnimator.SetBool("Spin", spin);
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

            if (movementMode == MovementMode.AngularVelocity)
            {
                if (Mathf.Abs(verticalInput) > 0.1f)
                {
                    rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, carBody.transform.right * verticalInput * MaxSpeed/radius, accelaration * Time.deltaTime);
                }
            }
            else if (movementMode == MovementMode.Velocity)
            {
                if (Mathf.Abs(verticalInput) > 0.1f && Input.GetAxis("Jump")<0.1f)
                {
                    rb.velocity = Vector3.Lerp(rb.velocity, carBody.transform.forward * verticalInput * MaxSpeed, accelaration/10 * Time.deltaTime);
                }
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

        if (GroundCheck == groundCheck.rayCast)
        {
            if (Physics.Raycast(rb.position, Vector3.down, out hit, maxdistance, drivableSurface))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        else if(GroundCheck == groundCheck.sphereCaste)
        {
            if (Physics.SphereCast(origin, radius + 0.1f, direction, out hit, maxdistance, drivableSurface))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        else { return false; }
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

    private void DebugController()
    {
        if (Input.GetKeyDown(gameOverKey))
        {
            GameOver();
        }

        if (Input.GetKeyUp(refillNosKey))
        {
            currentNos = nosCapacity;
            NosToGM();
        }
    }

    private void NosController()
    {
        if (Input.GetKeyDown(nosKey) && currentNos > 0)
        {
            accelaration = accelaration * nosSpeedBoost;
            nosFX.SetActive(true);
        }

        if (Input.GetKey(nosKey) && currentNos > 0)
        {
            currentNos -= 10.0f * Time.deltaTime;
            NosToGM();
        }

        if (Input.GetKeyUp(nosKey) || accelaration != baseAccelaration && currentNos <= 0)
        {
            accelaration = baseAccelaration;
            nosFX.SetActive(false);
        }
    }

    private void NosToGM()
    {
        GameManager.Instance.NosCounterGM = Mathf.FloorToInt(currentNos);
    }

    private void FlipController()
    {
        if (Input.GetKeyDown(flipKey) && !grounded() && !flip)
        {
            flip = true;
        }

        if (Input.GetKeyUp(flipKey) && !grounded() && flip)
        {
            flip = false;

            Invoke("FlipBoost", 0.3f);
        }
    }

    private void FlipBoost()
    {
        rb.AddForce(transform.forward * flipBoost, ForceMode.Impulse);
    }

    private void SpinController()
    {
        if (Input.GetKeyDown(spinKey) && !camExtras.CinematicCamTurn)
        {
            camExtras.CinematicCamTurn = true;

            if (spin)
            {
                spin = false;
                accelaration = baseAccelaration;
            }
            else
            {
                spin = true;
                accelaration = accelaration * spinSpeedDebuff;
            }
        }
    }

    private void GameOver()
    {
        GameManager.Instance.GameOver();
        Destroy(gameObject);
    }
}
