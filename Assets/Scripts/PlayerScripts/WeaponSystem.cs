using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using MoreMountains.Feedbacks;
using UnityEngine.Events;

public class WeaponSystem : MonoBehaviour
{
    #region Fields
    //Stats
    [Header("Stats")]
    [SerializeField] private int damage;
    [SerializeField] private int damageRandomRange;
    [SerializeField] private float shootForce;
    [Tooltip("Serves as range, bullet will despawn after this time has elapsed")]
    [SerializeField] private float projectileLifespan;
    [SerializeField] private float spread;
    [SerializeField] private int heatPerShot;
    [SerializeField] private float shootDelay;

    //Setup stuff
    [Header("Setup")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private AudioSource shootSound;
    [SerializeField] private Transform[] attackPoints;
    private Transform currentAttackPoint;
    private bool shooting, readyToShoot;

    //Relay stuff
    private ArcadeVehicleController carDaddy;
    private PlayerInput playerInput;
    private UI ui;

    //Feedback Systems ------------
    [Header("Feedbacks")]
    public MMF_Player ShootFeedback;
    public MMF_Player HeatBarFeedback;

    #endregion

    private void Awake()
    {
        readyToShoot = true;
        currentAttackPoint = attackPoints[0];
        carDaddy = GetComponentInParent<ArcadeVehicleController>();
        playerInput = GetComponentInParent<PlayerInput>();
        shootSound = GetComponent<AudioSource>();
        ui = carDaddy.UI;
    }

    private void Update()
    {
        shooting = playerInput.actions["Shoot"].IsPressed();

        if (readyToShoot && shooting && heatPerShot <= (100 - carDaddy.Heat))
        {
            Shoot();
        }
        
        if (!carDaddy.Flip)
        {
            AdjustRotation();
        }
    }

    private void AdjustRotation()
    {
        // Get the current rotation
        Vector3 currentRotation = transform.eulerAngles;

        // Set the X rotation to 0
        currentRotation.x = -90f;

        // Apply the new rotations
        transform.eulerAngles = currentRotation;

        if (transform.localEulerAngles.z > 1.0f || transform.localEulerAngles.z < -1.0f)
        {
            Vector3 currentLocalRotation = transform.localEulerAngles;

            currentLocalRotation.y = 0f;
            currentLocalRotation.z = 0f;

            transform.localEulerAngles = currentLocalRotation;
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        //Creates new attackPoint a little further ahead than the car so the projectile doesn't collide with the car
        Vector3 attackPoint = currentAttackPoint.position + -transform.up * 1.25f;

        //Spawns the projectile
        SpawnProjectile(attackPoint, Quaternion.LookRotation(-transform.up));

        //Do gunshot
        if (shootSound != null)
        {
            shootSound.Play();
        }
        
        //Instantiate muzzle flash
        if (muzzleFlash != null)
        {
            //Instantiate(muzzleFlash, currentAttackPoint.position, Quaternion.identity);   // Desactivated Flash handdle with MMF_Player.ShootFeedback
        }

        //Add shot heat to the car's heat
        carDaddy.Heat += heatPerShot;

        Invoke("ResetShot", shootDelay);


        // Call Feedback System
        ShootFeedback?.PlayFeedbacks();  
        HeatBarFeedback?.PlayFeedbacks();

    }

    private void SpawnProjectile(Vector3 attackPoint, Quaternion attackRotation)
    {
        //Instantiate bullet
        GameObject currentProjectile = Instantiate(projectile, attackPoint, attackRotation);

        //Grabs projectile script to apply stats
        Projectile currentProjectileScript = currentProjectile.GetComponent<Projectile>();

        //Sets up projectile stats
        int damageWithRandomValues = damage + Random.Range(-damageRandomRange, damageRandomRange);  //Damage
        ArcadeVehicleController source = GetComponentInParent<ArcadeVehicleController>();           //Source

        //Applies stats
        currentProjectileScript.Setup(projectileLifespan, damageWithRandomValues, source);

        //Spread
        float spreadX = Random.Range(-spread, spread);  //Random x spread
        float spreadY = Random.Range(-spread, spread);  //Random y spread  
        float spreadZ = Random.Range(-spread, spread);  //Random z spread
        Vector3 currentSpread = new Vector3(spreadX, spreadY, spreadZ) / 100; //Creates the spread value divided by 100 for ease of manipulation with the inspector

        //Sets the target force direction
        Vector3 targetDirection = -transform.up + currentSpread;

        //Applies force to the bullet so it flies in the direction provided plus spread
        currentProjectile.GetComponent<Rigidbody>().AddForce(targetDirection * shootForce, ForceMode.Impulse);

        //Remove parent
        currentProjectile.transform.parent = null;
    }

    //Reenables shooting and switches attackPoint
    private void ResetShot()
    {
        readyToShoot = true;

        if (attackPoints.Length > 1)
        {
            //Cycles through provided attackPoints
            int currentIndex = System.Array.IndexOf(attackPoints, currentAttackPoint);
            currentIndex = (currentIndex + 1) % attackPoints.Length;
            currentAttackPoint = attackPoints[currentIndex];
        }
    }
}
