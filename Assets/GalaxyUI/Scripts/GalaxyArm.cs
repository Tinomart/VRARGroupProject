using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GalaxyArm : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private List<GameObject> stars = new List<GameObject>();
    private List<Transform> starsTransforms = new List<Transform>();
    public float spiralRadius = 12f;
    public float spiralAngle = 2f;

    public float quadraticA = 1f;
    public Vector3 offset = new Vector3(1,0,0);
    public float quadraticB = 0f;
    public float quadraticC = 0f;
    public int starCount = 12;
    
    public GameObject starPrefab;
        
    public void Setup()
    {
        for (int i = 0; i < starCount; i++)
        {
            GameObject obj = Instantiate(starPrefab);
            obj.transform.SetParent(transform);
            stars.Add(obj);
        }
        
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
        
    }
    
    Vector3 CalculateCurvePosition(int starNumber)
    {
        float posX = (float)Math.Cos(starNumber*spiralAngle/stars.Count) * starNumber*spiralRadius/stars.Count;
        float posY = (float)Math.Sin(starNumber*spiralAngle/stars.Count) * starNumber*spiralRadius/stars.Count;
        return new Vector3(posX, 0, posY) + offset;
    }
    
    float Quadratic(float x)
    {
        return quadraticA * (float)Math.Pow(x, 2) + quadraticB * x + quadraticC;
    }
    
   
    // Update is called once per frame
    void Update()
    {
        
    }
}
