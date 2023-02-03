using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ResourceManager : MonoBehaviour
{
    public Color ConsumedColor;
    public Color ClaimedColor;
    public Color UnclaimedColor;
    private new ParticleSystem particleSystem;
    private List<Resource> resources = new List<Resource>();
    void Start()
    {
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
                State = Resource.ResourceState.Unclaimed
            });

            particle.startColor = UnclaimedColor;
        }

        particleSystem.SetParticles(particles);
    }

    void Update()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        mouseWorldPosition = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, transform.position.z);

        MainModule _mainModule = particleSystem.main;

        Particle[] particles = new Particle[_mainModule.maxParticles];
        particleSystem.GetParticles(particles);

        List<Particle> newParticles = new List<Particle>();
        newParticles.AddRange(particles);

        for (int i = 0; i < particles.Length; i++)
        {
            Particle particle = newParticles[i];
            if ((particle.position - mouseWorldPosition).magnitude < 1)
                particle.startColor = new Color(1, 0, 0, 1);
            else 
                particle.startColor = new Color(0, 1, 0, 1);

            newParticles[i] = particle;
        }
        
        particleSystem.SetParticles(newParticles.ToArray());

        if (Input.GetMouseButton(0))
        {
            _mainModule.maxParticles += 100;
            for (int i = 0; i < 100; i++)
            {
                Vector3 newParticlePosition = Random.insideUnitCircle;
                newParticlePosition = new Vector3(newParticlePosition.x, newParticlePosition.y, 0);
                newParticlePosition += mouseWorldPosition;
                EmitParams emitParams = new EmitParams() {
                    startColor = new Color(1, 0, 0, 1),
                    startSize = newParticles[0].startSize,
                    position = newParticlePosition
                };
                particleSystem.Emit(emitParams, 1);
                resources.Add(new Resource() {
                    Position = newParticlePosition,
                    State = Resource.ResourceState.Unclaimed
                });
            }
        }        
    }

    public List<Resource> GetResourcesInsideCircle(Vector3 position, float maxDistance)
        => resources.Where(x => (x.Position - position).magnitude <= maxDistance).ToList();
}
