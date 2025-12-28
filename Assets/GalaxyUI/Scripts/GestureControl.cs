using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class GestureControl : MonoBehaviour
{
    public Camera playerCamera;
    
    [SerializeField]
    private Vector3 offset = new Vector3(0f, 0.5f, 0f);
    [SerializeField] private float galaxyBaseScale = 0.2f;
    [SerializeField] private float galaxyMinScale = 0.002f;
    [SerializeField] private Vector3 galaxyOffset = new Vector3(0f, 0.5f, 0f);
    [SerializeField] private float galaxySpawnDelay = 0.5f;
    [SerializeField] private Vector3 galaxySpawnStart = new Vector3(0f, 500, 0f);
    [SerializeField] private float galaxySpawnSpeed = 100f;
    [SerializeField] AnimationCurve GalaxyDespawnCurve;
    public GameObject galaxy;
    private XRGrabInteractable galaxyGrabInteractable;
    private XRDirectInteractor leftHandGrabInteractor;
    private XRDirectInteractor rightHandGrabInteractor;
    private bool isLeftHandHovering = false;
    private bool isRightHandHovering = false;
    private bool galaxySpawnStarted = false;
    private bool galaxyDespawnStarted = false;
    private bool galaxyFlyDownStarted = false;
    private bool galaxyFlyUpStarted = false;
    private XRDirectInteractor galaxySpawnInteractor;
    
    
    private List<XRDirectInteractor> _interactors = new List<XRDirectInteractor>();

    void Start()
    {
        leftHandGrabInteractor = Player.LeftHand.GetComponentInChildren<XRDirectInteractor>();
        rightHandGrabInteractor = Player.RightHand.GetComponentInChildren<XRDirectInteractor>();
        if (leftHandGrabInteractor == null)
        {
            Debug.LogError("Left Hand Interactor not found.");
        }
        if (rightHandGrabInteractor == null)
        {
            Debug.LogError("Right Hand Interactor not found.");
        }
        galaxyGrabInteractable = galaxy.GetComponent<XRGrabInteractable>();
        Galaxy.Instance = galaxy;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.TryGetComponent(out XRDirectInteractor grabInteractor))
        {
            _interactors.Add(grabInteractor);
            if (!galaxyDespawnStarted)
            {
                if (grabInteractor.hasSelection)
                {
                    IXRSelectInteractable selectedInteractable = grabInteractor.interactablesSelected[0];
                    if ((XRGrabInteractable)selectedInteractable == galaxyGrabInteractable)
                    {
                        galaxySpawnInteractor = grabInteractor;
                        StartCoroutine(DespawnGalaxyGesture(grabInteractor));
                    }
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
        UpdateHoveringHands();
        transform.position = playerCamera.transform.position + offset;
        if (PlayerLookingAtSky())
        {
            if (_interactors.Count > 0)
            {
                foreach (XRDirectInteractor grabInteractor in _interactors)
                {
                    if (grabInteractor.logicalSelectState.active)
                    {
                        galaxySpawnInteractor = grabInteractor;
                        SpawnGalaxy(grabInteractor);
                    }
                }
                
                    
            }

            if (!galaxy.activeInHierarchy)
            {
                if (isRightHandHovering)
                {
                    Player.SendHapticsToHand(true, 0.1f);
                }
                if (isLeftHandHovering)
                {
                    Player.SendHapticsToHand(false, 0.1f);
                }
            }
                
        }

        if (galaxy.transform.localScale.x < galaxyMinScale)
        {
            if (!galaxyGrabInteractable.isSelected)
            {
                DespawnGalaxyScale();
            }
            
        }

        if (galaxyFlyDownStarted)
        {
            float distance = Vector3.Distance(galaxy.transform.position, galaxySpawnInteractor.transform.position + galaxyOffset);
            float step = galaxySpawnSpeed * Time.deltaTime*distance;
            if (distance < galaxySpawnStart.magnitude/5000)
            {
                galaxyFlyDownStarted = false;
            }
            else
            {
                galaxy.transform.position = Vector3.MoveTowards(galaxy.transform.position, galaxySpawnInteractor.transform.position + galaxyOffset, step);
            }
        } else if (galaxyFlyUpStarted)
        {
            float distance = Vector3.Distance(galaxy.transform.position, galaxySpawnInteractor.transform.position + galaxySpawnStart);
            float step = galaxySpawnSpeed * galaxySpawnStart.magnitude*15 * Time.deltaTime/distance;
            if (distance < step
                 || distance < galaxySpawnStart.magnitude/5)
            {
                galaxyFlyUpStarted = false;
            }
            else
            {
                galaxy.transform.position = Vector3.MoveTowards(galaxy.transform.position, galaxySpawnInteractor.transform.position + galaxySpawnStart, step);
            }
        }
        
        
    }
    
    private void SpawnGalaxy(XRDirectInteractor grabInteractor)
    {
        if (!galaxy.activeInHierarchy && !galaxySpawnStarted)
        {
            StartCoroutine(StartGalaxySpawn(grabInteractor));
        }
        
    }
    
    IEnumerator StartGalaxySpawn(XRDirectInteractor grabInteractor)
    {
        galaxySpawnStarted = true;
        AudioManager.GalaxySpawnSource.Play();
        yield return new WaitForSeconds(galaxySpawnDelay);
        
        galaxy.SetActive(true);
        galaxy.transform.position = grabInteractor.transform.position + galaxySpawnStart;
        galaxy.transform.rotation = Quaternion.Euler(Vector3.zero);
        galaxy.transform.localScale = new Vector3(galaxyBaseScale, galaxyBaseScale, galaxyBaseScale);
        galaxyFlyDownStarted = true;
        
        yield return new WaitUntil(() => !galaxyFlyDownStarted);
        XRInteractionManager interactionManager = grabInteractor.interactionManager;
        interactionManager.SelectEnter((IXRSelectInteractor)grabInteractor, galaxyGrabInteractable);
        galaxySpawnStarted = false;
    }

    private void DespawnGalaxyScale()
    {
        if (galaxy.activeInHierarchy)
        {
            AudioManager.GalaxyDespawnSource.Play();
            galaxy.SetActive(false);
        }
    }
    
    IEnumerator DespawnGalaxyGesture(XRDirectInteractor grabInteractor)
    {
        if (galaxy.activeInHierarchy)
        {
            XRInteractionManager interactionManager = grabInteractor.interactionManager;
            interactionManager.SelectExit((IXRSelectInteractor)grabInteractor, galaxyGrabInteractable);
            AudioManager.GalaxyDespawnSource.Play();
            galaxyDespawnStarted = true;
            
            galaxyFlyUpStarted = true;
        
            yield return new WaitUntil(() => !galaxyFlyUpStarted);
            galaxy.SetActive(false);
            galaxyDespawnStarted = false;
            
        }
        yield return null;
    }

    private bool PlayerLookingAtSky()
    {
        Physics.Raycast(playerCamera.transform.position,
            playerCamera.transform.forward,
            out RaycastHit hit);
        if (hit.collider)
        {
            hit.collider.gameObject.TryGetComponent<GestureControl>(out var gestureControl);
            if (gestureControl)
            {
                return true;
            }
        }
        
        return false;
    }

    private void UpdateHoveringHands()
    {
        isLeftHandHovering = false;
        isRightHandHovering = false;
        if (_interactors.Count > 0)
        {
            foreach (XRDirectInteractor grabInteractor in _interactors)
            {
                if (grabInteractor == leftHandGrabInteractor)
                {
                    isLeftHandHovering = true;
                }
                if (grabInteractor == rightHandGrabInteractor)
                {
                    isRightHandHovering = true;
                }
            }
        }
    }
}
