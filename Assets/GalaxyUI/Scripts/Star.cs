using System;
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
    [HideInInspector] public TextMeshProUGUI labelText;
    private int _originalLayerMask;
    private Vector3 _basePosition;
    [HideInInspector] public GalaxyArm galaxyArm;

    void Start()
    {
        
        _grabInteractable = GetComponent<XRGrabInteractable>();
        _grabInteractable.activated.AddListener(OnActivated);
        _grabInteractable.hoverEntered.AddListener(OnHoverEnter);
        _grabInteractable.hoverExited.AddListener(OnHoverExit);
        _originalLayerMask = _grabInteractable.interactionLayers;
        _basePosition = transform.localPosition;
        labelText = label.GetComponentInChildren<TextMeshProUGUI>();
        sceneName = "Room" + UnityEngine.Random.Range(1,10);
        labelText.text = sceneName;
        AudioManager.PlayAudioFrom(AudioManager.StarAmbienceSource, gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_grabInteractable.isSelected)
        {
            transform.localPosition = _basePosition;
        }

        //transform.localScale = _baseScale;
        //transform.localPosition = _basePosition;

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
        _grabInteractable.interactionManager.SelectExit(interactor, _grabInteractable);
        
        _basePosition = transform.localPosition;
        if (galaxyArm)
        {
            galaxyArm.StarHoverExit();
            galaxyArm.RemoveStar(this.gameObject);
            galaxyArm = null;
        }
        AudioManager.StarActivateSource.Play();
        
    }

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        label.gameObject.SetActive(true);
        if (galaxyArm != null)
        {
            galaxyArm.StarHoverEnter();
        }
        
    }
    
    private void OnHoverExit(HoverExitEventArgs args)
    {
        label.gameObject.SetActive(false);
        if (galaxyArm != null)
        {
            galaxyArm.StarHoverExit();
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
    }
}
