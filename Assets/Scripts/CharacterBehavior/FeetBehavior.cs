using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeetBehavior : MonoBehaviour
{
    [SerializeField] private Animator playerAnim;

    public bool isGrounded = false;

    void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            isGrounded = true;
            playerAnim.SetBool("Grounded", true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.isTrigger)
        {
            isGrounded = false;
            playerAnim.SetBool("Grounded", false);
        }
    }
}
