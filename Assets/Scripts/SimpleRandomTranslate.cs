using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRandomTranslate : MonoBehaviour
{
    public bool translateOnY = true;
    public float minY = 0f;            // Minimum Y-axis value
    public float maxY = 5f;            // Maximum Y-axis value
    public float speed = 2f;           // Translation speed
    public float interval = 2f;        // Time interval between translations

    private float timeSinceLastMovement = 0f;  // Time since the last movement

    


    void OnEnable()
    {
        timeSinceLastMovement = Time.time; // Initialize the movement
    }

    
   void Update()
   {
        if (Time.time - timeSinceLastMovement >= interval)
        {
            timeSinceLastMovement = Time.time; // Reset the timer

            if (translateOnY)
            {
                // Move in the opposite direction each time the interval is reached
                float targetY = (transform.position.y <= minY) ? maxY : minY;
                StartCoroutine(MoveTo(targetY));
            }
        }
    }

    private IEnumerator MoveTo(float targetY)
    {
        while (Mathf.Abs(transform.position.y - targetY) > 0.01f)
        {
            float newY = Mathf.MoveTowards(transform.position.y, targetY, speed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            yield return null;
        }
    }
}
