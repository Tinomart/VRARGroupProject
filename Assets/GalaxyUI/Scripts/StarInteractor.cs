using System;
using System.Collections.Generic;
using Runemark.SCEMA;
using UnityEngine;

public class StarInteractor : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /*
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.TryGetComponent<Star>(out Star star))
        {
            if (star._grabInteractable.isSelected)
            {
                Debug.Log("Selected");
                Location location = _locations[star.locationIndex];
                location.Enter();
            }
            
        }
    }
    */
    public void SceneTransition(String sceneName)
    {
        Debug.Log(sceneName);
        Debug.Log(SCEMA.Instance.FindLocation(sceneName));
        Location location = SCEMA.Instance.FindLocation(sceneName);
        location.Enter();
            
    }
    
}
