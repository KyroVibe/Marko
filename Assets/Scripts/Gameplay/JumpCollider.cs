using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpCollider : MonoBehaviour
{
    public bool Triggered { get; private set; }

    public void OnTriggerStay(Collider other)
    {
        Triggered = true;
    }

    public void OnTriggerExit(Collider other)
    {
        Triggered = false;
    }
}
