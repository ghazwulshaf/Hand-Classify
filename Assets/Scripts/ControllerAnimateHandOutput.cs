using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ControllerAnimateHandOutput : MonoBehaviour
{
    public InputActionProperty pinchAnimationAction;
    public InputActionProperty gripAnimationAction;
    public Animator handAnimator;

    void Update()
    {
        float triggerValue = pinchAnimationAction.action.ReadValue<float>();
        handAnimator.SetFloat("Trigger", triggerValue);

        float gridValue = gripAnimationAction.action.ReadValue<float>();
        handAnimator.SetFloat("Grip", gridValue);
    }
}
