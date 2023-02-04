using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HubCreator : MonoBehaviour
{
    public GameObject HubPrefab;
    public int HubsToCreate;
    public float MinDistance;
    public Vector2 GamePlaneSize;

    private List<GameObject> hubObjects = new List<GameObject>();

    public List<Hub> Hubs => hubObjects.Select(x => x.GetComponent<Hub>()).ToList();

    void Start()
    {
        ResourceManager resourceManager = GameObject.Find("ResourceManager").GetComponent<ResourceManager>();

        float gamePlaneExtentX = GamePlaneSize.x / 2;
        float gamePlaneExtentY = GamePlaneSize.y / 2;

        for (int i = 0; i < HubsToCreate; i++)
        {
            bool randomSuccessful = false;
            while (!randomSuccessful)
            {
                float newX = Random.Range(-gamePlaneExtentX, gamePlaneExtentX);
                float newY = Random.Range(-gamePlaneExtentY, gamePlaneExtentY);

                Vector3 newPosition = new Vector3(newX, newY, 0);
                
                GameObject closestHub = hubObjects.OrderBy(x => (x.transform.position - newPosition).magnitude).FirstOrDefault();
                if (closestHub == null || (closestHub.transform.position - newPosition).magnitude > MinDistance) {
                    GameObject newHub = GameObject.Instantiate(HubPrefab, newPosition, HubPrefab.transform.localRotation);
                    hubObjects.Add(newHub);
                    newHub.transform.parent = transform;
                    newHub.name = $"Hub {i}";

                    Hub hub = newHub.GetComponent<Hub>();
                    hub.ResourceManager = resourceManager;
                    hub.Id = i;
                    hub.HubCreator = this;

                    if (i == 0)
                    {
                        hub.OwnHub();
                    }

                    randomSuccessful = true;
                }
            }
        }   
    }
}
