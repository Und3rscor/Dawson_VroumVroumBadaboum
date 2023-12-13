using Mono.CompilerServices.SymbolWriter;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private GameObject impactVFX;
    private AudioSource impactSound;
    private float lifespan;
    private int damage;
    private ArcadeVehicleController source;

    public void Setup(float newLifespan, int newDamage, ArcadeVehicleController newSource)
    {
        //Sets the new stats
        lifespan = newLifespan;
        damage = newDamage;
        source = newSource;

        //Starts the lifespan death timer
        Invoke("DestroyObj", lifespan);
        
        //Grabs the variables for if it collides with an enemy
        impactSound = GetComponent<AudioSource>();
    }

    //Plays the sfx before destroying
    private void Die()
    {
        impactSound.Play();

        Instantiate(impactVFX);

        GetComponentInChildren<Collider>().enabled = false;

        StartCoroutine(DieAfterSoundEffect());
    }

    //Destroys the projectile after sfx
    IEnumerator DieAfterSoundEffect()
    {
        yield return new WaitForSeconds(impactSound.clip.length);

        DestroyObj();
    }

    //Destroys the projectile
    private void DestroyObj()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider coll)
    {
        //Makes sure the collision isn't with the shooter
        if (coll.gameObject.GetComponentInParent<ArcadeVehicleController>() != source)
        {
            //If the projectile collides with a player
            if (coll.transform.tag == "MainPlayer")
            {
                //Grabs avc
                var avc = coll.gameObject.GetComponentInParent<ArcadeVehicleController>();

                //Deals damage
                avc.TakeDamage(damage, source);

                //Gives score
                source.UI.AddScore(5);

                //Switches latest damage source
                avc.latestDamageIsProjectile = true;

                //Plays the sfx before destroying the projectile
                Die();
            }
            //If the colliding object is the PP volume, don't collide with it. Lol
            else if (coll.transform.tag == "Volume")
            {
                //Do Nothing
            }
            //If the projectile collides with the environment
            else
            {
                DestroyObj();
            }
        } 
    }
}
