using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    [SerializeField] float m_MinDistanceForInteractions = 2.0f;
    [SerializeField] int m_LayerMask; 
    [SerializeField] bool m_IsPlayerInFrontOfInteractable = false; 

    [SerializeField] GameEvent m_PlayerInteractWithCallEscalatorButton;
    [SerializeField] GameEvent m_PlayerInteractWithCloseEscalatorButton;
    [SerializeField] GameEvent m_PlayerInFrontOfInteractable;
    [SerializeField] GameEvent m_PlayerAwayFromInteractable;

    FirstPersonController fps;
    RaycastHit hit;
    public bool pilotFlight = false;

    public GameEvent playerEntredInFlight;

    private void Awake()
    {
        fps = GetComponent<FirstPersonController>();
    }

    private void Update()
    {



        if (!Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, m_MinDistanceForInteractions))
        {
            if (m_IsPlayerInFrontOfInteractable)
                m_PlayerAwayFromInteractable.Raise(this, "");

            m_IsPlayerInFrontOfInteractable = false;
            
            return; 
        }

        if (hit.collider.gameObject.layer == 9)
        {

            Plane plane = hit.transform.root.GetComponentInChildren<Plane>();

            if (plane == null)
                return; 

            m_PlayerInFrontOfInteractable.Raise(this, "Appuyez sur la touche 'E' pour entrer dans l'avion.");

            if (Input.GetKeyUp(KeyCode.E))
            {

                var meshRenderer = transform.GetComponentInChildren<MeshRenderer>();

                if (meshRenderer != null)
                    meshRenderer.enabled = false;

                fps.enabled = false;

                // Make the player's transform a child of the plane's transform
                transform.parent = plane.gameObject.transform;


                // Optionally, you might want to reset the local position and rotation of the player to align with the plane
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;

                plane.playerIn = true;

                playerEntredInFlight.Raise(this);

            }

        }

        if (hit.collider.TryGetComponent(out CallEscalatorBtn caller))
        {
            m_PlayerInFrontOfInteractable.Raise(this, "Appuyez sur la touche 'E' pour appeler l'ascenceur.");


            if (Input.GetKeyUp(KeyCode.E)) {
                caller.CallEscalator();

            }

        }
        else if (hit.collider.transform.TryGetComponent(out CloseDoorEscalatorBtn closer))
        {
            m_PlayerInFrontOfInteractable.Raise(this, "Appuyez sur la touche 'E' pour monter au sommet de la tour.");

            if (Input.GetKeyUp(KeyCode.E))
            {
                Escalator escalator = closer.GetComponentInParent<Escalator>();

                if (escalator != null)
                {
                    closer.CloseDoor();
                }


            }
        }

        m_IsPlayerInFrontOfInteractable = true;

    }
}
