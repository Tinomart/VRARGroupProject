using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Random = System.Random;

public class GalaxyArm : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [HideInInspector] public List<GameObject> stars = new List<GameObject>();
    [HideInInspector] public List<GameObject> dust = new List<GameObject>();
    
    
    private List<Transform> starsTransforms = new List<Transform>();
    [HideInInspector] public List<Star> collidingStars = new List<Star>();
    public float spiralRadius = 12f;
    public float positionRange = 1f;
    public float spiralAngle = 2f;
    public float galaxyArmLeaveDistance = 0.1f;
    public Vector3 offset = new Vector3(1,0,0);
    public float dustArmOffset = 0.5f;
    public float colliderRadius = 1f;
    
    public int starCount = 12;
    public int dustEmitterCount = 50;
    private int starHoverCount = 0;
    
    public GameObject starPrefab;
    public GameObject dustGeneratorPrefab;
    public GameObject label;
    [HideInInspector]
    public TextMeshProUGUI labelText;
    public String armName = "None";
    
    private List<XRDirectInteractor> _interactors = new List<XRDirectInteractor>();
    
    void Update()
    {
        CheckCollidingStarsForAddition();
    }
        
    public void Setup()
    {
        for (int i = 0; i < starCount; i++)
        {
            GameObject obj = Instantiate(starPrefab);
            stars.Add(obj);
            obj.transform.SetParent(Galaxy.Instance.transform);
            Star starComponent = obj.GetComponent<Star>();
            starComponent.InitializeGalaxyArm(this);
        }

        label.transform.SetParent(Galaxy.Instance.transform);
        labelText = label.GetComponentInChildren<TextMeshProUGUI>();
        labelText.text = armName;
        
        CreateDust();
        StartCoroutine(UpdateColliderNextFrame());
        

    }
    
    public Vector3 CalculateCurvePosition(int starNumber)
    {
        float posX = ((float)Math.Cos(starNumber*spiralAngle/stars.Count) * starNumber*spiralRadius/stars.Count) + UnityEngine.Random.Range(-positionRange, positionRange);
        float posY =  UnityEngine.Random.Range(-positionRange, positionRange);
        float posZ = ((float)Math.Sin(starNumber*spiralAngle/stars.Count) * starNumber*spiralRadius/stars.Count) + UnityEngine.Random.Range(-positionRange, positionRange);
        
        return new Vector3(posX, posY, posZ) + offset;
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

    public void RemoveStar(GameObject star, bool isTemporary = false)
    {
        if (star.TryGetComponent<Star>(out Star starComponent))
        {
            if (!isTemporary)
            {
                starComponent.currentGalaxyArmIndex = stars.IndexOf(star);
            }
            stars.Remove(star);
            RearrangeStars();
        }
        else
        {
            Debug.LogError("The object that was tried to be removed is not a star.");
        }
    }
    
    public void AddStar(GameObject star, int index = 0)
    {
        if (star.GetComponent<Star>())
        {
            stars.Add(star);
            //stars.Insert(index, star);
            RearrangeStars();
        }
        else
        {
            Debug.LogError("The object that was tried to be added is not a star.");
        }
        
    }
    
    IEnumerator UpdateColliderNextFrame()
    {
        Galaxy.Instance.GetComponent<Galaxy>().localStarScale = stars[0].transform.localScale;
        yield return null; // Wait one frame
        RearrangeStars();
        PositionDust();
        yield return null;
        BoxCollider collider = gameObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        FitColliderToTransforms(collider, starsTransforms, colliderRadius);
    }

    private void RearrangeStars()
    {
        starsTransforms = new List<Transform>();
        foreach (GameObject star in stars)
        {
            starsTransforms.Add(star.transform);
        }
        var starcount = 0;

        foreach (GameObject star in stars)
        {
            Star starComponent = star.GetComponent<Star>();
            starComponent.basePosition = Galaxy.Instance.transform.InverseTransformPoint(transform.TransformPoint(CalculateCurvePosition(starcount)));
            starcount++;

        }
        label.transform.localPosition = Galaxy.Instance.transform.InverseTransformPoint(transform.TransformPoint(CalculateCurvePosition(starcount)));
        starsTransforms.Add(label.transform);
        
    }
    public void FitColliderToTransforms(BoxCollider box, List<Transform> transforms, float radius = 0f)
    {
        if (transforms == null || transforms.Count == 0)
        {
            Debug.LogWarning("Transform list is empty!");
            return;
        }
    
        // Convert all world positions to local space of the collider's GameObject
        Transform colliderTransform = box.transform;
        List<Vector3> localPositions = new List<Vector3>();
    
        foreach (Transform t in transforms)
        {
            localPositions.Add(colliderTransform.InverseTransformPoint(t.position));
        }
    
        // Find min and max in each axis
        Vector3 min = localPositions[0];
        Vector3 max = localPositions[0];
    
        foreach (Vector3 pos in localPositions)
        {
            min.x = Mathf.Min(min.x, pos.x);
            min.y = Mathf.Min(min.y, pos.y);
            min.z = Mathf.Min(min.z, pos.z);
        
            max.x = Mathf.Max(max.x, pos.x);
            max.y = Mathf.Max(max.y, pos.y);
            max.z = Mathf.Max(max.z, pos.z);
        }
    
        // Expand bounds by radius in all directions
        min -= new Vector3(radius, radius, radius);
        max += new Vector3(radius, radius, radius);
    
        // Calculate center and size
        Vector3 center = (min + max) / 2f;
        Vector3 size = max - min;
    
        box.center = center;
        box.size = size;
    }

    void CheckCollidingStarsForAddition()
    {
        foreach (Star collidingStar in collidingStars)
        {
            if (collidingStar._grabInteractable.isSelected && !stars.Contains(collidingStar.gameObject))
            {
                
                float minimumDistance = 100000f;
                //int miniumDistanceIndex = 0;
                //int whereIndex = -1;
                for (int i = 0; i < starsTransforms.Count; i++)
                {
                    Transform heldStar = starsTransforms[i];
                    
                    float starDistance = Vector3.Distance(collidingStar.transform.position, heldStar.position);
                    if (starDistance < minimumDistance)
                    {
                        minimumDistance = starDistance;
                       // miniumDistanceIndex = i;
                    }
                }
               
                float temporaryGalaxyArmDistance = 0f;
                if (collidingStar.currentGalaxyArm)
                {
                    temporaryGalaxyArmDistance = collidingStar.currentGalaxyArm.CalculateMinimumDistance(collidingStar.transform);
                }
                else
                {
                    temporaryGalaxyArmDistance = 100000f;
                }
                //Debug.Log("potentialMinimumDistance=" + minimumDistance);
                //Debug.Log("currentMinimumDistance=" + temporaryGalaxyArmDistance);
                
                //Debug.Log("Condition1=" + minimumDistance+ "<" + CalculateRelativeGalaxyLeaveDistance());
                //Debug.Log(minimumDistance < CalculateRelativeGalaxyLeaveDistance());
                //Debug.Log("Condition2=" + minimumDistance+ "<" + temporaryGalaxyArmDistance);
                //Debug.Log(minimumDistance < temporaryGalaxyArmDistance);
                if (minimumDistance < CalculateRelativeGalaxyLeaveDistance() && minimumDistance < temporaryGalaxyArmDistance)
                {
                    /*
                    if (miniumDistanceIndex + 1 < starsTransforms.Count)
                    {
                        if(miniumDistanceIndex - 1 > 0)
                        {
                            float behindDistance = Vector3.Distance(starsTransforms[miniumDistanceIndex - 1].position, collidingStar.transform.position);
                            float infrontDistance = Vector3.Distance(starsTransforms[miniumDistanceIndex + 1].position, collidingStar.transform.position);
                            if (infrontDistance < behindDistance)
                            {
                                whereIndex = 1;
                            }
                        }
                        else
                        {
                            whereIndex = 0;
                        }
                        
                    }
                    else
                    {
                        whereIndex = 0;
                    }
                    */
                    
                    //collidingStar.currentGalaxyArmIndex = miniumDistanceIndex + whereIndex;
                    collidingStar.ChangeCurrentGalaxyArm(this);
                    
                }
                
            }
        }
    }

    public float CalculateRelativeGalaxyLeaveDistance()
    {
        return (galaxyArmLeaveDistance * transform.lossyScale.magnitude/ Mathf.Sqrt(3))*(float)stars.Count /
            (float)starCount;
    }
    
    public float CalculateMinimumDistance(Transform objectTransform)
    {
        float minimumDistance = 100000f;
        for (int i = 0; i < starsTransforms.Count; i++)
        {
            Transform heldStar = starsTransforms[i];
            float starDistance = 100000f;
            if (heldStar == objectTransform)
            {
                starDistance = Vector3.Distance(objectTransform.position, transform.TransformPoint(CalculateCurvePosition(i)));
            }
            else
            {
                starDistance = Vector3.Distance(objectTransform.position, heldStar.transform.position);
            }
            
            
            if (starDistance < minimumDistance)
            {
                minimumDistance = starDistance;
            }
        }

        
        return minimumDistance;
    }
    
    public Vector3 CalculateDustPosition(float armPosition)
    {
        float posX = ((float)Math.Cos((armPosition-dustArmOffset)*spiralAngle/stars.Count) * (armPosition-dustArmOffset)*spiralRadius/stars.Count) + (UnityEngine.Random.Range(-positionRange, positionRange)/10);
        float posY =  UnityEngine.Random.Range(-positionRange, positionRange)/10;
        float posZ = ((float)Math.Sin((armPosition-dustArmOffset)*spiralAngle/stars.Count) * (armPosition-dustArmOffset)*spiralRadius/stars.Count) + (UnityEngine.Random.Range(-positionRange, positionRange)/10);
        
        return new Vector3(posX, posY, posZ) + offset;
    }

    void CreateDust()
    {
        for (int i = 0; i < dustEmitterCount; i++)
        {
            GameObject obj = Instantiate(dustGeneratorPrefab);
            obj.transform.SetParent(Galaxy.Instance.transform);
            dust.Add(obj);
        }
    }

    void PositionDust()
    {
        int i = 0;
        foreach (var emitter in dust)
        {
            //Debug.Log((float)stars.Count/(float)dustEmitterCount*(float)i);
            emitter.transform.localPosition = Galaxy.Instance.transform.InverseTransformPoint(transform.TransformPoint(CalculateDustPosition(((float)stars.Count/(float)dustEmitterCount)*(float)i)));
            i++;
        }
    }
}
