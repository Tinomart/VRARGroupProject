using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class BlackHole : MonoBehaviour
{
    public GameObject starPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out XRDirectInteractor xRDirectInteractor))
        {
            GameObject obj = Instantiate(starPrefab);
            obj.transform.SetParent(Galaxy.Instance.transform);
            Star starComponent = obj.GetComponent<Star>();
            Debug.Log("stern erstellt");
            StartCoroutine(UpdateNextFrame(obj));
        }
    }void OnTriggerStay(Collider other)
    {
        
    }
    IEnumerator UpdateNextFrame(GameObject obj)
    {
        yield return null; // Wait one frame
        obj.transform.position = Galaxy.Instance.transform.position;
    }
}
