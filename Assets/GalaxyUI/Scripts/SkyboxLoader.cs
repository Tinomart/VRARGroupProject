using UnityEngine;

public class SkyboxLoader : MonoBehaviour
{
    [SerializeField] private Material skyboxMaterial;
    
    void OnEnable()
    {
        if (skyboxMaterial != null)
        {
            RenderSettings.skybox = skyboxMaterial;
            DynamicGI.UpdateEnvironment();
        }
    }
}
