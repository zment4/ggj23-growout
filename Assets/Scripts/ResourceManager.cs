using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ResourceManager : MonoBehaviour
{
    public Color UnclaimedColor;
    public Color ClaimedColor;
    public Color ConsumedColor;
    private new ParticleSystem particleSystem;
    private List<Resource> resources = new List<Resource>();
    private Color[] ColorByState;

    void Start()
    {
        ColorByState = new Color[] { UnclaimedColor, ClaimedColor, ConsumedColor };

        particleSystem = GetComponent<ParticleSystem>();
        
        MainModule _mainModule = particleSystem.main;

        _mainModule.simulationSpeed = 1;
        particleSystem.Simulate(1, false, true, false);
        particleSystem.Play();
        _mainModule.simulationSpeed = 0;
        particleSystem.Pause();

        Particle[] particles = new Particle[_mainModule.maxParticles];
        particleSystem.GetParticles(particles);
        
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

        particleSystem.SetParticles(particles);
    }

    public List<Resource> GetResourcesInsideCircle(Vector3 position, float maxDistance)
        => resources.Where(x => (x.Position - position).magnitude <= maxDistance).ToList();

    public void SetStateInsideCircle(Vector3 position, float maxDistance, Resource.ResourceState fromState, Resource.ResourceState toState)
    {
        List<Resource> insideResources = GetResourcesInsideCircle(position, maxDistance)
            .Where(x => x.State == fromState).ToList();

        MainModule _mainModule = particleSystem.main;

        Particle[] particles = new Particle[_mainModule.maxParticles];
        particleSystem.GetParticles(particles);

        foreach (Resource resource in insideResources)
        {
            Particle particle = particles[resource.Index];

            particle.startColor = ColorByState[(int) toState];
            resource.State = toState;
            
            particles[resource.Index] = particle;
        }
        
        particleSystem.SetParticles(particles);        
    }
}
