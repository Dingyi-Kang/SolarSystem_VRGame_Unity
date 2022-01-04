using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LightFluc : MonoBehaviour
{

    public Light light;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        // Increases brightness of light
        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            light.intensity = light.intensity + 0.1f;
        }

        // Decreases brightness of light
        if (Keyboard.current.sKey.wasPressedThisFrame && light.intensity > 0.5f) //Prevents turning off sunlight
        {
            light.intensity = light.intensity - 0.1f;
        }

    }
}
