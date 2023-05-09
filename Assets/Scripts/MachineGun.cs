using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private int ammoCount = 100;
    private bool shooting, readyToShoot;

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
    }

    private void Update()
    {
        shooting = Input.GetKey(shootKey);

        if (readyToShoot && shooting && ammoCount > 0)
        {
            Shoot();
        }

        if (ammoDisplay != null)
        {
            ammoDisplay.SetText(ammoCount.ToString());
        }
    }

    private void Shoot()
    {
        readyToShoot = false;
        ammoCount--;

        //Instantiate bullet
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, attackPoint.rotation);
        Vector3 dir = attackPoint.rotation * Vector3.forward;
        currentBullet.GetComponent<Rigidbody>().AddForce(dir.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Bullet>().damage = damage + Random.Range(-damageRandomRange, damageRandomRange);
        currentBullet.transform.parent = null;

        //Instantiate muzzle flash
        if (muzzleFlash != null)
        {
            Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
        }

        Invoke("ResetShot", timeBetweenShooting);
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
}