using UnityEngine;

public class Create_Particles_After_Death : MonoBehaviour {
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).

    // The number of particles to create after death
    public int particles_number;

    // Prefab of particles
    public GameObject particles_prefab;

    // Force that would be used to the particle when it would be created
    public float explosion_force = 100f;

}