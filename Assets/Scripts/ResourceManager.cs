using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;
using static UnityEngine.ParticleSystem;

public class ResourceManager : MonoBehaviour
{
    public Color UnclaimedColor;
    public Color ClaimedColor;
    public Color ConsumedColor;
    private ParticleSystem _particleSystem;
    private List<Resource> resources = new List<Resource>();
    private Color[] ColorByState;

    private List<Resource> _unclaimedResources;
    private  List<Resource> unclaimedResources {
        get {
            if (_unclaimedResources == null)
            {
                _unclaimedResources = resources.Where(x => x.State == Resource.ResourceState.Unclaimed).ToList();
            }

            return _unclaimedResources;
        }
    }
    private List<Resource> _claimedResources;
    private  List<Resource> claimedResources {
        get {
            if (_claimedResources == null)
            {
                _claimedResources = resources.Where(x => x.State == Resource.ResourceState.Claimed).ToList();
            }

            return _claimedResources;
        }
    }
    private List<Resource> _consumedResources;
    private List<Resource> consumedResources {
        get {
            if (_consumedResources == null)
            {
                _consumedResources = resources.Where(x => x.State == Resource.ResourceState.Consumed).ToList();
            }

            return _consumedResources;
        }
    }

    void LateUpdate()
    {
        UpdateParticles();
    }

    void Start()
    {
        ColorByState = new Color[] { UnclaimedColor, ClaimedColor, ConsumedColor };

        _particleSystem = GetComponent<ParticleSystem>();
        
        MainModule _mainModule = _particleSystem.main;

        _mainModule.simulationSpeed = 1;
        _particleSystem.Simulate(1, false, true, false);
        _particleSystem.Play();
        _mainModule.simulationSpeed = 0;
        _particleSystem.Pause();

        Particle[] particles = new Particle[_mainModule.maxParticles];
        _particleSystem.GetParticles(particles);
        
        for (int i = 0; i < particles.Length; i++)
        {
            Particle particle = particles[i];
            resources.Add(new Resource() {
                Position = new Vector3(particle.position.x, particle.position.y, 0),
                State = Resource.ResourceState.Unclaimed,
                Index = i
            });

            particle.startColor = UnclaimedColor;
        }

        _unclaimedResources = null;
        _claimedResources = null;
        _consumedResources = null;
        
        _particleSystem.SetParticles(particles);
    }

    public Vector3 GetResourceMidpoint(Vector3 position, Resource.ResourceState state, float maxDistance, out int resourcesAmount)
    {
        float maxSqrDistance = maxDistance * maxDistance;
        IEnumerable<Resource> resourcesList = null;

        if (state == Resource.ResourceState.Unclaimed)
            resourcesList = unclaimedResources
                .Where(x => (x.Position - position).sqrMagnitude <= maxSqrDistance);
        if (state == Resource.ResourceState.Claimed)
            resourcesList = claimedResources
                .Where(x => (x.Position - position).sqrMagnitude <= maxSqrDistance);
        if (state == Resource.ResourceState.Consumed)
            resourcesList = consumedResources
                .Where(x => (x.Position - position).sqrMagnitude <= maxSqrDistance);

        resourcesAmount = resourcesList.Count();
        Vector3 midpoint = resourcesList.Select(x => x.Position).Aggregate((x, y) => x + y) / resourcesAmount;
        return midpoint;
    }

    public IEnumerable<Resource> GetResourcesInsideCircle(Vector3 position, float maxDistance, Resource.ResourceState state)
    {
        IEnumerable<Resource> resourcesList = null;
        float maxSqrDistance = maxDistance * maxDistance;
        if (state == Resource.ResourceState.Unclaimed)
            resourcesList = unclaimedResources.Where(x => (x.Position - position).sqrMagnitude <= maxSqrDistance);
        if (state == Resource.ResourceState.Claimed)
            resourcesList = claimedResources.Where(x => (x.Position - position).sqrMagnitude <= maxSqrDistance);
        if (state == Resource.ResourceState.Consumed)
            resourcesList = consumedResources.Where(x => (x.Position - position).sqrMagnitude <= maxSqrDistance);

        return resourcesList;
    }

    public List<Resource> GetResourcesInsideCircle(Vector3 position, float maxDistance)
    {
        List<Resource> resourcesList = resources.Where(x => (x.Position - position).magnitude <= maxDistance).ToList();
        return resourcesList;
    }

    public void SetStateInsideCircle(Vector3 position, float maxDistance, Resource.ResourceState fromState, Resource.ResourceState toState)
    {
        IEnumerable<Resource> insideResources = GetResourcesInsideCircle(position, maxDistance, fromState);

        foreach (Resource resource in insideResources)
        {
            resource.State = toState;
        }

        if (fromState == Resource.ResourceState.Unclaimed)
            _unclaimedResources = null;
        if (fromState == Resource.ResourceState.Claimed || toState == Resource.ResourceState.Claimed)
            _claimedResources = null;
        if (toState == Resource.ResourceState.Consumed)
            _consumedResources = null;
    }

    void UpdateParticles()
    {
        MainModule _mainModule = _particleSystem.main;

        Particle[] particles = new Particle[_mainModule.maxParticles];
        _particleSystem.GetParticles(particles);

        foreach (Resource resource in resources)
        {
            Particle particle = particles[resource.Index];

            particle.startColor = ColorByState[(int) resource.State];
            
            particles[resource.Index] = particle;
        }

        _particleSystem.SetParticles(particles);
    }
}
