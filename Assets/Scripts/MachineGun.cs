using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MachineGun : MonoBehaviour
{
    //Gun stuff
    [Header("GunStuff")]
    [SerializeField] private KeyCode shootKey = KeyCode.Mouse0;
    [SerializeField] private Transform[] attackPoints;
    private Transform attackPoint;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private TextMeshProUGUI ammoDisplay;
    [SerializeField] private float timeBetweenShooting;
    [SerializeField] private int maxAmmoCount = 100;
    private int currentAmmoCount;
    private bool shooting, readyToShoot;

    //Relay stuff
    private Rigidbody rb;
    private ArcadeVehicleController carDaddy;

    //Bullet stuff
    [Header("Bullet stuff")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private float shootForce;
    [SerializeField] private int damage;
    [SerializeField] private int damageRandomRange;

    private void Awake()
    {
        readyToShoot = true;
        attackPoint = attackPoints[0];
        currentAmmoCount = maxAmmoCount;
        carDaddy = GetComponentInParent<ArcadeVehicleController>();
    }

    private void Update()
    {
        shooting = Input.GetKey(shootKey);

        if (readyToShoot && shooting && currentAmmoCount > 0)
        {
            Shoot();
        }

        if (ammoDisplay != null)
        {
            ammoDisplay.SetText(currentAmmoCount.ToString());
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
        currentAmmoCount--;

        //Instantiate bullet
        Vector3 carVelocity = GetComponentInParent<Rigidbody>().velocity;

        //Spawn bullet
        if (!carDaddy.Flip)
        {
            Vector3 newAttackpoint = attackPoint.position + (carVelocity / 75);
            GameObject currentBullet = Instantiate(bullet, newAttackpoint, attackPoint.rotation);
            Vector3 dir = attackPoint.rotation * Vector3.forward;
            currentBullet.GetComponent<Rigidbody>().AddForce(dir.normalized * shootForce, ForceMode.Impulse);
            EditBullet(currentBullet);
        }
        else
        {
            GameObject currentBullet = Instantiate(bullet, attackPoint.position, attackPoint.rotation);
            Vector3 dir = Quaternion.Inverse(attackPoint.rotation) * Vector3.forward;
            currentBullet.GetComponent<Rigidbody>().AddForce(dir.normalized * shootForce, ForceMode.Impulse);
            EditBullet(currentBullet);
        }
        
        //Instantiate muzzle flash
        if (muzzleFlash != null)
        {
            Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
        }

        Invoke("ResetShot", timeBetweenShooting);
    }

    private void EditBullet(GameObject currentBullet)
    {
        currentBullet.GetComponent<Bullet>().damage = damage + Random.Range(-damageRandomRange, damageRandomRange);
        currentBullet.transform.parent = null;
    }

    private void ResetShot()
    {
        readyToShoot = true;

        if (attackPoint == attackPoints[0])
        {
            attackPoint = attackPoints[1];
        }
        else
        {
            attackPoint = attackPoints[0];
        }
    }

    public void RefillAmmo()
    {
        currentAmmoCount = maxAmmoCount;
    }
}
