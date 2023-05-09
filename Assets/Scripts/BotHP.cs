using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class BotHP : MonoBehaviour
{
    public int health;
    public GameObject explosionParticleFX;
    public AudioSource shotSound;

    private void Update()
    {
        if (health <= 0)
        {
            Instantiate(explosionParticleFX, transform.position, Quaternion.identity, null);
            GameManager.Instance.Kill();
            Destroy(gameObject);
        }
    }

    public void DealDamage(int damage)
    {
        health -= damage;
        shotSound.Play();
    }
}
