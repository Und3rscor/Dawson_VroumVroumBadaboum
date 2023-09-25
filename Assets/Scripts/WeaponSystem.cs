using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSystem : MonoBehaviour
{
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
        
        if (carDaddy.grounded())
        {
            // Get the current rotation
            Vector3 currentRotation = transform.eulerAngles;

            // Set the X rotation to 0
            currentRotation.x = 0f;

            // Apply the new rotations
            transform.eulerAngles = currentRotation;

            if (transform.localEulerAngles.y > 1.0f || transform.localEulerAngles.y < -1.0f)
            {
                Vector3 currentLocalRotation = transform.localEulerAngles;

                currentLocalRotation.y = 0f;
                currentLocalRotation.z = 0f;

                transform.localEulerAngles = currentLocalRotation;
            }
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        //Creates new attackPoint a little further ahead than the car so the projectile doesn't collide with the car
        Vector3 attackPoint = currentAttackPoint.position + transform.TransformDirection(Vector3.forward) * 1.25f;

        //Spawns the projectile
        SpawnProjectile(attackPoint, currentAttackPoint.rotation);

        //Do gunshot
        if (shootSound != null)
        {
            shootSound.Play();
        }
        
        //Instantiate muzzle flash
        if (muzzleFlash != null)
        {
            Instantiate(muzzleFlash, currentAttackPoint.position, Quaternion.identity);
        }

        //Add shot heat to the car's heat
        carDaddy.Heat += heatPerShot;

        Invoke("ResetShot", shootDelay);
    }

    private void SpawnProjectile(Vector3 attackPoint, Quaternion attackRotation)
    {
        //Instantiate bullet
        GameObject currentProjectile = Instantiate(projectile, attackPoint, attackRotation);

        //Grabs projectile script to apply stats
        Projectile currentProjectileScript = currentProjectile.GetComponent<Projectile>();

        //Apply Stats
        currentProjectileScript.damage = damage + Random.Range(-damageRandomRange, damageRandomRange);  //Damage
        currentProjectileScript.source = GetComponentInParent<ArcadeVehicleController>();               //Source
        currentProjectileScript.lifespan = projectileLifespan;                                          //Lifespan

            //Spread
            float spreadX = Random.Range(-spread, spread);  //Random x spread
            float spreadY = Random.Range(-spread, spread);  //Random y spread  
            float spreadZ = Random.Range(-spread, spread);  //Random z spread
            Vector3 currentSpread = new Vector3(spreadX, spreadY, spreadZ) / 100; //Creates the spread value divided by 100
            
        //Adds force to the bullet so it flies in the direction provided plus spread
        currentProjectile.GetComponent<Rigidbody>().AddForce((transform.TransformDirection(Vector3.forward) + currentSpread) * shootForce, ForceMode.Impulse);

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
