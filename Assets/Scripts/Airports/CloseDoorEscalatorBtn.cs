using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseDoorEscalatorBtn : MonoBehaviour
{
    [SerializeField] GameEvent m_CloseDoorBtn;

    public void CloseDoor ()
    {
        m_CloseDoorBtn.Raise(transform.root);
    }
} 
