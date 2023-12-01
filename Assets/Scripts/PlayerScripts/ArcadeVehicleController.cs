using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using MoreMountains.Feedbacks;
using UnityEngine.Events;


public class ArcadeVehicleController : MonoBehaviour
{
    //HAS THINGS TO DISABLE ON RELEASE

    #region Fields
    //Car Stats
    [Header("Stats")]
    [SerializeField] private int maxHealth;
    [SerializeField] private int nosCapacity;
    [SerializeField] private float nosSpeedBoost;
    [SerializeField] private float coolingMultiplier;
    [SerializeField] private float flipBoost;
    [SerializeField] private float spinSpeedDebuff;
    [SerializeField] private int spinDamage;
    [SerializeField] private float spinBump;
    public float MaxSpeed, accelaration, turn, gravity = 7f;

    //Editor Setup
    [Header("Setup")]
    public LayerMask drivableSurface;
    public Rigidbody rb, carBody;
    [HideInInspector] public RaycastHit hit;
    public AnimationCurve frictionCurve;
    public AnimationCurve turnCurve;
    public PhysicMaterial frictionMaterial;
    [SerializeField] private bool flipUsed;

    //Visual Editor Setup
    [Header("Visuals")]
    public Transform BodyMesh;
    public Transform[] FrontWheels = new Transform[2];
    public Transform[] RearWheels = new Transform[2];
    [HideInInspector] public Vector3 carVelocity;
    [Range(0, 10)] public float BodyTilt;
    [SerializeField] private GameObject explosionParticleFX;
    [SerializeField] private GameObject[] brakeLights;
    [SerializeField] private Material brakeLightMat;
    [SerializeField] private GameObject nosFX;
    [SerializeField] private GameObject playerPositionArrow;
    private GameObject crown;

    //Audio Editor Setup
    [Header("Audio settings")]
    public AudioSource engineSound;
    [Range(0, 1)] public float minPitch;
    [Range(1, 3)] public float MaxPitch;
    public AudioSource SkidSound;

    //Car variables
    private int speedometer;
    private float baseAccelaration;
    [HideInInspector] public float skidWidth;
    private Vector3 origin;
    private float currentNos;
    private int currentHealth;
    private bool deathAvailable;
    private float heat;
    private Material ogBrakeMat;
    private float spinPressTime = 0.0f;
    private bool currentReverseCamPrio = false;
    private bool insideBlueZone = true;

    //Flip variables
    public bool Flip { get; }
    private bool flip = false;
    private bool flipAvailable;

    //Spin variable
    public bool Spin { get { return spin; } }
    private bool spin = false;
    private bool spinning = false;

    //Fetch Setup
    private float radius;
    private CameraExtras camExtras;
    private GameObject model;
    private Animator modelAnimator;
    private PlayerInput playerInput;
    private UI ui;
    private RespawnManager respawnManager;
    private PlayerColorSetup playerColor;

    //Relay
    public UI UI { get { return ui; } }
    public RespawnManager RespawnManager { get { return respawnManager; } }
    public Rigidbody RiBy { get { return rb; } }
    public float Heat { get { return heat; } set { heat = value; } }
    public PlayerColorSetup PlayerColor { get { return playerColor; } }
    public PlayerInput PlayerInputToRelay { get { return playerInput; } }

    //Inputs
    private float horizontalInput, verticalInput; //Movement Input


    //Feedback Systems ------------
    [Header("Feedbacks")]
    public MMF_Player WheelsFeedback_Left;
    public MMF_Player WheelsFeedback_Right;
    public MMF_Player NosFeedback;
    public MMF_Player LifeFeedback;
    public MMF_Player BoostFeedback;
    public MMF_Player SpinFeedback;


    #endregion

    private void Start()
    {
        //Fetches
        radius = rb.GetComponent<SphereCollider>().radius;      //radius = sphereRB's radius
        ui = GetComponentInChildren<UI>();                      //ui = UI script in the canvas
        model = transform.Find("Model").gameObject;             //model = gameobject the mesh of the car is attached to
        modelAnimator = model.GetComponent<Animator>();         //modelAnimator = animator of the car's body (used for flip and spin)
        camExtras = GetComponentInChildren<CameraExtras>();     //camExtras = CameraExtras script in cameraBrain
        respawnManager = GetComponent<RespawnManager>();        //respawnManager = RespawnManager script
        crown = model.transform.Find("Crown").gameObject;       //Finds the firstPlacePlayer crown object
        playerColor = GetComponentInChildren<PlayerColorSetup>(); //Grabs the color setup script
        playerInput = GetComponent<PlayerControllerRelay>().CarInput; //Grabs the input

        //Variable setup
        baseAccelaration = accelaration;
        currentNos = nosCapacity;
        currentHealth = maxHealth;
        deathAvailable = true;

        //Extra Setup
        nosFX.SetActive(false);
        ManageBrakeLights(false);
        ogBrakeMat = brakeLights[0].GetComponent<Renderer>().materials[2];
        EnterBlueZone();

        //UI Setup
        NosToUI();
        HealthToUI();

        Debug.Log("Has things to disable on release");
    }

    private void Update()
    {
        InputManager();

        Visuals();

        AudioManager();

        //Speedometer calculator
        speedometer = (int)Mathf.Round(Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z));
        ui.Speedometer = speedometer;

        //Cooling stuff
        CoolingManager();

        //DISABLE ON RELEASE
        DebugInput();

        //Spin bump checker
        if (model.transform.localRotation.y != 0)
        {
            spinning = true;
        }
        else
        {
            spinning = false;
        }
    }

    private void DebugInput()
    {
        //Blow Up Override
        if (Input.GetKeyDown(KeyCode.H))
        {
            BlowUp(null);
        }

        //Fastforward
        if (Input.GetKeyDown(KeyCode.L))
        {
            var checkpoints = CheckpointManager.Instance.Checkpoints;

            this.transform.position = checkpoints[checkpoints.Length - 1].transform.position;

            respawnManager.NextCheckpoint = checkpoints.Length - 1;
        }
    }


    private void InputManager()
    {
        horizontalInput = playerInput.actions["Move"].ReadValue<Vector2>().x;   //turning input

        verticalInput = playerInput.actions["Move"].ReadValue<Vector2>().y;     //accelaration input

        //Spin
        if (playerInput.actions["Spin"].WasPressedThisFrame())
        {
            SpinAction(true);
        }

        //Spin return
        if (playerInput.actions["Spin"].WasReleasedThisFrame())
        {
            SpinAction(false);
        }

        //Spin held
        ReverseController();

        //Flip
        if (flipUsed)
        {
            if (playerInput.actions["Flip"].WasPressedThisFrame() && !grounded() && !flip && flipAvailable && !spin)
            {
                flip = true;
                flipAvailable = false;
                Invoke("FlipBoost", 0.3f);
                
            }

            modelAnimator.SetBool("FrontFlip", flip);

            if (grounded())
            {
                flipAvailable = true;
            }
        }

        //Nos
        if (horizontalInput != 0 || verticalInput != 0)
        {
            NosController();
        }
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
            frictionMaterial.dynamicFriction = frictionCurve.Evaluate(Mathf.Abs(carVelocity.x / 100));
        }


        if (grounded())
        {
            //turnlogic
            float sign = Mathf.Sign(carVelocity.z);
            float TurnMultiplyer = turnCurve.Evaluate(carVelocity.magnitude / MaxSpeed);
            if (verticalInput > 0.1f || carVelocity.z > 1)
            {
                carBody.AddTorque(Vector3.up * horizontalInput * sign * turn * 100 * TurnMultiplyer);
            }
            else if (verticalInput < -0.1f || carVelocity.z < -1)
            {
                carBody.AddTorque(Vector3.up * horizontalInput * sign * turn * 100 * TurnMultiplyer);
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
            rb.velocity = Vector3.Lerp(rb.velocity, rb.velocity + Vector3.down * gravity, Time.deltaTime * gravity);
        }

    }

    public void Visuals()
    {
        //tires
        if (FrontWheels.Length > 0 || RearWheels.Length > 0)
        {

            foreach (Transform FW in FrontWheels)
            {
                FW.localRotation = Quaternion.Slerp(FW.localRotation, Quaternion.Euler(FW.localRotation.eulerAngles.x,
                                   30 * horizontalInput, FW.localRotation.eulerAngles.z), 0.1f);
                FW.GetChild(0).localRotation = rb.transform.localRotation;
            }
            RearWheels[0].localRotation = rb.transform.localRotation;
            RearWheels[1].localRotation = rb.transform.localRotation;
        }

        //Body
        if (carVelocity.z > 1)
        {
            BodyMesh.localRotation = Quaternion.Slerp(BodyMesh.localRotation, Quaternion.Euler(Mathf.Lerp(0, -5, carVelocity.z / MaxSpeed),
                                        BodyMesh.localRotation.eulerAngles.y, BodyTilt * horizontalInput), 0.05f);
        }
        else
        {
            BodyMesh.localRotation = Quaternion.Slerp(BodyMesh.localRotation, Quaternion.Euler(0, 0, 0), 0.05f);
        }

        //Crown
        if (RaceManager.Instance.FirstPlacePlayer == respawnManager)
        {
            crown.SetActive(true);
        }
        else
        {
            crown.SetActive(false);
        }

        //Wheels
        if (horizontalInput > 0) //If the player inputs right, it calls the right feedback
        {
            WheelsFeedback_Right?.PlayFeedbacks();
        }
        
        if (horizontalInput < 0) //If the player inputs left, it calls the left feedback
        {
            WheelsFeedback_Left?.PlayFeedbacks();
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
            Gizmos.DrawWireCube(rb.transform.position + ((radius + width) * Vector3.down), new Vector3(2 * radius, 2 * width, 4 * radius));
            if (GetComponent<BoxCollider>())
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(transform.position, GetComponent<BoxCollider>().size);
            }

        }

    }

    #region NOS

    private void NosController()
    {
        //Activates boost when the boost is pressed while you have enough nos in the capacity, you're not spinning and going forward
        if (playerInput.actions["Boost"].IsPressed() && currentNos > 0 && !spin && verticalInput > 0)
        {
            //Activates the nos
            if (!nosFX.activeInHierarchy)
            {
                accelaration = accelaration * nosSpeedBoost;
                nosFX.SetActive(true);
            }

            currentNos -= 10.0f * Time.deltaTime;
            NosToUI();

            NosFeedback?.PlayFeedbacks();  // Call UI Feedback System
            BoostFeedback?.PlayFeedbacks(); // call boost feedback
        }

        if (playerInput.actions["Boost"].WasReleasedThisFrame() || accelaration != baseAccelaration && currentNos <= 0)
        {
            accelaration = baseAccelaration;
            nosFX.SetActive(false);
        }
    }

    public void BoostPlayer(int boostPadLevel, Vector3 direction)
    {
        float boostForce = 0.0f;

        //Sets the boost force
        if (boostPadLevel == 0)
        {
            boostForce = BoostPadManager.Instance.Lvl0boostForce;
        }
        else if (boostPadLevel == 1)
        {
            boostForce = BoostPadManager.Instance.Lvl1boostForce;
        }
        else if (boostPadLevel == 2)
        {
            boostForce = BoostPadManager.Instance.Lvl2boostForce;
        }

        //Boosts player
        rb.AddForce(direction * boostForce, ForceMode.Impulse);

        BoostFeedback?.PlayFeedbacks(); // call boost feedback
    }

    public void RefillNos()
    {
        currentNos = nosCapacity;
        NosToUI();
    }

    private void NosToUI()
    {
        ui.NosCounter = Mathf.FloorToInt(currentNos);
    }

    #endregion

    
    
    private void FlipBoost()
    {
        rb.AddForce(transform.forward * flipBoost, ForceMode.Impulse);
        flip = false;
    }

    //Controls the spin move
    private void SpinAction(bool startSpinning)
    {
        //Starts the first 180
        if (startSpinning && !spin)
        {
            //Sets spin bool to true and debuffs acceleration while reversing
            spin = true;
            accelaration = accelaration * spinSpeedDebuff;
            SpinFeedback?.PlayFeedbacks();
        }

        //Does the second 180 to finish the sequence
        else
        {
            //Sets spin bool to false and puts the acceleration back to normal
            spin = false;
            accelaration = baseAccelaration;
        }

        //Turns the car
        modelAnimator.SetBool("Spin", spin);

        //Toggles the brake lights
        ManageBrakeLights(startSpinning);
    }

    private void ReverseController()
    {
        //Rotates the camera if the spin action is pressed for more than 1 second.
        if (playerInput.actions["Spin"].IsPressed())
        {
            if (grounded())
            {
                // Increment the time the "Spin" action has been pressed.
                spinPressTime += Time.deltaTime;

                // Check if the action has been pressed for the desired duration.
                if (spinPressTime >= 0.28f)
                {
                    //Turns the camera
                    TurnCamera();

                    // Reset the timer to prevent repeated actions.
                    spinPressTime = 0f;
                }
            }
        }
        else
        {
            // Reset the timer if the "Spin" action is released.
            spinPressTime = 0f;

            //Reset the camera too
            TurnCamera();
        }
    }

    private void TurnCamera()
    {
        if (currentReverseCamPrio != spin)
        {
            camExtras.SwitchCam();
            currentReverseCamPrio = spin;
        }
    }

    private void ManageBrakeLights(bool on)
    {
        Material[] brakeLightMaterials0 = brakeLights[0].GetComponent<Renderer>().materials;
        Material[] brakeLightMaterials1 = brakeLights[1].GetComponent<Renderer>().materials;

        if (on)
        {
            // Make the second material of both brake lights the lit mat
            brakeLightMaterials0[2] = brakeLightMat;
            brakeLightMaterials1[2] = brakeLightMat;
        }
        else
        {
            // Make the second material of both brake lights the mat it used to be
            brakeLightMaterials0[2] = ogBrakeMat;
            brakeLightMaterials1[2] = ogBrakeMat;
        }

        // Assign the modified materials arrays back to the Renderers
        brakeLights[0].GetComponent<Renderer>().materials = brakeLightMaterials0;
        brakeLights[1].GetComponent<Renderer>().materials = brakeLightMaterials1;
    }

    public void TakeDamage(int damageTaken, ArcadeVehicleController damageSource)
    {
        if (currentNos >= damageTaken)
        {
            currentNos -= damageTaken;
        }
        else if (currentNos < damageTaken)
        {
            int damageToTake = damageTaken;   //new value to keep track of how much damage to relay to hp

            if (currentNos != 0)
            {                                                
                damageToTake = damageTaken - (int)currentNos;       //remove the currentNos value from the damageTaken to know how much hp to remove
                currentNos = 0.0f;                                  //set the currentNos to 0
            }            
            
            TakeHealthDamage(damageToTake, damageSource);       //remove the rest of the damage to take from the currentHealth                      
        }

        //Relay to ui
        NosToUI();
    }

    //Damage directly at the health pool
    private void TakeHealthDamage(int damageTaken, ArcadeVehicleController damageSource)
    {
        //Removes hp = to the damage taken
        currentHealth -= damageTaken;

        //Relay to ui
        HealthToUI();

        //Kills the car if health is under 0
        if (currentHealth <= 0)
        {
            BlowUp(damageSource);
        }

        LifeFeedback?.PlayFeedbacks();
    }

    #region BlueZone

    // Start the coroutine when leaving the blue zone
    public void LeaveBlueZone()
    {
        if (insideBlueZone)
        {
            insideBlueZone = false;

            //Activates the ui Blue Zone Damage Visuals
            ui.BlueZoneDamageVisualsToggle(true);

            StartCoroutine(BlueZoneDamager());
        }
    }

    IEnumerator BlueZoneDamager()
    {
        while (!insideBlueZone)
        {
            yield return new WaitForSeconds(1f);
            TakeHealthDamage(RaceManager.Instance.BlueZoneDps, null);
        }
    }

    // Stop the coroutine when entering the blue zone
    public void EnterBlueZone()
    {
        if (!insideBlueZone)
        {
            insideBlueZone = true;

            //Deactivates the ui Blue Zone Damage Visuals
            ui.BlueZoneDamageVisualsToggle(false);

            StopCoroutine(BlueZoneDamager());
        }
    }

    #endregion

    public void RefillHealth()
    {
        currentHealth = maxHealth;
        HealthToUI();
    }
    
    private void HealthToUI()
    {
        ui.HealthCounter = currentHealth;
    }

    public void BlowUp(ArcadeVehicleController damageSource)
    {
        if (deathAvailable)
        {
            playerInput.DeactivateInput();

            //Toggles dead values
            RespawnToggle(false);

            //Gives credit where credit is due
            if (damageSource != null)
            {
                //Gives a kill to the player that killed this player
                damageSource.ui.Kill();

                //If you died with the audio listener, give it to the player that killed you
                AudioListener myListener = GetComponentInChildren<AudioListener>();
                if (myListener.enabled)
                {
                    //Remove the audio listener from this player
                    GetComponentInChildren<AudioListener>().enabled = false;

                    //Gives the audio listener to the killer
                    damageSource.GetComponentInChildren<AudioListener>().enabled = true;
                }
            }

            //Stops player animations
            SpinAction(false);
            flip = false;

            //Does the explosionFX
            GameObject explosion = Instantiate(explosionParticleFX, transform.position, Quaternion.identity, null);
            Destroy(explosion, 5.0f);

            //Spawns a dead car

            //Removes velocity
            rb.velocity = Vector3.zero;

            //Starts countdown until respawn
            StartCoroutine(RespawnCountdown(respawnManager.respawnDelay));

            //Switch to GameOverUI
            ui.UIRedraw(ui.gameOverUI);
        }
    }

    IEnumerator RespawnCountdown(int seconds)
    {
        int counter = seconds;
        while (counter > 0)
        {
            ui.RespawnTimer = counter;
            yield return new WaitForSeconds(1);
            counter--;
        }
        respawnManager.Respawn();
    }

    public void CarRespawn()
    {
        //Switch to GameUI
        ui.UIRedraw(ui.gameUI);

        //Reactivates playerInput
        playerInput.ActivateInput();

        //Toggles alive values
        RespawnToggle(true);

        //Refill values
        RefillHealth();
        RefillNos();

        camExtras.ResetCamsToDefault();

        //Sets the nextCheckpoint to the same as the first player
        respawnManager.NextCheckpoint = RaceManager.Instance.FirstPlacePlayer.NextCheckpoint;

        //Respawn Boost
        BoostPlayer(0, RaceManager.Instance.RespawnPoint.transform.forward);
    }

    private void RespawnToggle(bool toggle)
    {
        //Toggles further deaths until respawn
        deathAvailable = toggle;

        //Toggles the rigidbodies gravities so the dead player doesn't just fall through the ground
        carBody.useGravity = toggle;
        rb.useGravity = toggle;

        //Toggles the collider so nothing can collide with it while dead
        rb.GetComponent<SphereCollider>().enabled = toggle;
        carBody.GetComponent<CapsuleCollider>().enabled = toggle;

        //Toggles visuals
        playerColor.RespawnToggle(toggle);

        //Toggles PlayerPositionArrow
        playerPositionArrow.SetActive(toggle);

        //Toggles engine sound so the car doesn't make sounds while dead
        engineSound.enabled = toggle;
    }

    private void CoolingManager()
    {
        //Removes heat at the rate of the car's speed multiplied by the cooling multiplier which is divided by 100 for ease of use
        heat -= speedometer * coolingMultiplier * Time.deltaTime;
        heat = Mathf.Clamp(heat, 0.0f, 100.0f);
        HeatToUI();
    }

    private void HeatToUI()
    {
        ui.HeatCounter = (int)Mathf.Round(heat);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (spinning)
        {
            //Makes sure the trigger isn't with the spinner
            if (other.gameObject.GetComponentInParent<ArcadeVehicleController>() != this)
            {
                //If the projectile collides with a player
                if (other.transform.tag == "MainPlayer")
                {
                    //Grabs the ArcadeVehicleController from collider
                    ArcadeVehicleController aVC = other.gameObject.GetComponentInParent<ArcadeVehicleController>();

                    //Deals damage
                    aVC.TakeDamage(spinDamage, this);

                    //Pushes the enemy
                    Rigidbody riby = aVC.RiBy;

                    // Calculate the direction from the player to the enemy
                    Vector3 pushDirection = other.transform.position - transform.position;

                    // Apply a force in the direction away from the player
                    riby.AddForce(pushDirection.normalized * spinBump, ForceMode.Impulse);
                }
            }
        }
    }
}
