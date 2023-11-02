using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    [SerializeField] private GameObject objToLookAt;

    private void Update()
    {
        this.transform.LookAt(objToLookAt.transform);
    }
}
