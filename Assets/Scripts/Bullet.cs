using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.transform.tag == "Bot")
        {
            coll.gameObject.GetComponentInParent<BotHP>().DealDamage(damage);
        }

        Destroy(gameObject);
    }
}
