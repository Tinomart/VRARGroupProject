using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class BlackHole : MonoBehaviour
{
    public GameObject starPrefab;
    [HideInInspector] public static Collider BlackHoleCollider;
    private List<XRDirectInteractor> _interactors;
    private List<XRGrabInteractable> _stars;

    private Star _newStar;

    private bool _awaitingGrab = false;

    private float _radius = 2.5f;

    private XRGrabInteractable _galaxyInteractable;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BlackHoleCollider = GetComponent<Collider>();
        _interactors = new List<XRDirectInteractor>();
        _stars = new List<XRGrabInteractable>();
        _radius = gameObject.GetComponent<SphereCollider>().radius;
        _galaxyInteractable = Galaxy.Instance.GetComponent<XRGrabInteractable>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_galaxyInteractable.interactorsSelecting.Count > 1)
        {
            _interactors.Clear();
            
            if (_newStar)
            {
                _newStar.gameObject.TryGetComponent(out Star starComponent);
                if (starComponent)
                {
                    starComponent.RemoveStar();
                }
                _newStar = null;
            }

        }
        
        if (_interactors.Count > 0)
        {
            foreach (XRDirectInteractor interactor in _interactors)
            {
                if (!_awaitingGrab && !interactor.isSelectActive)
                {
                    StartCoroutine(AwaitGrab(interactor));
                }
            }
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out XRDirectInteractor interactor))
        {
            if (!interactor.activateInput.manualPerformed)
            {
                _interactors.Add(interactor);
            }
                
            
        }
        if (other.gameObject.TryGetComponent(out XRGrabInteractable star))
        {
            _stars.Add(star);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out XRDirectInteractor interactor))
        {
            _interactors.Remove(interactor);
            if (!interactor.interactablesSelected.Contains(_galaxyInteractable) || interactor.activateInput.manualPerformed)
            {
            
                StopCoroutine(AwaitGrab(interactor));
                _awaitingGrab = false;
            }
            else
            {
                interactor.interactionManager.SelectExit((IXRSelectInteractor)interactor, _galaxyInteractable);
            }
            
            
        }
        
        if (other.gameObject.TryGetComponent(out Star star))
        {
            if (star == _newStar)
            {
                _newStar = null;
            }
        } 
        if (other.gameObject.TryGetComponent(out XRGrabInteractable starGrabInteractable))
        {
            _stars.Remove(starGrabInteractable);
        }
    }

    IEnumerator AwaitGrab(XRDirectInteractor interactor)
    {
        _awaitingGrab  = true;
        yield return new WaitUntil(() => interactor.interactablesSelected.Contains(_galaxyInteractable));
        
        SpawnStar(interactor);
        
    }
    
    void SpawnStar(XRDirectInteractor interactor)
    {
        if (!_newStar)
        {
            GameObject obj = Instantiate(starPrefab);
            obj.transform.SetParent(Galaxy.Instance.transform);
            Star starComponent = obj.GetComponent<Star>();
            StartCoroutine(UpdateNextFrame(obj, interactor));
            starComponent.blackHoleStar = true;
            _newStar = starComponent;
            

        }
        
    }
    
    IEnumerator UpdateNextFrame(GameObject obj, XRDirectInteractor interactor)
    {
        yield return null; // Wait one frame
        obj.transform.position = Galaxy.Instance.transform.position;
        obj.GetComponent<Star>().basePosition = Vector3.zero;
        obj.transform.localScale = Galaxy.Instance.GetComponent<Galaxy>().localStarScale;
        
        XRGrabInteractable interactable = obj.GetComponent<XRGrabInteractable>();
        XRInteractionManager interactionManager = interactable.interactionManager;
        interactionManager.SelectEnter((IXRSelectInteractor)interactor, interactable);
        _awaitingGrab = false;
        
    }
    
    
    
}
