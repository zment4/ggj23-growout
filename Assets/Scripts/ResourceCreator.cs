using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceCreator : MonoBehaviour
{
    public GameObject ResourcePrefab;
    public int ResourcesToCreate;
    public Vector2 GamePlaneSize;

    private List<GameObject> resourceObjects = new List<GameObject>();

    void Start()
    {
        float gamePlaneExtentX = GamePlaneSize.x / 2;
        float gamePlaneExtentY = GamePlaneSize.y / 2;

        for (int i = 0; i < ResourcesToCreate; i++)
        {
            float newX = Random.Range(-gamePlaneExtentX, gamePlaneExtentX);
            float newY = Random.Range(-gamePlaneExtentY, gamePlaneExtentY);

            Vector3 newPosition = new Vector3(newX, newY, 0);

            GameObject newResource = GameObject.Instantiate(ResourcePrefab, newPosition, ResourcePrefab.transform.localRotation);
            resourceObjects.Add(newResource);
            newResource.transform.parent = transform;
            newResource.name = $"Resource {i}";
        }   
    }
}
