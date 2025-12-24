using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class StarInteractionTutorial : TutorialStep
{
    [SerializeField] private TextMeshProUGUI hoverPrompt;
    [SerializeField] private TextMeshProUGUI movePrompt;
    [SerializeField] private TextMeshProUGUI releasePrompt;
    [SerializeField] private TextMeshProUGUI activatePrompt;
    [SerializeField] private TextMeshProUGUI sceneSwitchPrompt;
    [SerializeField] private float minimumStarMoveDistance = 0.3f;
    private XRDirectInteractor leftHandInteractor;
    private XRDirectInteractor rightHandInteractor;
    private GameObject grabbedStar;
    private XRGrabInteractable grabbedStarInteractable;
    private Vector3 grabPosition;
    private bool grabbedStarIsActivated = false;
    public override IEnumerator TutorialSequence()
    {
        leftHandInteractor = Player.LeftHand.GetComponentInChildren<XRDirectInteractor>();
        rightHandInteractor = Player.RightHand.GetComponentInChildren<XRDirectInteractor>();
        
        hoverPrompt.gameObject.SetActive(true);
        yield return new WaitUntil(PlayerHoveringStar);
        hoverPrompt.gameObject.SetActive(false);
        
        movePrompt.gameObject.SetActive(true);
        yield return new WaitUntil(PlayerMovedStar);
        movePrompt.gameObject.SetActive(false);
        
        movePrompt.gameObject.SetActive(true);
        yield return new WaitUntil(PlayerMovedStar);
        movePrompt.gameObject.SetActive(false);
        
        releasePrompt.gameObject.SetActive(true);
        yield return new WaitUntil(PlayerReleasedStar);
        releasePrompt.gameObject.SetActive(false);
        
        releasePrompt.gameObject.SetActive(true);
        yield return new WaitUntil(PlayerReleasedStar);
        releasePrompt.gameObject.SetActive(false);
        
        activatePrompt.gameObject.SetActive(true);
        yield return new WaitUntil(PlayerActivatedStar);
        activatePrompt.gameObject.SetActive(false);
        
        sceneSwitchPrompt.gameObject.SetActive(true);
    }

    bool PlayerHoveringStar()
    {
        if (leftHandInteractor.hasHover)
        {
            foreach (IXRHoverInteractable hoverInteractable in leftHandInteractor.interactablesHovered)
            {
                if (hoverInteractable is MonoBehaviour monoBehaviour)
                {
                    if (monoBehaviour.gameObject.GetComponent<Star>())
                    {
                        return true;
                    }
                }
            }
            
        }
        if (rightHandInteractor.hasHover)
        {
            foreach (IXRHoverInteractable hoverInteractable in rightHandInteractor.interactablesHovered)
            {
                if (hoverInteractable is MonoBehaviour monoBehaviour)
                {
                    if (monoBehaviour.gameObject.GetComponent<Star>())
                    {
                        return true;
                    }
                }
            }
            
        }
        
        return false;
    }

    bool PlayerMovedStar()
    {
        if (leftHandInteractor.hasSelection)
        {
            foreach (IXRSelectInteractable selectInteractable in leftHandInteractor.interactablesSelected)
            {
                if (selectInteractable is MonoBehaviour monoBehaviour)
                {
                    if (monoBehaviour.gameObject.GetComponent<Star>())
                    {
                        if (monoBehaviour.gameObject != grabbedStar)
                        {
                            grabbedStar = monoBehaviour.gameObject;
                            grabPosition = monoBehaviour.transform.position;
                        }
                    }
                }
            }
            
        }
        else if (rightHandInteractor.hasSelection)
        {
            foreach (IXRSelectInteractable selectInteractable in rightHandInteractor.interactablesSelected)
            {
                if (selectInteractable is MonoBehaviour monoBehaviour)
                {
                    if (monoBehaviour.gameObject.GetComponent<Star>())
                    {
                        if (monoBehaviour.gameObject != grabbedStar)
                        {
                            grabbedStar = monoBehaviour.gameObject;
                            grabPosition = monoBehaviour.transform.position;
                        }
                    }
                }
            }
            
        }

        if (grabbedStar)
        {
            if (Vector3.Distance(grabbedStar.transform.position, grabPosition) > minimumStarMoveDistance)
            {
                return true;
            }
        }
            
        
        return false;
    }

    bool PlayerReleasedStar()
    {
        if (!leftHandInteractor.hasSelection && !rightHandInteractor.hasSelection)
        {
            return true;
        }
        return false;
    }

    bool PlayerActivatedStar()
    {
        if (leftHandInteractor.hasSelection)
        {
            foreach (IXRSelectInteractable selectInteractable in leftHandInteractor.interactablesSelected)
            {
                if (selectInteractable is MonoBehaviour monoBehaviour)
                {
                    if (monoBehaviour.gameObject.GetComponent<Star>())
                    {
                        if (monoBehaviour.gameObject.TryGetComponent<XRGrabInteractable>(out XRGrabInteractable starInteractable))
                        {
                            if (starInteractable != grabbedStarInteractable)
                            {
                                grabbedStarInteractable = starInteractable;
                                grabbedStarInteractable.activated.AddListener(OnGrabbedStarActivated);
                            }
                        
                        } 
                    }
                    
                }
            }
            
        }
        else if (rightHandInteractor.hasSelection)
        {
            foreach (IXRSelectInteractable selectInteractable in rightHandInteractor.interactablesSelected)
            {
                if (selectInteractable is MonoBehaviour monoBehaviour)
                {
                    if (monoBehaviour.gameObject.GetComponent<Star>())
                    {
                        if (monoBehaviour.gameObject.TryGetComponent<XRGrabInteractable>(out XRGrabInteractable starInteractable))
                        {
                            if (starInteractable != grabbedStarInteractable)
                            {
                                grabbedStarInteractable = starInteractable;
                                grabbedStarInteractable.activated.AddListener(OnGrabbedStarActivated);
                            }
                        
                        } 
                    }
                }
            }
            
        }

        return grabbedStarIsActivated;
    }

    void OnGrabbedStarActivated(ActivateEventArgs args)
    {
        grabbedStarIsActivated = true;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
