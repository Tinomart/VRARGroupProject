using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


public class Star : MonoBehaviour
{
    public String sceneName = "Room1";
    public XRGrabInteractable _grabInteractable;
    public GameObject label;
    [SerializeField] private float returnSpeed = 0.1f;
    [HideInInspector] public TextMeshProUGUI labelText;
    private int _originalLayerMask;
    [HideInInspector] public Vector3 basePosition;
    private Vector3 _baseGlobalPosition;
    private GalaxyArm galaxyArm;
    [HideInInspector] public GalaxyArm currentGalaxyArm;
    [HideInInspector] public int currentGalaxyArmIndex = 0;
    [HideInInspector] public bool processingcurrentGalaxyArm = false;
    private GameObject mesh;

    void Start()
    {
        _grabInteractable = GetComponent<XRGrabInteractable>();
        _grabInteractable.activated.AddListener(OnActivated);
        _grabInteractable.selectExited.AddListener(OnSelectExit);
        _grabInteractable.hoverEntered.AddListener(OnHoverEnter);
        _grabInteractable.hoverExited.AddListener(OnHoverExit);
        _originalLayerMask = _grabInteractable.interactionLayers;
        labelText = label.GetComponentInChildren<TextMeshProUGUI>();
        int roomNumber = UnityEngine.Random.Range(1, 10);
        sceneName = "Room" + roomNumber.ToString();
        labelText.text = sceneName;
        AudioManager.PlayAudioFrom(AudioManager.StarAmbienceSource, gameObject);
        //mesh = GameObject.Find("Sphere");
        
        //mesh = transform.Find("Sphere");
        
        MeshRenderer mr = GetComponentInChildren<MeshRenderer>();
        GameObject mesh = mr.gameObject;
        
        //mesh.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.red);
        mesh.GetComponent<Renderer>().material.SetFloat("_roomNumber", roomNumber);
    }

    public void InitializeGalaxyArm(GalaxyArm newGalaxyArm)
    {
        galaxyArm = newGalaxyArm;
        currentGalaxyArm = galaxyArm;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!_grabInteractable.isSelected)
        {
            _baseGlobalPosition = transform.position;
            
            transform.localPosition = Vector3.Lerp(transform.localPosition, basePosition, returnSpeed * Time.deltaTime);
        }
        else
        {
            if (galaxyArm)
            {
                if (galaxyArm.CalculateMinimumDistance(transform) > galaxyArm.CalculateRelativeGalaxyLeaveDistance())
                {
                    if (currentGalaxyArm == galaxyArm)
                    {
                        RemoveCurrentGalaxyArm();
                    }
                } else
                {
                    ChangeCurrentGalaxyArm(galaxyArm);
                }
            }
        }
    }
    
    public void SetPassPriority(bool pass)
    {
        if (pass)
        {
            // Temporarily disable interaction by changing layer mask
            _grabInteractable.interactionLayers = 0;
        }
        else
        {
            // Restore original layer
            _grabInteractable.interactionLayers = _originalLayerMask;
        }
    }

    private void OnActivated(ActivateEventArgs args)
    {
        var interactor = _grabInteractable.interactorsSelecting[0] as IXRSelectInteractor;
            
        // Tell the interaction manager to deselect
        
        
        if(currentGalaxyArm == null)
        {
            if (galaxyArm)
            {
                galaxyArm.StarHoverExit();
                galaxyArm = null;
            }
            basePosition = Galaxy.Instance.transform.InverseTransformPoint(transform.position);
        }
        else if (currentGalaxyArm != galaxyArm)
        {
            galaxyArm = currentGalaxyArm;
            currentGalaxyArm.StarHoverExit();
        }
        
        _grabInteractable.interactionManager.SelectExit(interactor, _grabInteractable);
        AudioManager.StarActivateSource.Play();
        
    }

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        label.gameObject.SetActive(true);
        if (currentGalaxyArm != null)
        {
            currentGalaxyArm.StarHoverEnter();
        }
        
    }
    
    private void OnHoverExit(HoverExitEventArgs args)
    {
        label.gameObject.SetActive(false);
        if (currentGalaxyArm != null)
        {
            currentGalaxyArm.StarHoverExit();
        }
        if (galaxyArm != null)
        {
            if (galaxyArm != currentGalaxyArm)
            {
                galaxyArm.StarHoverExit();
            }
        }

        
    }

    private void OnSelectExit(SelectExitEventArgs args)
    {
        if (galaxyArm)
        {
            ChangeCurrentGalaxyArm(galaxyArm);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out StarInteractor starInteractor))
        {
            if (_grabInteractable.isSelected)
            {
                starInteractor.SceneTransition(sceneName);
            }
            
        }
        if (other.gameObject.TryGetComponent(out GalaxyArm galaxyArm))
        {
            galaxyArm.collidingStars.Add(this);
        }
        
    }

    public void RemoveCurrentGalaxyArm()
    {
        if (currentGalaxyArm && !processingcurrentGalaxyArm)
        {
            currentGalaxyArm.RemoveStar(gameObject);
            currentGalaxyArm.StarHoverExit();
        }

        currentGalaxyArm = null;

    }
    public void ChangeCurrentGalaxyArm(GalaxyArm newGalaxyArm, bool indexSetManually = false)
    {
        
        
        if (!processingcurrentGalaxyArm)
        {
            if (!(currentGalaxyArm == newGalaxyArm))
            {
                if (currentGalaxyArm)
                {
                    currentGalaxyArm.RemoveStar(gameObject, indexSetManually); 
                    currentGalaxyArm.StarHoverExit();
                }
                currentGalaxyArm = newGalaxyArm;
                currentGalaxyArm.AddStar(gameObject, currentGalaxyArmIndex);
            }
            if (!galaxyArm)
            {
                galaxyArm = newGalaxyArm;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out GalaxyArm galaxyArm))
        {
            galaxyArm.collidingStars.Remove(this);
        }
    }
}
