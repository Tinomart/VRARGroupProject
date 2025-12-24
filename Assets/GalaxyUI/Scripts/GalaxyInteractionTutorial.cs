using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GalaxyInteractionTutorial : TutorialStep
{
    [SerializeField] private TextMeshProUGUI spawnPrompt;
    [SerializeField] private TextMeshProUGUI movePrompt;
    [SerializeField] private TextMeshProUGUI gestureDespawnPrompt;
    [SerializeField] private TextMeshProUGUI respawnPrompt;
    [SerializeField] private TextMeshProUGUI rescalePrompt;
    [SerializeField] private TextMeshProUGUI scaleDespawnPrompt;
    [SerializeField] private float skyHeight = 0.3f;
    [SerializeField] private float scaleDifference = 0.3f;
    private float _galaxySpawnHeight;
    private float _galaxySpawnScale;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override IEnumerator TutorialSequence()
    {
        spawnPrompt.gameObject.SetActive(true);
        yield return new WaitUntil(() => Galaxy.Instance.activeInHierarchy);
        spawnPrompt.gameObject.SetActive(false);
        
        _galaxySpawnHeight = Galaxy.Instance.transform.position.y;
        movePrompt.gameObject.SetActive(true);
        yield return new WaitUntil(() => GalaxyMovedOutOfSky());
        movePrompt.gameObject.SetActive(false);
        
        gestureDespawnPrompt.gameObject.SetActive(true);
        yield return new WaitUntil(() => !Galaxy.Instance.activeInHierarchy);
        gestureDespawnPrompt.gameObject.SetActive(false);
        
        respawnPrompt.gameObject.SetActive(true);
        yield return new WaitUntil(() => Galaxy.Instance.activeInHierarchy);
        respawnPrompt.gameObject.SetActive(false);
        
        _galaxySpawnScale = Galaxy.Instance.transform.localScale.y;
        rescalePrompt.gameObject.SetActive(true);
        yield return new WaitUntil(() => GalaxyRescaled());
        rescalePrompt.gameObject.SetActive(false);
        
        scaleDespawnPrompt.gameObject.SetActive(true);
        yield return new WaitUntil(() => !Galaxy.Instance.activeInHierarchy);
        scaleDespawnPrompt.gameObject.SetActive(false);
        
        respawnPrompt.gameObject.SetActive(true);
        yield return new WaitUntil(() => Galaxy.Instance.activeInHierarchy);
        respawnPrompt.gameObject.SetActive(false);
        stepComplete.Invoke();
    }

    bool GalaxyMovedOutOfSky()
    {
        if (_galaxySpawnHeight - Galaxy.Instance.transform.position.y > skyHeight)
        {
            return true;
        }
        return false;
    }

    bool GalaxyRescaled()
    {
        if (scaleDifference < Math.Abs(1 - _galaxySpawnScale/Galaxy.Instance.transform.localScale.y))
        {
            return true;
        }
        return false;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
