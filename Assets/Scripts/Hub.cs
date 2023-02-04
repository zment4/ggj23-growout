using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hub : MonoBehaviour
{
    [HideInInspector]
    public ResourceManager ResourceManager;
    public GameObject MidPointShower;
    public GameObject NodePrefab;
    public float StalkLength;
    [HideInInspector]
    public List<MonoBehaviour> Children;

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) 
        {
            foreach (Node node in Children)
            {
                node.Grow();
            }
            
            try {
                Vector3 midpoint = GetResourceMidpoint(Resource.ResourceState.Claimed);
                Vector3 delta = midpoint - transform.position;
                delta = delta.normalized * StalkLength;

                ResourceManager.SetStateInsideCircle(MidPointShower.transform.position, StalkLength / 2, Resource.ResourceState.Claimed, Resource.ResourceState.Consumed);
                CreateNode(transform.position + delta);
            } catch (InvalidOperationException) { Debug.Log("Empty set"); }; // Eat exception if empty set

            try {
                Vector3 midpoint = GetResourceMidpoint(Resource.ResourceState.Unclaimed);
                Vector3 delta = midpoint - transform.position;
                delta = delta.normalized * 0.35f;
                MidPointShower.transform.position = transform.position + delta;

                ResourceManager.SetStateInsideCircle(MidPointShower.transform.position, StalkLength * 2, Resource.ResourceState.Unclaimed, Resource.ResourceState.Claimed);
            } catch (InvalidOperationException) {}; // Eat exception if empty set
        }    
    }

    Vector3 GetResourceMidpoint(Resource.ResourceState state)
    {
        List<Resource> resources = ResourceManager.GetResourcesInsideCircle(this.transform.position, 1);
        return resources.Where(x => x.State == state).Select(x => x.Position).Aggregate((x, y) => x + y) / resources.Count;
    }

    public Node CreateNode(Vector3 position)
    {
        GameObject newNodeGameObject = GameObject.Instantiate(NodePrefab, position, NodePrefab.transform.rotation);
        Node newNode = newNodeGameObject.GetComponent<Node>();
        newNode.TargetPosition = transform.position;
        newNode.MainHub = this;
        return newNode;
    }
}
