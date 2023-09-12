using Newtonsoft.Json.Bson;
using PowerslideKartPhysics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class ArcadeVehicleController : MonoBehaviour
{
    //Car Stats
    [Header("Stats")]
    [SerializeField] private int maxLives;
    [SerializeField] private int maxHealth;
    [SerializeField] private int nosCapacity;
    [SerializeField] private float nosSpeedBoost;
    [SerializeField] private float flipBoost;
    [SerializeField] private float spinSpeedDebuff;
    public float MaxSpeed, accelaration, turn, gravity = 7f;
    
    //Editor Setup
    [Header("Setup")]
    public LayerMask drivableSurface;
    public Rigidbody rb, carBody;
    [HideInInspector] public RaycastHit hit;
    public AnimationCurve frictionCurve;
    public AnimationCurve turnCurve;
    public PhysicMaterial frictionMaterial;

    //Visual Editor Setup
    [Header("Visuals")]
    public Transform BodyMesh;
    public Transform[] FrontWheels = new Transform[2];
    public Transform[] RearWheels = new Transform[2];
    [HideInInspector] public Vector3 carVelocity;
    [Range(0, 10)] public float BodyTilt;
    [SerializeField] private GameObject explosionParticleFX;
    private Light[] brakeLights;

    //Audio Editor Setup
    [Header("Audio settings")]
    public AudioSource engineSound;
    [Range(0, 1)] public float minPitch;
    [Range(1, 3)] public float MaxPitch;
    public AudioSource SkidSound;

    //Car variables
    private float baseAccelaration;
    [HideInInspector] public float skidWidth;
    private Vector3 origin;
    private float currentNos;
    private int currentHealth;
    private int currentLives;
    private bool deathAvailable;

        //Flip variables
        public bool Flip { get { return flip; } }
        private bool flip = false;
        private bool flipAvailable;

        //Spin variable
        public bool Spin { get { return spin; } }
        private bool spin = false;

    //Fetch Setup
    private float radius;
    private CameraExtras camExtras;
    private GameObject nosFX;
    private GameObject model;
    private Animator modelAnimator;
    private PlayerInput playerInput;
    private UI ui;
    [HideInInspector] public RespawnManager respawnManager;

    //Inputs
    private float horizontalInput, verticalInput; //Movement Input

    private void Start()
    {
        //Fetches
        radius          = rb.GetComponent<SphereCollider>().radius;                     //radius = sphereRB's radius
        ui              = GetComponentInChildren<UI>();                                 //ui = UI script in the canvas
        playerInput     = GetComponent<PlayerInput>();                                  //playerInput = PlayerInput script
        nosFX           = transform.Find("Mesh/Body/Hatchback/Exhaust/NOS").gameObject; //nosFX = Nos particle effect in the car's exhaust
        brakeLights     = transform.GetComponentsInChildren<Light>();                   //brakeLights = Light component of the brake lights
        model           = transform.Find("Mesh").gameObject;                            //model = gameobject the mesh of the car is attached to
        modelAnimator   = model.GetComponent<Animator>();                               //modelAnimator = animator of the car's body (used for flip and spin)
        camExtras       = GetComponentInChildren<CameraExtras>();                       //camExtras = CameraExtras script in cameraBrain
        respawnManager  = GetComponent<RespawnManager>();                               //respawnManager = RespawnManager script

        //Variable setup
        baseAccelaration = accelaration;
        currentNos = nosCapacity;
        currentHealth = maxHealth;
        currentLives = maxLives;
        deathAvailable = true;

        //Extra Setup
        nosFX.SetActive(false);
        ManageBrakeLights(false);

        //UI Setup
        NosToUI();
        HealthToUI();
        LivesToUI();
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

    public void RefillNos()
    {
        currentNos = nosCapacity;
        NosToUI();
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

    public void TakeDamage(int damageTaken, ArcadeVehicleController damageSource)
    {
        if (currentNos >=  damageTaken)
        {
            currentNos -= damageTaken;
        }
        else if (currentNos < damageTaken && currentNos != 0)
        {
            int damageToTake;                                   //new value to keep track of how much damage to relay to hp
            damageToTake = damageTaken - (int)currentNos;       //remove the currentNos value from the damageTaken to know how much hp to remove
            currentNos = 0.0f;                                  //set the currentNos to 0
            currentHealth -= damageToTake;                      //remove the rest of the damage to take from the currentHealth
        }
        else //if currentNos is 0, the damage can go straight to the currentHealth
        {
            currentHealth -= damageTaken;
        }

        //Relay to ui
        HealthToUI();
        NosToUI();

        //Kills the car if health is under 0
        if (currentHealth <= 0)
        {
            BlowUp(damageSource);
        }
    }

    public void RefillHealth()
    {
        currentHealth = maxHealth;
        HealthToUI();
    }

    private void HealthToUI()
    {
        ui.healthCounter = currentHealth;
    }

    public void BlowUp(ArcadeVehicleController damageSource)
    {
        if (deathAvailable)
        {
            //Toggles dead values
            RespawnToggle(false);

            //Gives credit where credit is due
            if (damageSource != null)
            {
                //Gives a kill to the player that killed this player
                damageSource.ui.Kill();

                //Remove the audio listener from this player
                GetComponentInChildren<AudioListener>().enabled = false;

                //Gives the audio listener to the killer
                damageSource.GetComponentInChildren<AudioListener>().enabled = true;
            }

            //Does the explosionFX
            Instantiate(explosionParticleFX, transform.position, Quaternion.identity, null);

            if (currentLives > 1)
            {
                //Remove 1 life
                currentLives--;

                //Relays to UI
                LivesToUI();

                //Switch to GameOverUI
                ui.UIRedraw(ui.gameOverUI);

                //Spawns a dead car

                //Starts countdown until respawn
                StartCoroutine(RespawnCountdown(respawnManager.respawnDelay));
            }
            else
            {
                //Deletes the player
                Destroy(gameObject);
            }
        }
    }

    IEnumerator RespawnCountdown(int seconds)
    {
        int counter = seconds;
        while (counter > 0)
        {
            ui.respawnTimer = counter;
            yield return new WaitForSeconds(1);
            counter--;
        }
        respawnManager.Respawn();
    }

    public void CarRespawn()
    {
        //Switch to GameUI
        ui.UIRedraw(ui.gameUI);

        //Toggles alive values
        RespawnToggle(true);

        //Refill values
        RefillHealth();
        RefillNos();
    }

    private void RespawnToggle(bool toggle)
    {
        //Toggles further deaths until respawn
        deathAvailable = toggle;

        //Toggles controls so the player can't move while they're dead
        playerInput.enabled = toggle;

        //Toggles the rigidbody so the dead player doesn't just fall through the ground
        carBody.isKinematic = !toggle;

        //Toggles the collider so nothing can collide with it while dead
        rb.GetComponent<SphereCollider>().enabled = toggle;
        carBody.GetComponent<BoxCollider>().enabled = toggle;

        //Toggles meshes so they car is invisible while dead
        model.SetActive(toggle);

        //Toggles engine sound so the car doesn't make sounds while dead
        engineSound.enabled = toggle;
    }

    private void LivesToUI()
    {
        ui.livesCounter = currentLives;
    }
}
