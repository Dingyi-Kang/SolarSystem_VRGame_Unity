using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LightColorController : MonoBehaviour
{
    public Animator animator;

    public Light light;
    public int picker;

    // Start is called before the first frame update
    void Start()
    {
        picker = 0; //Setting the default color
    }

    // Update is called once per frame
    void Update()
    {


        //Rotates through the color wheel
        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            picker = picker + 1;
        }

        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            picker = picker - 1;
        }

        //Keeps the picker variable within the color wheel
        if (picker >= 8)
        {
            picker = 0;
        } else if (picker < 0)
        {
            picker = 7;
        }

        //Allow colors to be modified
        if (picker != 0)
            animator.SetBool("Colors Shift", false);

        //Updates the color of the light and material of the sun 
        switch (picker)
        {

            case 0: 
                light.color = Color.white;
                animator.SetBool("Colors Shift", true);
                break;

            case 1: 
                light.color = Color.red;
                animator.SetFloat("Colors Tint", 0f); // Color.red;
                break;

            case 2:
                light.color = Color.magenta;
                animator.SetFloat("Colors Tint", (1f / 6f) * 1f); // Color.magenta;
                break;

            case 3:
                light.color = Color.blue;
                animator.SetFloat("Colors Tint", (1f / 6f) * 2f); // Color.blue;
                break;

            case 4:
                light.color = Color.cyan;
                animator.SetFloat("Colors Tint", (1f / 6f) * 3f); // Color.cyan;
                break;

            case 5:
                light.color = Color.green;
                animator.SetFloat("Colors Tint", (1f / 6f) * 4f); // Color.green;
                break;

            case 6:
                light.color = Color.yellow;
                animator.SetFloat("Colors Tint", (1f / 6f) * 5f); // Color.yellow;
                break;

            case 7:
                light.color = new Color(0.9f, 0.3f, 0f);  // Made a new orange color, as unity doesn't have one built in. Unity hates OSU and halloween confirmed??
                animator.SetFloat("Colors Tint", 1f); // Orange
                break;

            default:
                break;

        }

    }
}
