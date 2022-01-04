using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OutlinesScript : MonoBehaviour
{
    public Animator animator;
    public InputActionAsset actions;
    public InputActionReference leftTouchpadClick;
    public InputActionReference rightTouchpadClick;
    private int outlinesHash;

    [SerializeField, Range(0.0f, 1.0f)]
    private float outlines = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        outlinesHash = Animator.StringToHash("Outlines");
    }

    // Update is called once per frame
    void Update()
    {
        float interval = Time.deltaTime;
        // If either touchpad is pressed down, have the outlines fade in over time or vice versa.
        if (leftTouchpadClick.action.ReadValue<float>() == 1f || rightTouchpadClick.action.ReadValue<float>() == 1f)
            outlines = Mathf.Lerp(0.0f, 1.0f, outlines + interval);
        else
            outlines = Mathf.Lerp(0.0f, 1.0f, outlines - interval);
        animator.SetFloat(outlinesHash, outlines);
    }
}
