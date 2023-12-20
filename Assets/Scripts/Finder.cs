using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finder : MonoBehaviour
{
    private GameObject obj = null;

    // Update is called once per frame
    void Update()
    {
        //obj = FindObjectOfType<PlayerSelection>().gameObject;

        if (obj != null )
        {
            Debug.Log(obj.gameObject.name);
        }
    }
}
