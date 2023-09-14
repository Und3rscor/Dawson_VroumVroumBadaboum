using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public float lifespan;
    [HideInInspector] public int damage;
    [HideInInspector] public ArcadeVehicleController source;

    private AudioSource impactSound;

    private void Awake()
    {
        Invoke("DestroyObj", lifespan);

        impactSound = GetComponent<AudioSource>();
    }

    private void DestroyObj()
    {
        Destroy(gameObject);
    }

    private void Die()
    {
        impactSound.Play();

        GetComponentInChildren<Collider>().enabled = false;

        StartCoroutine(DieAfterSoundEffect());
    }

    IEnumerator DieAfterSoundEffect()
    {
        yield return new WaitForSeconds(impactSound.clip.length);

        DestroyObj();
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.transform.tag == "MainPlayer")
        {
            coll.gameObject.GetComponentInParent<ArcadeVehicleController>().TakeDamage(damage, source);
            Die();
        }
        else if (coll.transform.tag == "Bot")
        {
            coll.gameObject.GetComponentInParent<BotHP>().TakeDamage(damage, source);
            Die();
        }
        else
        {
            DestroyObj();
        }        
    }
}
