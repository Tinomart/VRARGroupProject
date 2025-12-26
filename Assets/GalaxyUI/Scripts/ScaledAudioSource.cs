using System;
using UnityEngine;

public class ScaledAudioSource : MonoBehaviour
{
    
    [HideInInspector] public AudioSource audioSource;
    private float baseScale = 1.0f;
    private float baseMinDistance = 1f;
    private float baseMaxDistance = 50f;

    private void Start()
    {
        baseMaxDistance = audioSource.maxDistance;
        baseMinDistance = audioSource.minDistance;
        baseScale = transform.lossyScale.magnitude / Mathf.Sqrt(3);
    }

    private void Update()
    {
        float updatedScale = transform.lossyScale.magnitude / Mathf.Sqrt(3);
        audioSource.minDistance = baseMinDistance * updatedScale/baseScale;
        audioSource.maxDistance = baseMaxDistance * updatedScale/baseScale;
    }
}
