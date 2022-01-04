using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpeedScript : MonoBehaviour
{
    public Animator animator;
    public InputActionAsset actions;
    public InputActionReference leftTrigger;
    public InputActionReference leftTriggerPress;
    public InputActionReference rightTrigger;
    public InputActionReference rightTriggerPress;

    [SerializeField, Range(0f, 1f)]
    private float speed = 0f;

    private int speedHash;
    private int speedPercentageHash;

    // Start is called before the first frame update
    void Start()
    {
        speedHash = Animator.StringToHash("Speed (#)");
        speedPercentageHash = Animator.StringToHash("Speed (%)");
    }

    // Update is called once per frame
    void Update()
    {
        // Take the greater value of either controller's trigger.
        if (rightTriggerPress.action.ReadValue<float>() == 1f || leftTriggerPress.action.ReadValue<float>() == 1f)
            speed = 1f;
        else
            speed = Mathf.Max(leftTrigger.action.ReadValue<float>(), rightTrigger.action.ReadValue<float>());

        // Use it to set the speed parameters.
        animator.SetFloat(speedHash, (speed * 9f) + 1f);
        animator.SetFloat(speedPercentageHash, speed);     
    }
}
