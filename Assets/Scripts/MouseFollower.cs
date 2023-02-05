using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    private ResourceManager resourceManager;

    void Start()
    {
        resourceManager = GameObject.Find("ResourceManager").GetComponent<ResourceManager>(); 
    }

    void Update()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        transform.position = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, 0);

        if (Input.GetMouseButton(0)) {
            resourceManager.SetStateInsideCircle(transform.position, 0.5f, Resource.ResourceState.Unclaimed, Resource.ResourceState.Claimed);
        }
    }
}
