using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialSteps : MonoBehaviour
{
    private GameObject leftHand;
    private GameObject rightHand;
    public List<TextMeshProUGUI> rightHandPrompts;
    public List<TextMeshProUGUI> leftHandPrompts;
    public Vector3 promptOffset;
    private List<TutorialStep> _tutorialSteps = new List<TutorialStep>();
    private int nextStep = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        leftHand = Player.LeftHand;
        rightHand = Player.RightHand;
        if (leftHand == null)
        {
            Debug.LogError("Left Hand GameObject not found.");
        }
        if (rightHand == null)
        {
            Debug.LogError("Right Hand GameObject not found.");
        }

        foreach (TutorialStep step in GetComponentsInChildren<TutorialStep>())
        {
            
            _tutorialSteps.Add(step);
        }

        _tutorialSteps.Sort((a, b) => a.stepNumber.CompareTo(b.stepNumber));
        BeginNextStep();
    }

    private void BeginNextStep()
    {
        if (nextStep < _tutorialSteps.Count)
        {
            _tutorialSteps[nextStep].BeginStep();
            _tutorialSteps[nextStep].stepComplete.AddListener(BeginNextStep);
            nextStep++;
        }
        else
        {
            FinishTutorial();
        }
    }

    private void FinishTutorial()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (rightHand)
        {
            foreach (TextMeshProUGUI hand in rightHandPrompts)
            {
                Vector3 worldVector = rightHand.transform.TransformDirection(promptOffset);
                hand.transform.position = rightHand.transform.position + worldVector;
            }
        }

        if (leftHand)
        {
            foreach (TextMeshProUGUI hand in leftHandPrompts)
            {
                Vector3 worldOffset = leftHand.transform.TransformDirection(promptOffset);
                hand.transform.position = leftHand.transform.position + worldOffset;
            }
        }
        
    }
}
