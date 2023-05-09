using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifespan;
    [HideInInspector] public int damage;

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

        Die();
    }
}
