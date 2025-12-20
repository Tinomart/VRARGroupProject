using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// Locks rotation on specified axes for both single and two-handed grabs
/// Add this to your XR Grab Interactable GameObject
/// </summary>

public class Galaxy : MonoBehaviour
{
    public int numOfArms = 15;
    public GameObject armPrefeb;
    public List<GameObject> arms = new List<GameObject>();
    public GameObject overheadGesture;
    private GestureControl _gestureControl;

    private void Awake()
    {
        for (int i = 0; i < numOfArms; i++)
        {
            GameObject obj = Instantiate(armPrefeb);
            if (obj.TryGetComponent<GalaxyArm>(out var arm))
            {
                // Yes, this GameObject contains a GalaxyArm component
                arm.Setup();
            }
            obj.transform.SetParent(transform);
            arms.Add(obj);
            arms[i].transform.Translate(0,0,0);
            arms[i].transform.Rotate(0, (360/numOfArms)*i, 0);
        }   
        
        _gestureControl = overheadGesture.GetComponent<GestureControl>();
    }

    
}

