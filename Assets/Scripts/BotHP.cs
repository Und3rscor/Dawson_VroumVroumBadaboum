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
            Instantiate(explosionParticleFX, transform.position, Quaternion.identity, null);
            Destroy(gameObject);
        }
    }

    public void DealDamage(int damage)
    {
        health -= damage;
    }
}
