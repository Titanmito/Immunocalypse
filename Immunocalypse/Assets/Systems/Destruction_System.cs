using UnityEngine;
using FYFY;

public class Destruction_System : FSystem {
	// This system manages the destruction of enemies and ends the level if the Joueur has negative or equal to zero health.
	// The part controling the end of the level is not yet implemented.

	private Family _EnemiesAliveGO = FamilyManager.getFamily(new AnyOfTags("Respawn"));
	private Family _AlliesAliveGO = FamilyManager.getFamily(new AnyOfTags("Tower"), new AllOfComponents(typeof(Has_Health)), new AllOfComponents(typeof(Lifespan)));
    private Family _ParticlesAliveGO = FamilyManager.getFamily(new AnyOfTags("Particle"), new AllOfComponents(typeof(Lifespan)));
    private float _DestroyParticlesProgress = 0.0f, _DestroyParticlesReload = 0.05f;

	protected override void onProcess(int familiesUpdateCount)
	{
        _DestroyParticlesProgress += Time.deltaTime;
		// destroy enemies that should be destroyed.
		foreach (GameObject go in _EnemiesAliveGO)
		{
			Has_Health hh = go.GetComponent<Has_Health>();
			Attack_J aj = go.GetComponent<Attack_J>();
            Create_Particles_After_Death cpad = go.GetComponent<Create_Particles_After_Death>();

			// it destroys all enemies with negative or equal to zero energy and those who have already attacked (and aren't visible anymore).
			if (hh.health <= 0 || aj.has_attacked)
			{
                // If an enemy has a component Create_Particles_After_Death it means that we must create particles in the place where it dies
                if (cpad != null)
                {
                    for (int i = 0; i < cpad.particles_number; i++)
                    {
                        GameObject particle = UnityEngine.Object.Instantiate<GameObject>(cpad.particles_prefab, go.transform.position, go.transform.rotation);
                        GameObjectManager.bind(particle);
                        particle.GetComponent<SpriteRenderer>().color = go.GetComponent<SpriteRenderer>().color;
                        Rigidbody2D prb = particle.GetComponent<Rigidbody2D>();
                        prb.AddForce(new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)) * cpad.explosion_force);
                    }
                }
				GameObjectManager.unbind(go);
				Object.Destroy(go);
			}
		}

		// destroy allies that should be destroyed.
		foreach (GameObject go in _AlliesAliveGO)
		{
			Has_Health hh = go.GetComponent<Has_Health>();
			Lifespan ls = go.GetComponent<Lifespan>();

			if (hh.health <= 0 || ls.lifespan <= 0)
            {
				GameObjectManager.unbind(go);
				Object.Destroy(go);
			}
			else
            {
				ls.lifespan -= Time.deltaTime;
			}

        }

        // This loop destroys all the particles after they live their lifespan
        // ATTENTION This loop would probably force a RETURN OF THE METHOD that is why it is necessary to put it at the very end of this method !!!
        foreach (GameObject go in _ParticlesAliveGO)
        {
            Lifespan ls = go.GetComponent<Lifespan>();

            if (ls.lifespan <= 0 && _DestroyParticlesProgress >= _DestroyParticlesReload)
            {
                GameObjectManager.unbind(go);
                Object.Destroy(go);
                _DestroyParticlesProgress = 0f;
                return;
            }
            else
            {
                ls.lifespan -= Time.deltaTime;
            }

        }

    }
}