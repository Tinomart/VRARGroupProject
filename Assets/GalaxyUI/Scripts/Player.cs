using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Player : MonoBehaviour
{
    public static GameObject LeftHand;
    public static GameObject RightHand;
    public GameObject leftHand;
    public GameObject rightHand;

    private XRDirectInteractor leftHandGrabInteractor;
    private XRDirectInteractor rightHandGrabInteractor;

    private XRGrabInteractable leftHandHoveredInteractable;
    private XRGrabInteractable rightHandHoveredInteractable;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (leftHand == null)
        {
            
            Debug.LogError("Left Hand GameObject not assigned to Player");
        } else
        {
            leftHandGrabInteractor = leftHand.GetComponentInChildren<XRDirectInteractor>();
        }
        
        if (rightHand == null)
        {
            Debug.LogError("Right Hand GameObject not assigned to Player");
        } else
        {
            rightHandGrabInteractor = rightHand.GetComponentInChildren<XRDirectInteractor>();
        }
        LeftHand = leftHand;
        RightHand = rightHand;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHoveredInteractable(leftHandGrabInteractor);
        UpdateHoveredInteractable(rightHandGrabInteractor, false);
    }
    

    void UpdateHoveredInteractable(XRDirectInteractor hand, bool isRightHand = true)
    {
        
        
        var validTargets = new List<IXRInteractable>();
        hand.GetValidTargets(validTargets);
        if (validTargets.Count == 0)
        {
            return;
        }
        
        var starsOnly = validTargets
            .Where(target => target is MonoBehaviour mb && mb.TryGetComponent(out Star star))
            .ToList();

        if (starsOnly.Count == 0)
        {
            if (isRightHand)
            {
                if (rightHandHoveredInteractable)
                {
                    if(rightHandHoveredInteractable.TryGetComponent(out Star star))
                    {
                        star.MainHoverExit();
                    }
                    rightHandHoveredInteractable = null;
                
                }
            }
            else
            {
                if (leftHandHoveredInteractable)
                {
                    if(leftHandHoveredInteractable.TryGetComponent(out Star star))
                    {
                        star.MainHoverExit();
                    }
                    leftHandHoveredInteractable = null;
                }
            }
            return;
        }
        
        if (isRightHand)
        {
            
            XRGrabInteractable newInteractable = (XRGrabInteractable)validTargets[0];
            if (rightHandHoveredInteractable)
            {
                if (rightHandHoveredInteractable == newInteractable)
                {
                    return;
                }
            }
            
            newInteractable.gameObject.TryGetComponent<Star>(out var star);
            if (star)
            {
                if (rightHandHoveredInteractable)
                {
                    rightHandHoveredInteractable.TryGetComponent<Star>(out var oldStar);
                    if (oldStar)
                    {
                        oldStar.MainHoverExit();
                    }
                }
                
                
                star.MainHoverEnter();
                rightHandHoveredInteractable = newInteractable;
            }
        }
        else
        {
            XRGrabInteractable newInteractable = (XRGrabInteractable)validTargets[0];
            if (leftHandHoveredInteractable)
            {
                if (leftHandHoveredInteractable == newInteractable)
                {
                    return;
                }
            }
            
            newInteractable.gameObject.TryGetComponent<Star>(out var star);
            if (star)
            {
                if (leftHandHoveredInteractable)
                {
                    leftHandHoveredInteractable.TryGetComponent<Star>(out var oldStar);
                    if (oldStar)
                    {
                        oldStar.MainHoverExit();
                    }
                }
                
                
                star.MainHoverEnter();
                leftHandHoveredInteractable = newInteractable;
            }
        }
        
        
            
        
    }
    
    public static void SendHapticsToHand(
        bool rightHand,
        float amplitude = 1f,
        float duration = 0.15f)
    {
        InputDeviceCharacteristics hand =
            rightHand
                ? InputDeviceCharacteristics.Right
                : InputDeviceCharacteristics.Left;

        var devices = new List<InputDevice>();

        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.Controller | hand,
            devices
        );

        foreach (var device in devices)
        {
            if (!device.isValid)
                continue;

            if (device.TryGetHapticCapabilities(out var caps) &&
                caps.supportsImpulse)
            {
                device.SendHapticImpulse(
                    channel: 0,
                    amplitude: amplitude,
                    duration: duration
                );
            }
        }
    }
}
