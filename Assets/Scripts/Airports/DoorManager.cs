using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class DoorManager : MonoBehaviour
{
    [SerializeField] bool m_IsCurrentlyUsed = false;

    [SerializeField] bool m_DoorClosed;

    [SerializeField] bool m_EscalatorIsMoving = false;

    [SerializeField] GameEvent m_DoorHasBeenClosed; 
    public void OnCalledEscalator(Component sender, object data)
    {
        if (sender != transform.root) return;

        Animation animation = GetComponent<Animation>();

        animation.clip = animation.GetClip("OpenEscalatorDoor");

        animation.Play();

        m_IsCurrentlyUsed = true;
    }

    public void OnCloseEscalator(Component sender, object data)
    {
        if (sender != transform.root) return;

        Animation animation = GetComponent<Animation>();

        animation.clip = animation.GetClip("CloseEscalatorDoor");

        animation.Play();
    }


    public void DoorOpened()
    {
        if (!m_IsCurrentlyUsed) return; 
    }

    public void DoorClosed ()
    {
        if (!m_IsCurrentlyUsed) return;

        m_DoorHasBeenClosed.Raise(transform.root);
    }
}
