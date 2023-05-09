using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class Countdown : MonoBehaviour
{
    public string[] Messages; //3, 2, 1, Go!
    public float interval = 1f;
    private TextMeshProUGUI countdownText = null;
    public List<Rigidbody> rigidbodies = new List<Rigidbody>();
    public GameObject machineGun;

    public IEnumerator Start()
    {
        countdownText = GetComponent<TextMeshProUGUI>();
        rigidbodies.AddRange(FindObjectsOfType<Rigidbody>());
        machineGun.GetComponent<MachineGun>().enabled = false;

        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = true;
        }

        int MessageDisplay = Messages.Length - 1;

        while (MessageDisplay >= 0)
        {
            //print(Messages[MessageDisplay]);
            countdownText.text = Messages[MessageDisplay];
            yield return new WaitForSeconds(interval);
            MessageDisplay -= 1;
        }

        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = false;
        }

        machineGun.GetComponent<MachineGun>().enabled = true;
        gameObject.SetActive(false);
    }
}
