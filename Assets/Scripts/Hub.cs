using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

public class Hub : MonoBehaviour
{
    public int MinimumResourceAmountToGrow = 300;
    [HideInInspector]
    public float CurrentInfluenceSize = 1f;
    public float MaxInfluenceSize = 3f;
    public float InfluenceIncrease = 0.25f;
    public float StartInfluenceSize = 0.25f;
    public float ChanceToBranch = 0.75f;
    public Color FreeColor;
    public Color OwnedColor;
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

    [HideInInspector]
    public HubCreator HubCreator;

    public bool Active = false;

    private Material _graphicsMaterial;
    private Material graphicsMaterial {
        get {
            if (_graphicsMaterial == null) {
                _graphicsMaterial = transform.Find("Graphics").GetComponent<MeshRenderer>().material;

            }

            return _graphicsMaterial;
        }
    }

    void Start() {
        // StartCoroutine(GrowCoRoutine());

        if (!Active) graphicsMaterial.color = FreeColor;
    }

    public void OwnHub()
    {
        Active = true;
        graphicsMaterial.color = OwnedColor;
    }

    void Update()
    {
        // if (Input.GetMouseButtonDown(1)) 
        {
            Grow();
        }    
    }

    IEnumerator GrowCoRoutine()
    {
        yield return new WaitForSeconds(0.1f);

        Grow();

        StartCoroutine(GrowCoRoutine());
    }

    void Grow()
    {
        if (!Active) return;

        CurrentInfluenceSize += InfluenceIncrease;
        if (CurrentInfluenceSize > MaxInfluenceSize) CurrentInfluenceSize = MaxInfluenceSize;

        bool tryToGrow = UnityEngine.Random.Range(0f, 1f) < ChanceToGrow;
        bool didGrow = false;

        if (tryToGrow)
        {
            try {
                int resourcesAmount = 0;
                Vector3 midpoint = ResourceManager.GetResourceMidpoint(transform.position, Resource.ResourceState.Claimed, StalkLength, out resourcesAmount);
                float minAngle = 90;
                if (Children.Count > 0) {
                    minAngle = Children.Select(x => Vector3.Angle(x.transform.position - transform.position, midpoint - transform.position)).OrderBy(x => x).First();
                }
                if (minAngle > 30 && resourcesAmount >= MinimumResourceAmountToGrow) {
                    Vector3 delta = midpoint - transform.position;
                    delta = delta.normalized * StalkLength;
                    Vector3 nodePosition = transform.position + delta;

                    Children.Add(CreateNode(nodePosition));
                    didGrow = true;
                }
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
            ResourceManager.SetStateInsideCircle(transform.position, CurrentInfluenceSize, Resource.ResourceState.Unclaimed, Resource.ResourceState.Claimed);
        } catch (InvalidOperationException) {}; // Eat exception if empty set
    }

    public Vector3 GetResourceMidpoint(Vector3 position, Resource.ResourceState state, float radius, out int resourcesAmount)
    {
        return ResourceManager.GetResourceMidpoint(position, state, radius, out resourcesAmount);
    }

    public Node CreateNode(Vector3 position)
    {
        GameObject newNodeGameObject = GameObject.Instantiate(NodePrefab, position, NodePrefab.transform.rotation);
        Node newNode = newNodeGameObject.GetComponent<Node>();
        newNode.Parent = this;
        newNode.MainHub = this;
        newNodeGameObject.transform.parent = transform;
        newNodeGameObject.name = "Node";

        ResourceManager.SetStateInsideCircle(position, StalkLength * 0.75f, Resource.ResourceState.Claimed, Resource.ResourceState.Consumed);

        List<Hub> inactiveHubs = HubCreator.Hubs.Where(x => !x.Active).ToList();
        if (inactiveHubs.Count == 0)
            return newNode;

        Hub closestHub = inactiveHubs.OrderBy(x => (x.transform.position - position).magnitude).First();
        if ((position - closestHub.transform.position).magnitude < StalkLength * 2) {
            closestHub.OwnHub();
            newNode.Active = false;
        }
        return newNode;
    }
}
