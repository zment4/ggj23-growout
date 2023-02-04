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
    public MonoBehaviour Parent;
    [HideInInspector]
    public Hub MainHub;

    private Transform stalkTransform;

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
        
        MainHub.ResourceManager.SetStateInsideCircle(transform.position, MainHub.StalkLength * 2, Resource.ResourceState.Unclaimed, Resource.ResourceState.Claimed);
    }

    public bool Grow() {
        bool tryToGrow = UnityEngine.Random.Range(0f, 1f) < MainHub.ChanceToGrow;
        bool didGrow = false;

        if (tryToGrow)
        {
            try { 
                Vector3 deltaPosition = transform.position - targetPosition;
                deltaPosition = deltaPosition.normalized;
                Vector3 newTargetPosition = transform.position + deltaPosition * MainHub.StalkLength;        
                
                Vector3 resourceMidPoint = MainHub.GetResourceMidpoint(newTargetPosition, Resource.ResourceState.Claimed, MainHub.StalkLength);
                Vector3 deltaVector = resourceMidPoint - transform.position;
                deltaVector = deltaVector.normalized * MainHub.StalkLength;

                Profiler.BeginSample("Node.Grow");
                Node newNode = MainHub.CreateNode(transform.position + deltaVector);
                Profiler.EndSample();
                newNode.Parent = this;
                Children.Add(newNode);
                didGrow = true;
            } catch (InvalidOperationException) { }
        }

        if (!didGrow)
        {
            foreach (Node node in Children.Select(x => x.GetComponent<Node>()))
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
        Size++;
        if (Size > MaxSize) Size = MaxSize;

        stalkTransform.localScale = new Vector3(stalkTransform.localScale.x, Size * SizeScale, stalkTransform.localScale.z);
    }
}