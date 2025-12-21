using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
//using Runemark.SCEMA;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StarInteractor : MonoBehaviour
{
    [SerializeField]
    private String startingSceneName = "Room1";
    private String galaxyUISceneName = "GalaxyUI";
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UnloadAllAndLoad(startingSceneName);
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
        UnloadAllAndLoad(sceneName);
    }
    
    public void UnloadAllAndLoad(string sceneName)
    {
        // Unload all except persistent
        for (int i = SceneManager.sceneCount - 1; i >= 0; i--)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name != galaxyUISceneName)
            {
                SceneManager.UnloadSceneAsync(scene);
            }
        }
    
        // Load new scene
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        Resources.UnloadUnusedAssets();

        DynamicGI.UpdateEnvironment();
    }
    
    
}
