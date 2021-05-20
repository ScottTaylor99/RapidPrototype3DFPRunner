using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public GameObject Player;

    //JumpPad
    public float jumpPadSpeed;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == Player)
        {
            Player.transform.parent = transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == Player)
        {
            Player.transform.parent = null;
        }
    }

    private void JumpPad()
    {
        Rigidbody rb = Player.GetComponent<Rigidbody>();
        rb.AddForce(Player.transform.up * jumpPadSpeed, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag == "Player")
        {
            Player = other.gameObject;
            JumpPad();
        }
    }
}
