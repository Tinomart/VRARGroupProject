using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera _camera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (_camera)
        {
            transform.LookAt(_camera.transform.position, Vector3.up);
            transform.Rotate(0, 180, 0);
        }
        
    }
}
