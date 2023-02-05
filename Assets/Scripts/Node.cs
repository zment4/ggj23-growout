using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

public class Node : MonoBehaviour
{
    [HideInInspector]
    public int Size = 0;
    public int MaxSize;
    private float SizeScale;

    [HideInInspector]
    private Vector3 targetPosition;
    [HideInInspector]
    public List<MonoBehaviour> Children = new List<MonoBehaviour>();
    private List<Node> _childNodes;
    private List<Node> childNodes {
        get {
            if (_childNodes == null)
            {
                _childNodes = Children.Select(x => x.GetComponent<Node>()).ToList();
            }
            return _childNodes;
        }
    }
    public MonoBehaviour Parent;
    [HideInInspector]
    public Hub MainHub;

    private Transform stalkTransform;

    public bool Active = true;

    void Start()
    {
        targetPosition = Parent.transform.position;
        Vector3 deltaPosition = targetPosition - transform.position;

        stalkTransform = transform.Find("Stalk");

        SizeScale = stalkTransform.localScale.y / MaxSize;
        stalkTransform.localScale = new Vector3(deltaPosition.magnitude, stalkTransform.localScale.y, stalkTransform.localScale.z);
        stalkTransform.localRotation = Quaternion.LookRotation(Vector3.forward, deltaPosition);
        stalkTransform.localRotation = Quaternion.Euler(0, 0, 90 + stalkTransform.localRotation.eulerAngles.z);

        IncreaseSize();

        MainHub.ResourceManager.SetStateInsideCircle(transform.position, MainHub.StalkLength * 0.25f, Resource.ResourceState.Unclaimed, Resource.ResourceState.Claimed);
    }

    public bool Grow() {
        if (!Active) return false;

        bool tryToBranch = UnityEngine.Random.Range(0f, 1f) < MainHub.ChanceToBranch;
        bool tryToGrow = UnityEngine.Random.Range(0f, 1f) < MainHub.ChanceToGrow;
        bool didGrow = false;

        // if (tryToBranch && Children.Count > 0) {
        //     try { 
        //         Vector3 deltaPosition = transform.position - targetPosition;
        //         deltaPosition = deltaPosition.normalized;
        //         deltaPosition = Quaternion.AngleAxis(-45, Vector3.forward) * deltaPosition;
        //         Vector3 newTargetPosition = transform.position + deltaPosition * MainHub.StalkLength;        
                
        //         int resourcesAmount = 0;
        //         Vector3 resourceMidPoint = MainHub.GetResourceMidpoint(newTargetPosition, Resource.ResourceState.Claimed, MainHub.StalkLength, out resourcesAmount);
        //         if (resourcesAmount >= MainHub.MinimumResourceAmountToGrow) {
        //             Vector3 deltaVector = resourceMidPoint - transform.position;
        //             deltaVector = deltaVector.normalized * MainHub.StalkLength;

        //             Node newNode = MainHub.CreateNode(transform.position + deltaVector);
        //             newNode.Parent = this;
        //             Children.Add(newNode);
        //             didGrow = true;                        
        //         }
        //     } catch (InvalidOperationException) { }
        //     if (!didGrow)
        //     {
        //         try { 
        //             Vector3 deltaPosition = transform.position - targetPosition;
        //             deltaPosition = deltaPosition.normalized;
        //             deltaPosition = Quaternion.AngleAxis(45, Vector3.forward) * deltaPosition;
        //             Vector3 newTargetPosition = transform.position + deltaPosition * MainHub.StalkLength;        
                    
        //             int resourcesAmount = 0;
        //             Vector3 resourceMidPoint = MainHub.GetResourceMidpoint(newTargetPosition, Resource.ResourceState.Claimed, MainHub.StalkLength, out resourcesAmount);
        //             if (resourcesAmount >= MainHub.MinimumResourceAmountToGrow) {
        //                 Vector3 deltaVector = resourceMidPoint - transform.position;
        //                 deltaVector = deltaVector.normalized * MainHub.StalkLength;

        //                 Node newNode = MainHub.CreateNode(transform.position + deltaVector);
        //                 newNode.Parent = this;
        //                 Children.Add(newNode);
        //                 didGrow = true;                        
        //             }
        //         } catch (InvalidOperationException) { }
        //     }
        // }

        if ((tryToBranch && Children.Count > 0 && Children.Count < 3) || (tryToGrow && Children.Count == 0))
        {
            try { 
                Vector3 deltaPosition = transform.position - targetPosition;
                deltaPosition = deltaPosition.normalized;
                Vector3 newTargetPosition = transform.position + deltaPosition * MainHub.StalkLength;        
                
                int resourcesAmount = 0;
                Vector3 resourceMidPoint = MainHub.ResourceManager.GetResourceMidpoint(newTargetPosition, Resource.ResourceState.Claimed, MainHub.StalkLength, out resourcesAmount);
                float minAngle = 90;

                if (Children.Count > 0) {
                    minAngle = Children.Select(x => Vector3.Angle(x.transform.position - transform.position, resourceMidPoint - transform.position)).OrderBy(x => x).First();
                    MainHub.MidPointShower.transform.position = resourceMidPoint;
                }
                if (minAngle > 30 && resourcesAmount >= MainHub.MinimumResourceAmountToGrow) {
                    Vector3 deltaVector = resourceMidPoint - transform.position;
                    deltaVector = deltaVector.normalized * MainHub.StalkLength;

                    Node newNode = MainHub.CreateNode(transform.position + deltaVector);
                    newNode.Parent = this;
                    Children.Add(newNode);
                    _childNodes = null;
                    didGrow = true;                        
                }
            } catch (InvalidOperationException) { }
        }

        if (!didGrow)
        {
            foreach (Node node in childNodes)
            {
                if (node.Grow())
                {
                    didGrow = true;
                    break;
                }
            }
        }

        if (didGrow) {
            IncreaseSize();
        }

        return didGrow;
    }

    void IncreaseSize()
    {
        int childSize = 0;
        if (Children.Count > 0)
            childSize = Children.Select(x => x.GetComponent<Node>().Size).Max(x => x);
        Size = childSize + 1;
        if (Size > MaxSize) Size = MaxSize;

        stalkTransform.localScale = new Vector3(stalkTransform.localScale.x, Size * SizeScale, stalkTransform.localScale.z);
    }
}