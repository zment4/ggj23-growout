using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [HideInInspector]
    public Vector3 TargetPosition;
    public List<MonoBehaviour> Children = new List<MonoBehaviour>();
    public MonoBehaviour Parent;
    [HideInInspector]
    public Hub MainHub;

    void Start()
    {
        Vector3 deltaPosition = TargetPosition - transform.position;
        Transform stalkTransform = transform.Find("Stalk");

        stalkTransform.localScale = new Vector3(deltaPosition.magnitude, stalkTransform.localScale.y, stalkTransform.localScale.z);

        MainHub.ResourceManager.SetStateInsideCircle(transform.position, 1f, Resource.ResourceState.Unclaimed, Resource.ResourceState.Claimed);
    }

    public void Grow() {
        if (Children.Count == 0)
        {
            try { 
                Vector3 deltaPosition = TargetPosition - transform.position;
                deltaPosition = deltaPosition.normalized;
                Vector3 newTargetPosition = transform.position + deltaPosition * 0.25f;        
                MainHub.MidPointShower.transform.position = newTargetPosition;
                
                Vector3 resourceMidPoint = MainHub.GetResourceMidpoint(newTargetPosition, Resource.ResourceState.Claimed);
                Vector3 deltaVector = resourceMidPoint - transform.position;
                deltaVector = deltaVector.normalized * MainHub.StalkLength;

                Node newNode = MainHub.CreateNode(transform.position + deltaVector);
                newNode.Parent = this;
                Children.Add(newNode);
            } catch (InvalidOperationException) { Debug.Log("Node: Empty Set"); }
        }
    }
}