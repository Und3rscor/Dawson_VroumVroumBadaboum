using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class BotHP : MonoBehaviour
{
    public int health;
    public GameObject explosionParticleFX;

    public void TakeDamage(int damage, ArcadeVehicleController source)
    {
        health -= damage;

        if (health <= 0)
        {
            Die(source);
        }
    }

    public void Die(ArcadeVehicleController source)
    {
        Instantiate(explosionParticleFX, transform.position, Quaternion.identity, null);
        GameManager.Instance.Alive--;

        if (source != null)
        {
            source.UI.Kill();
        }
        
        Destroy(gameObject);
    }
}
