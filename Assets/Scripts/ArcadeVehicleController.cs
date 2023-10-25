using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.VFX;

public class ArcadeVehicleController : MonoBehaviour
{
    //Car Stats
    [Header("Stats")]
    [SerializeField] private int maxLives;
    [SerializeField] private int maxHealth;
    [SerializeField] private int nosCapacity;
    [SerializeField] private float nosSpeedBoost;
    [SerializeField] private float coolingMultiplier;
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
    [SerializeField] private GameObject[] brakeLights;
    [SerializeField] private Material brakeLightMat;
    [SerializeField] private GameObject nosFX;
    [SerializeField] private Color primaryColor;
    [SerializeField] private Color secondaryColor;
    [SerializeField] private bool randomColors;
    [SerializeField] private GameObject VFX;
    [SerializeField] private GameObject playerPositionArrow;

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
    private int currentLives;
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

    //Fetch Setup
    private float radius;
    private CameraExtras camExtras;
    private GameObject model;
    private Animator modelAnimator;
    private PlayerInput playerInput;
    private UI ui;
    private RespawnManager respawnManager;
    private List<MeshRenderer> meshRendererList;
    private List<VisualEffect> vfxList;

    //Relay
    public UI UI { get { return ui; } }
    public RespawnManager RespawnManager { get { return respawnManager; } }
    public Rigidbody RiBy { get { return rb; } }
    public bool RandomColor { get { return randomColors; } }
    public float Heat { get { return heat; } set { heat = value; } }

    //Inputs
    private float horizontalInput, verticalInput; //Movement Input

    //Tests
    private bool alive;

    private void Start()
    {
        //Fetches
        radius = rb.GetComponent<SphereCollider>().radius;      //radius = sphereRB's radius
        ui = GetComponentInChildren<UI>();                      //ui = UI script in the canvas
        playerInput = GetComponent<PlayerInput>();              //playerInput = PlayerInput script
        model = transform.Find("Model").gameObject;             //model = gameobject the mesh of the car is attached to
        modelAnimator = model.GetComponent<Animator>();         //modelAnimator = animator of the car's body (used for flip and spin)
        camExtras = GetComponentInChildren<CameraExtras>();     //camExtras = CameraExtras script in cameraBrain
        respawnManager = GetComponent<RespawnManager>();        //respawnManager = RespawnManager script
        meshRendererList = new List<MeshRenderer>();            //Initialize meshRendererList to store the meshRenderers found in the function below
        vfxList = new List<VisualEffect>();                     //Initialize Visual Effect list
        FindMeshRenderers(this.gameObject.transform);           //Finds all meshRendrers to toggle on and off during death sequences
        FindVFX(VFX.transform);                                 //Finds all Visual Effects in the vfx object

        //Variable setup
        baseAccelaration = accelaration;
        currentNos = nosCapacity;
        currentHealth = maxHealth;
        currentLives = maxLives;
        deathAvailable = true;

        //Extra Setup
        nosFX.SetActive(false);
        ManageBrakeLights(false);
        ogBrakeMat = brakeLights[0].GetComponent<Renderer>().materials[2];
        EnterBlueZone();
        if (!RaceManager.Instance.test)
        {
            alive = true;
        }

        //UI Setup
        NosToUI();
        HealthToUI();
        LivesToUI();
    }

    // This function will recursively find all MeshRenderers in the children of the specified transform
    private void FindMeshRenderers(Transform parentTransform)
    {
        // Loop through each child of the parentTransform
        foreach (Transform child in parentTransform)
        {
            // Check if the child has a MeshRenderer component
            MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();

            if (meshRenderer != null)
            {
                meshRendererList.Add(meshRenderer);
            }

            // Recursively search the child's children
            FindMeshRenderers(child);
        }

        //SetupColor(primaryColor, secondaryColor);
    }

    private void FindVFX(Transform parentTransform)
    {
        // Loop through each child of the parentTransform
        foreach (Transform child in parentTransform)
        {
            // Check if the child has a Visual Effect component
            VisualEffect vfx = child.GetComponent<VisualEffect>();

            if (vfx != null)
            {
                vfxList.Add(vfx);
            }

            // Recursively search the child's children
            FindVFX(child);
        }

        //SetupVFXColor(primaryColor, secondaryColor);
    }

    //Sets the color of the car body and the gradient of the trail vfx
    public void SetupColor(Color _primaryColor, Color _secondaryColor)
    {
        foreach (MeshRenderer meshRenderer in meshRendererList)
        {
            Material[] materials = meshRenderer.materials; // Get a reference to the materials array

            for (int i = 0; i < materials.Length; i++)
            {
                Material mat = materials[i]; // Get a reference to the current material

                // Changes all the base mat of the car to the chosen mat
                if (mat.name == "Synthwave_base (Instance)")
                {
                    materials[i].color = _primaryColor; // Modify the material in the array
                }

                // Changes all the base neon mat of the car to the chosen neon mat
                if (mat.name == "Synthwave_neon_base (Instance)")
                {
                    materials[i].color = _secondaryColor; // Modify the material in the array
                }
            }

            // Assign the modified materials array back to the meshRenderer
            meshRenderer.materials = materials;
        }

        //Creates a new gradient for the vfx
        var gradient = new Gradient();

        // Blend color from red at 0% to blue at 100%
        var colors = new GradientColorKey[2];
        colors[0] = new GradientColorKey(_primaryColor, 1.0f);
        colors[1] = new GradientColorKey(_secondaryColor, 0.0f);

        // Blend alpha from opaque at 0% to transparent at 100%
        var alphas = new GradientAlphaKey[2];
        alphas[0] = new GradientAlphaKey(1.0f, 0.0f);
        alphas[1] = new GradientAlphaKey(0.0f, 1.0f);

        gradient.SetKeys(colors, alphas);

        foreach (VisualEffect vfx in vfxList)
        {
            //Sets the value of the gradient created above
            vfx.SetGradient("SparksGradient", gradient);
        }

        if (randomColors)
        {
            primaryColor = _primaryColor;
            secondaryColor = _secondaryColor;
        }
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

        //Blow Up Override
        if (Input.GetKeyDown(KeyCode.H))
        {
            BlowUp(null);
        }
    }

    private void InputManager()
    {
        horizontalInput = playerInput.actions["Move"].ReadValue<Vector2>().x;   //turning input

        if (RaceManager.Instance.test)
        {
            verticalInput = playerInput.actions["Move"].ReadValue<Vector2>().y;     //accelaration input

            //Spin
            if (playerInput.actions["Spin"].WasPressedThisFrame() && grounded())
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
            if (playerInput.actions["Spin"].WasPressedThisFrame() && !grounded() && !flip && flipAvailable && !spin)
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

            //Nos
            if (horizontalInput != 0 || verticalInput != 0)
            {
                NosController();
            }
        }
        else if (alive)
        {
            verticalInput = playerInput.actions["Move"].ReadValue<Vector2>().y;     //accelaration input

            //Spin
            if (playerInput.actions["Spin"].WasPressedThisFrame() && grounded())
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
            if (playerInput.actions["Spin"].WasPressedThisFrame() && !grounded() && !flip && flipAvailable && !spin)
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

            //Nos
            if (horizontalInput != 0 || verticalInput != 0)
            {
                NosController();
            }
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
        ui.NosCounter = Mathf.FloorToInt(currentNos);
    }

    private void FlipBoost()
    {
        rb.AddForce(transform.forward * flipBoost, ForceMode.Impulse);
        flip = false;
    }

    //Controls the spin move
    private void SpinAction(bool startSpinning)
    {
        //Starts the first 180
        if (startSpinning)
        {
            //Sets spin bool to true and debuffs acceleration while reversing
            spin = true;
            accelaration = accelaration * spinSpeedDebuff;
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
                if (spinPressTime >= 1.0f)
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
    }

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
            if (RaceManager.Instance.test)
            {
                //Deactivates playerInput
                playerInput.DeactivateInput();
            }
            
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

            //Remove 1 life
            currentLives--;

            //Relays to UI
            LivesToUI();

            if (currentLives >= 0)
            {
                //Starts countdown until respawn
                StartCoroutine(RespawnCountdown(respawnManager.respawnDelay));
            }
            else
            {
                //Removes player from alive counter
                RaceManager.Instance.Alive--;
            }

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
    }

    private void RespawnToggle(bool toggle)
    {
        alive = toggle;

        //Toggles further deaths until respawn
        deathAvailable = toggle;

        if (RaceManager.Instance.test)
        {
            //Toggles the rigidbodies gravities so the dead player doesn't just fall through the ground
            carBody.useGravity = toggle;
            rb.useGravity = toggle;

            //Toggles the collider so nothing can collide with it while dead
            rb.GetComponent<SphereCollider>().enabled = toggle;
            carBody.GetComponent<BoxCollider>().enabled = toggle;
        }

        //Toggles meshes so they car is invisible while dead
        foreach (MeshRenderer meshRenderers in meshRendererList)
        {
            if (RaceManager.Instance.test)
            {
                meshRenderers.enabled = toggle;
            }
            else
            {
                if (toggle)
                {
                    //Toggles meshes so they car is invisible while dead
                    foreach (MeshRenderer mR in meshRendererList)
                    {
                        foreach (Material mat in mR.materials)
                        {
                            Color matColor = mat.color;
                            matColor.a = 255; // Set alpha to 0 (fully transparent)
                            mat.color = matColor;
                        }
                    }
                }
                else
                {
                    //Toggles meshes so they car is invisible while dead
                    foreach (MeshRenderer mR in meshRendererList)
                    {
                        foreach (Material mat in mR.materials)
                        {
                            Color matColor = mat.color;
                            matColor.a = 100; // Set alpha to 0 (fully transparent)
                            mat.color = matColor;
                        }
                    }
                }
            }
        }

        //Toggles visual effects
        foreach (VisualEffect visualEffect in vfxList)
        {
            visualEffect.enabled = toggle;
        }

        //Toggles PlayerPositionArrow
        playerPositionArrow.SetActive(toggle);

        //Toggles engine sound so the car doesn't make sounds while dead
        engineSound.enabled = toggle;
    }

    private void LivesToUI()
    {
        ui.LivesCounter = currentLives;
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
}
