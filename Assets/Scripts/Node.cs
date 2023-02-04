using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [HideInInspector]
    public Vector3 TargetPosition;
    public List<MonoBehaviour> Children;
    public MonoBehaviour Parent;
    [HideInInspector]
    public Hub MainHub;

    void Start()
    {
        Vector3 deltaPosition = TargetPosition - transform.position;
        Transform stalkTransform = transform.Find("Stalk");

        stalkTransform.localScale = new Vector3(deltaPosition.magnitude, stalkTransform.localScale.y, stalkTransform.localScale.z);
    }

    public void Grow() {
        Vector3 deltaPosition = transform.position - TargetPosition;
        Transform stalkTransform = transform.Find("Stalk");
        
    }
}