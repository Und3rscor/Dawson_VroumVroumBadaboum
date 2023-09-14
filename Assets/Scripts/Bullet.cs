using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector] public float lifespan;
    [HideInInspector] public int damage;
    [HideInInspector] public ArcadeVehicleController source;

    private void Awake()
    {
        Invoke("Die", lifespan);
    }

    private void Die()
    {
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider coll)
    {
        if (coll.transform.tag == "Bot")
        {
            coll.gameObject.GetComponentInParent<BotHP>().DealDamage(damage);
        }

        if (coll.transform.tag == "MainPlayer")
        {
            coll.gameObject.GetComponentInParent<ArcadeVehicleController>().TakeDamage(damage, source);
        }

        Die();
    }
}
