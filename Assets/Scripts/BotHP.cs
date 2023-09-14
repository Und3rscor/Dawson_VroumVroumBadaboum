using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class BotHP : MonoBehaviour
{
    public int health;
    public GameObject explosionParticleFX;

    private void Update()
    {
        if (health <= 0)
        {
            Die();
        }
    }

    public void DealDamage(int damage)
    {
        health -= damage;
    }

    public void Die()
    {
        Instantiate(explosionParticleFX, transform.position, Quaternion.identity, null);
        //GameManager.Instance.Kill();
        Destroy(gameObject);
    }
}
