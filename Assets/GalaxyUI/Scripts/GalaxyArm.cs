using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class GalaxyArm : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private List<GameObject> stars = new List<GameObject>();
    
    
    private List<Transform> starsTransforms = new List<Transform>();
    public float spiralRadius = 12f;
    public float spiralAngle = 2f;
    
    public Vector3 offset = new Vector3(1,0,0);
    
    public int starCount = 12;
    private int starHoverCount = 0;
    
    public GameObject starPrefab;
    public GameObject label;
    [HideInInspector]
    public TextMeshProUGUI labelText;
    public String armName = "None";
    
    private List<XRDirectInteractor> _interactors = new List<XRDirectInteractor>();
    
        
    public void Setup()
    {
        for (int i = 0; i < starCount; i++)
        {
            GameObject obj = Instantiate(starPrefab);
            obj.transform.SetParent(transform);
            stars.Add(obj);
            obj.GetComponent<Star>().galaxyArm = this;
        }
        
        labelText = label.GetComponentInChildren<TextMeshProUGUI>();
        labelText.text = armName;
        
        RearrangeStars();
        
        
    }
    
    Vector3 CalculateCurvePosition(int starNumber)
    {
        float posX = (float)Math.Cos(starNumber*spiralAngle/stars.Count) * starNumber*spiralRadius/stars.Count;
        float posY = (float)Math.Sin(starNumber*spiralAngle/stars.Count) * starNumber*spiralRadius/stars.Count;
        return new Vector3(posX, 0, posY) + offset;
    }

    public void StarHoverEnter()
    {
        starHoverCount++;
        label.SetActive(true);
    }
    public void StarHoverExit()
    {
        starHoverCount--;
        if (starHoverCount == 0)
        {
            label.SetActive(false);
        }
        
    }

    public void RemoveStar(GameObject star)
    {
        if (star.GetComponent<Star>())
        {
            stars.Remove(star);
            RearrangeStars();
        }
        else
        {
            Debug.LogError("The object that was tried to be removed is not a star.");
        }
    }
    
    public void AddStar(GameObject star)
    {
        if (star.GetComponent<Star>())
        {
            stars.Add(star);
            RearrangeStars();
            star.GetComponent<Star>().galaxyArm = this;
        }
        else
        {
            Debug.LogError("The object that was tried to be added is not a star.");
        }
        
    }

    private void RearrangeStars()
    {
        starsTransforms = new List<Transform>();
        foreach (GameObject star in stars)
        {
            starsTransforms.Add(star.transform);
        }
        var starcount = 0;

        foreach (Transform star in starsTransforms)
        {
            star.localPosition = CalculateCurvePosition(starcount);
            starcount++;

        }
        
        label.transform.localPosition = CalculateCurvePosition(starcount);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
