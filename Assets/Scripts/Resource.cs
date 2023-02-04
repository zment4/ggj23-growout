using UnityEngine;

public class Resource {
    public Vector3 Position;

    public enum ResourceState {
        Unclaimed = 0,
        Claimed,
        Consumed
    }

    public ResourceState State;

    public int Index;
}