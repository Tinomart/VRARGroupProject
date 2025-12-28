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
    public static GameObject Instance;
    public int numOfArms = 15;
    public GameObject armPrefeb;
    public List<GameObject> arms = new List<GameObject>();

    private void Awake()
    {
        Instance = gameObject;
        for (int i = 0; i < numOfArms; i++)
        {
            GameObject obj = Instantiate(armPrefeb);
            if (obj.TryGetComponent<GalaxyArm>(out var arm))
            {
                arm.armName = "Arm " + i;
            }
            obj.transform.SetParent(transform);
            arms.Add(obj);
            arms[i].transform.localPosition = Vector3.zero;
            arms[i].transform.localRotation = Quaternion.Euler(Vector3.zero);
            arms[i].transform.Rotate(0, (360/numOfArms)*i, 0);
            arm.Setup();
        }   
        AudioManager.PlayAudioFrom(AudioManager.GalaxyAmbienceSource, gameObject);
    }
}

