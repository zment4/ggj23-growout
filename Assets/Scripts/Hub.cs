using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

public class Hub : MonoBehaviour
{
    public float ChanceToGrow = 0.5f;

    [HideInInspector]
    public ResourceManager ResourceManager;
    public GameObject MidPointShower;
    public GameObject NodePrefab;
    public float StalkLength;
    [HideInInspector]
    public List<MonoBehaviour> Children;

    [HideInInspector]
    public int Id;

    void Start() {
        StartCoroutine(GrowCoRoutine());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) 
        {
            Grow();
        }    
    }

    IEnumerator GrowCoRoutine()
    {
        yield return new WaitForSeconds(0.25f);

        Grow();

        StartCoroutine(GrowCoRoutine());
    }

    void Grow()
    {
        bool tryToGrow = UnityEngine.Random.Range(0f, 1f) < ChanceToGrow;
        bool didGrow = false;

        if (tryToGrow)
        {
            try {
                Vector3 midpoint = GetResourceMidpoint(transform.position, Resource.ResourceState.Claimed, StalkLength);
                Vector3 delta = midpoint - transform.position;
                delta = delta.normalized * StalkLength;
                Vector3 nodePosition = transform.position + delta;

                Children.Add(CreateNode(nodePosition));
                didGrow = true;
            } catch (InvalidOperationException) { }; // Eat exception if empty set
        }

        if (!didGrow)
        {
            foreach (Node node in Children)
            {
                if (node.Grow()) 
                    break;
            }
        }
        
        try {
            Vector3 midpoint = GetResourceMidpoint(transform.position, Resource.ResourceState.Unclaimed, StalkLength);
            Vector3 delta = transform.position - midpoint;
            delta = delta.normalized * 0.35f;
            MidPointShower.transform.position = transform.position + delta;

            Profiler.BeginSample("SetStateInsideCircle");
            ResourceManager.SetStateInsideCircle(MidPointShower.transform.position, StalkLength, Resource.ResourceState.Unclaimed, Resource.ResourceState.Claimed);
            Profiler.EndSample();
        } catch (InvalidOperationException) {}; // Eat exception if empty set
    }

    public Vector3 GetResourceMidpoint(Vector3 position, Resource.ResourceState state, float radius)
    {
        List<Resource> resources = ResourceManager.GetResourcesInsideCircle(position, radius);
        List<Vector3> resourcePositions = resources.Where(x => x.State == state).Select(x => x.Position).ToList();
        return resourcePositions.Aggregate((x, y) => x + y) / resourcePositions.Count;
    }

    public Node CreateNode(Vector3 position)
    {
        GameObject newNodeGameObject = GameObject.Instantiate(NodePrefab, position, NodePrefab.transform.rotation);
        Node newNode = newNodeGameObject.GetComponent<Node>();
        newNode.Parent = this;
        newNode.MainHub = this;
        newNodeGameObject.transform.parent = transform;
        newNodeGameObject.name = "Node";

        ResourceManager.SetStateInsideCircle(position, StalkLength, Resource.ResourceState.Claimed, Resource.ResourceState.Consumed);
        return newNode;
    }
}
