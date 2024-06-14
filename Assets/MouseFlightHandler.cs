using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFlightHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    public void OnPlayerEnteredInFlight(Component sender, object data)
    {
        var mfc =  GetComponent<MouseFlightController>(); 
        mfc.enabled = true;

        Camera.main.transform.parent = null;

        //mfc.cameraRig = Camera.main.transform;
    }
}
