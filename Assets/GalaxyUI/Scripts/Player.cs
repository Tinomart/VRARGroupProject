using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

public class Player : MonoBehaviour
{
    public static GameObject LeftHand;
    public static GameObject RightHand;
    public GameObject leftHand;
    public GameObject rightHand;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (leftHand == null)
        {
            Debug.LogError("Left Hand GameObject not assigned to Player");
        }
        if (rightHand == null)
        {
            Debug.LogError("Right Hand GameObject not assigned to Player");
        }
        LeftHand = leftHand;
        RightHand = rightHand;
    }

    // Update is called once per frame
    void Update()
    {
        
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
