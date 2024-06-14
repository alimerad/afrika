using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallEscalatorBtn : MonoBehaviour
{
    [SerializeField] GameEvent m_CallEscalator;
    public void CallEscalator()
    {
       m_CallEscalator.Raise(transform.root);
    }
}
