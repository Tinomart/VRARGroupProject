using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandNavigationTutorial : TutorialStep
{
    [SerializeField] private TextMeshProUGUI teleportPrompt;
    [SerializeField] private TextMeshProUGUI snapTurnPrompt;
    [SerializeField] private TextMeshProUGUI movementPrompt;
    [SerializeField] private TextMeshProUGUI grabPrompt;
    [SerializeField] private TextMeshProUGUI activatePrompt;
    [SerializeField] private InputActionReference teleportAction;
    [SerializeField] private InputActionReference snapTurnAction;
    [SerializeField] private InputActionReference movementAction;
    [SerializeField] private InputActionReference grabAction;
    [SerializeField] private InputActionReference activateAction;
    
    public override IEnumerator TutorialSequence()
    {
        yield return new WaitForSeconds(6f);
        Player.SendHapticsToHand(true);
        teleportPrompt.gameObject.SetActive(true);
        yield return WaitForInputAction(teleportAction.action);
        teleportPrompt.gameObject.SetActive(false);
        
        yield return new WaitForSeconds(1f);
        Player.SendHapticsToHand(true);
        snapTurnPrompt.gameObject.SetActive(true);
        yield return WaitForInputAction(snapTurnAction.action);
        snapTurnPrompt.gameObject.SetActive(false);
        
        yield return new WaitForSeconds(1f);
        Player.SendHapticsToHand(false);
        movementPrompt.gameObject.SetActive(true);
        yield return WaitForInputAction(movementAction.action);
        movementPrompt.gameObject.SetActive(false);
        
        yield return new WaitForSeconds(1f);
        Player.SendHapticsToHand(true);
        grabPrompt.gameObject.SetActive(true);
        yield return WaitForInputAction(grabAction.action);
        grabPrompt.gameObject.SetActive(false);
        
        yield return new WaitForSeconds(1f);
        Player.SendHapticsToHand(true);
        activatePrompt.gameObject.SetActive(true);
        yield return WaitForInputAction(activateAction.action);
        activatePrompt.gameObject.SetActive(false);
        stepComplete.Invoke();
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
