using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HubController : MonoBehaviour
{
    [HideInInspector]
    public ResourceManager resourceManager;
    public GameObject MidPointShower;

    void Update()
    {
        if (Input.GetMouseButton(1)) 
        {
            Vector3 midpoint = GetResourceMidpoint();
            Vector3 delta = midpoint - transform.position;
            delta = delta.normalized * 0.35f;
            MidPointShower.transform.position = transform.position + delta;
        }    
    }

    Vector3 GetResourceMidpoint()
    {
        List<Resource> resources = resourceManager.GetResourcesInsideCircle(this.transform.position, 1);
        return resources.Select(x => x.Position).Aggregate((x, y) => x + y) / resources.Count;
    }
}
