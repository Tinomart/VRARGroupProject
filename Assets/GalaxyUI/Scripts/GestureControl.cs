using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class GestureControl : MonoBehaviour
{
    public Camera playerCamera;
    public Vector3 offset = new Vector3(0f, 0.7f, 0f);
    public GameObject galaxy;
    
    private List<XRDirectInteractor> _interactors = new List<XRDirectInteractor>();
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out XRDirectInteractor grabInteractor))
        {
            _interactors.Add(grabInteractor);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out XRDirectInteractor grabInteractor))
        {
            if (_interactors.Contains(grabInteractor))
            {
                _interactors.Remove(grabInteractor);
            }
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        transform.position = playerCamera.transform.position + offset;
        if (Physics.Raycast(playerCamera.transform.position,
                playerCamera.transform.forward,
                out RaycastHit hit))
        {
            if (_interactors.Count > 0)
            {
                var grabInteractor = _interactors[0];
                if (grabInteractor.isSelectActive)
                {
                    SpawnGalaxy(grabInteractor);
                }
            }
        }
    }
    
    private void SpawnGalaxy(XRDirectInteractor grabInteractor)
    {
        if (galaxy.activeInHierarchy) return;
        galaxy.SetActive(true);
        transform.position = grabInteractor.transform.position;
    }
    
}
