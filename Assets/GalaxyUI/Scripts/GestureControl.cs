using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class GestureControl : MonoBehaviour
{
    public Camera playerCamera;
    
    [SerializeField]
    private Vector3 offset = new Vector3(0f, 0.5f, 0f);
    [SerializeField]
    private float galaxyBaseScale = 0.2f;
    [SerializeField]
    private Vector3 galaxyOffset = new Vector3(0f, 0.5f, 0f);
    public GameObject galaxy;
    private XRGrabInteractable galaxyGrabInteractable;
    
    
    private List<XRDirectInteractor> _interactors = new List<XRDirectInteractor>();

    void Start()
    {
        galaxyGrabInteractable = galaxy.GetComponent<XRGrabInteractable>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out XRDirectInteractor grabInteractor))
        {
            _interactors.Add(grabInteractor);
            if (grabInteractor.hasSelection)
            {
                IXRSelectInteractable selectedInteractable = grabInteractor.interactablesSelected[0];
                if ((XRGrabInteractable)selectedInteractable == galaxyGrabInteractable){
                    DespawnGalaxy();
                }
            }
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
        if (PlayerLookingAtSky())
        {
            if (_interactors.Count > 0)
            {
                var grabInteractor = _interactors[0];
                
                if (grabInteractor.logicalSelectState.active)
                {
                    SpawnGalaxy(grabInteractor);
                }
            }
        }
    }
    
    private void SpawnGalaxy(XRDirectInteractor grabInteractor)
    {
        if (!galaxy.activeInHierarchy)
        {
            galaxy.SetActive(true);
            galaxy.transform.position = grabInteractor.transform.position + galaxyOffset;
            galaxy.transform.rotation = Quaternion.Euler(Vector3.zero);
            galaxy.transform.localScale = new Vector3(galaxyBaseScale, galaxyBaseScale, galaxyBaseScale);
            XRInteractionManager interactionManager = grabInteractor.interactionManager;
            
            interactionManager.SelectEnter((IXRSelectInteractor)grabInteractor, galaxyGrabInteractable);
        }
        
    }
    
    private void DespawnGalaxy()
    {
        if (galaxy.activeInHierarchy)
        {
            galaxy.SetActive(false);
        }
    }

    private bool PlayerLookingAtSky()
    {
        return Physics.Raycast(playerCamera.transform.position,
            playerCamera.transform.forward,
            out RaycastHit hit);
    }
}
