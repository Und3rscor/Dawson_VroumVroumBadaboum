using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject menuObject;
    public GameObject inGameObject;

    // Start is called before the first frame update
    void Start()
    {
        menuObject.SetActive(true);
        inGameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
