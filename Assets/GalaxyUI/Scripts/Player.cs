using UnityEngine;

public class Player : MonoBehaviour
{
    public static GameObject LeftHand;
    public static GameObject RightHand;
    public GameObject leftHand;
    public GameObject rightHand;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (leftHand == null)
        {
            Debug.LogError("Left Hand GameObject not assigned to Player");
        }
        if (rightHand == null)
        {
            Debug.LogError("Right Hand GameObject not assigned to Player");
        }
        LeftHand = leftHand;
        RightHand = rightHand;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
