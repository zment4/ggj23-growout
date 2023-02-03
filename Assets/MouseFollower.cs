using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    void Update()
    {
        LateUpdate();
    }
    
    void LateUpdate()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        transform.position = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, 0);
    }
}
