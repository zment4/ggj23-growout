using UnityEngine;

public class Resource {
    public Vector3 Position;

    public enum ResourceState {
        Unclaimed,
        Claimed,
        Consumed
    }

    public ResourceState State;
}