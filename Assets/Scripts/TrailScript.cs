using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailScript : MonoBehaviour
{

    TrailRenderer trail;

    // Start is called before the first frame update
    void Start()
    {
        //get reference to the current trail component
        trail = GetComponent<TrailRenderer>();
        
    }

    //restart the trail and make it visible
    public void Restart() {
        
        trail.Clear();

        trail.enabled = true;
    }

}
