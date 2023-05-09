using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    private MeshCollider mColl;

    private void Awake()
    {
        mColl = GetComponentInChildren<MeshCollider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            collision.gameObject.GetComponentInParent<BotHP>().DealDamage(damage);
            Debug.Log(collision.gameObject.transform.position);
            Time.timeScale = 0.0f;
        }

        Destroy(gameObject);
    }
}
