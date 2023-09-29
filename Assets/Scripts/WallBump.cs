using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBump : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "MainPlayer")
        {
            Rigidbody riby = collision.gameObject.GetComponent<ArcadeVehicleController>().RiBy;

            // Get the normal vector of the wall's surface (assuming the wall has a Collider)
            Vector3 wallNormal = collision.contacts[0].normal;

            // Apply a force in the direction away from the wall
            riby.AddForce(-wallNormal * RaceManager.Instance.BumpForce, ForceMode.Impulse);

            // Calculate the torque direction to make the object rotate away from the wall
            Vector3 torqueDirection = Vector3.Cross(riby.velocity, wallNormal).normalized;
            riby.AddTorque(-torqueDirection * RaceManager.Instance.BumpTorque, ForceMode.Impulse);

        }
    }
}
