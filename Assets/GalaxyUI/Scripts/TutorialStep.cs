using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using InputDevice = UnityEngine.XR.InputDevice;

public abstract class TutorialStep : MonoBehaviour
{
    public int stepNumber = 0;
    [HideInInspector] public UnityEvent stepComplete;
    [HideInInspector] public UnityEvent stepAbort;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void BeginStep()
    {
        StartCoroutine(TutorialSequence());
    }

    abstract public IEnumerator TutorialSequence();

    // Update is called once per frame
    void Update()
    {
        
    }
    public IEnumerator WaitForInputAction(InputAction action)
    {
        action.Enable();
        bool performed = false;
        action.performed += _ => performed = true;
        
        yield return new WaitUntil(() => performed);
    }
    
}
