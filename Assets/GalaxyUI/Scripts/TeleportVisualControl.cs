using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;

public class TeleportVisualControl : MonoBehaviour
{
    [SerializeField] private XRRayInteractor rayInteractor;
    [SerializeField] private XRNode controllerNode = XRNode.RightHand;
    [SerializeField] private float inputThreshold = 0.1f;
    
    private XRInteractorLineVisual lineVisual;

    private void Awake()
    {
        lineVisual = rayInteractor.GetComponent<XRInteractorLineVisual>();
    }

    private void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(controllerNode);
        
        if (device.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 thumbstick))
        {
            bool shouldShow = thumbstick.y > inputThreshold;
            
            if (lineVisual)
            {
                lineVisual.enabled = shouldShow;
            }
        }
    }
}
