using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MachineGun : MonoBehaviour
{
    //Stats
    [Header("Stats")]
    [SerializeField] private int damage;
    [SerializeField] private int damageRandomRange;
    [SerializeField] private float shootForce;
    [Tooltip("Serves as range, bullet will despawn after this time has elapsed")]
    [SerializeField] private float bulletLifeTime;
    [SerializeField] private float spread;
    [SerializeField] private int heatPerShot;
    [SerializeField] private float shootDelay;

    //Setup stuff
    [Header("Setup")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform[] attackPoints;
    private Transform currentAttackPoint;
    [SerializeField] private GameObject muzzleFlash;
    private bool shooting, readyToShoot;

    //Relay stuff
    private ArcadeVehicleController carDaddy;
    private PlayerInput playerInput;

    private void Awake()
    {
        readyToShoot = true;
        currentAttackPoint = attackPoints[0];
        carDaddy = GetComponentInParent<ArcadeVehicleController>();
        playerInput = GetComponentInParent<PlayerInput>();
    }

    private void Update()
    {
        shooting = playerInput.actions["Shoot"].IsPressed();

        if (readyToShoot && shooting)
        {
            Shoot();
        }

        if (!carDaddy.Flip && !carDaddy.Spin)
        {
            Vector3 target = new Vector3(0f, carDaddy.transform.rotation.eulerAngles.y, 0f);
            transform.rotation = Quaternion.Euler(target);
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        //Grabs the car velocity to affect the speed of the bullet
        Vector3 carVelocity = GetComponentInParent<Rigidbody>().velocity;

        //Spawn bullet
        if (!carDaddy.Flip)
        {
            Vector3 newAttackpoint = currentAttackPoint.position + (carVelocity / 75);
            GameObject currentBullet = Instantiate(bullet, newAttackpoint, currentAttackPoint.rotation);
            Vector3 dir = currentAttackPoint.rotation * Vector3.forward;
            currentBullet.GetComponent<Rigidbody>().AddForce(dir.normalized * shootForce, ForceMode.Impulse);
            EditBullet(currentBullet);
        }
        else
        {
            GameObject currentBullet = Instantiate(bullet, currentAttackPoint.position, currentAttackPoint.rotation);
            Vector3 dir = Quaternion.Inverse(currentAttackPoint.rotation) * Vector3.forward;
            currentBullet.GetComponent<Rigidbody>().AddForce(dir.normalized * shootForce, ForceMode.Impulse);
            EditBullet(currentBullet);
        }
        
        //Instantiate muzzle flash
        if (muzzleFlash != null)
        {
            Instantiate(muzzleFlash, currentAttackPoint.position, Quaternion.identity);
        }

        Invoke("ResetShot", shootDelay);
    }

    private void EditBullet(GameObject currentBullet)
    {
        currentBullet.GetComponent<Bullet>().damage = damage + Random.Range(-damageRandomRange, damageRandomRange);
        currentBullet.GetComponent<Bullet>().source = GetComponentInParent<ArcadeVehicleController>();
        currentBullet.transform.parent = null;
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
