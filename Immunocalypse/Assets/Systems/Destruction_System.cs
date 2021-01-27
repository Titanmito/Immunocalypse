using UnityEngine;
using UnityEngine.UI;
using FYFY;

public class Destruction_System : FSystem {
	// This system manages the destruction of enemies. It also controls the energy bonus the player gets each time they kill an enemy.
	// The part controling the end of the level is implemented at Load_Scene_System!!

	private Family _EnemiesAliveGO = FamilyManager.getFamily(new AnyOfTags("Respawn"), new AllOfComponents(typeof(Has_Health), typeof(Attack_J), typeof(Create_Particles_After_Death)));
	private Family _AlliesAliveGO = FamilyManager.getFamily(new AnyOfTags("Tower"), new AllOfComponents(typeof(Has_Health), typeof(Lifespan)));
    private Family _ParticlesAliveGO = FamilyManager.getFamily(new AnyOfTags("Particle"), new AllOfComponents(typeof(Lifespan)), new NoneOfComponents(typeof(Has_Health)));

    private Family _Spawn = FamilyManager.getFamily(new AllOfComponents(typeof(Spawn), typeof(Active_Lvl_Buttons)));
    private Family _Energy_nb = FamilyManager.getFamily(new AnyOfTags("Energy"), new AllOfComponents(typeof(Text)));
    private Family _Joueur = FamilyManager.getFamily(new AnyOfTags("Player"), new AllOfComponents(typeof(Has_Health), typeof(Bank), typeof(Score)));
    private int energy_enemy;
    private Bank bank;
    private Text energy_nb;
    private int score_enemy;
    private Score score;

    private float _DestroyParticlesProgress = 0.0f, _DestroyParticlesReload = 0.05f;

    public Destruction_System()
    {
        // this.Pause = true;
    }

    protected override void onPause(int currentFrame)
    {
        // Debug.Log("System " + this.GetType().Name + " go on pause");
    }

    protected override void onResume(int currentFrame)
    {
        // Debug.Log("System " + this.GetType().Name + " go on resume ; " + currentFrame.ToString());
        if (currentFrame == 1)
        {
            this.Pause = true;
            return;
        }
        _EnemiesAliveGO = FamilyManager.getFamily(new AnyOfTags("Respawn"), new AllOfComponents(typeof(Has_Health), typeof(Attack_J), typeof(Create_Particles_After_Death)));
        _AlliesAliveGO = FamilyManager.getFamily(new AnyOfTags("Tower"), new AllOfComponents(typeof(Has_Health), typeof(Lifespan)));
        _ParticlesAliveGO = FamilyManager.getFamily(new AnyOfTags("Particle"), new AllOfComponents(typeof(Lifespan)), new NoneOfComponents(typeof(Has_Health)));

        _Spawn = FamilyManager.getFamily(new AllOfComponents(typeof(Spawn), typeof(Active_Lvl_Buttons)));
        _Energy_nb = FamilyManager.getFamily(new AnyOfTags("Energy"), new AllOfComponents(typeof(Text)));
        _Joueur = FamilyManager.getFamily(new AnyOfTags("Player"), new AllOfComponents(typeof(Has_Health), typeof(Bank), typeof(Score)));
        energy_enemy = _Spawn.First().GetComponent<Spawn>().energy_enemy;
        bank = _Joueur.First().GetComponent<Bank>();
        energy_nb = _Energy_nb.First().GetComponent<Text>();
        score_enemy = _Spawn.First().GetComponent<Spawn>().score_enemy;
        score = _Joueur.First().GetComponent<Score>();
    }

    protected override void onProcess(int familiesUpdateCount)
	{
        float angle = 0.0f;
        _DestroyParticlesProgress += Time.deltaTime;
		// destroy enemies that should be destroyed.
		foreach (GameObject go in _EnemiesAliveGO)
		{
			Has_Health hh = go.GetComponent<Has_Health>();
			Attack_J aj = go.GetComponent<Attack_J>();
            Create_Particles_After_Death cpad = go.GetComponent<Create_Particles_After_Death>();

            // we give the bonus energy to the player.
            if (hh.health <= 0 && !aj.has_attacked)
            {
                bank.energy += energy_enemy;
                score.lvl_score += score_enemy;

                // Actualizes the energy display to the player.
                energy_nb.text = "energy: " + bank.energy.ToString();
            }
            // it destroys all enemies with negative or equal to zero energy and those who have already attacked (and aren't visible anymore).
            if (hh.health <= 0 || aj.has_attacked)
            {
                // If an enemy has a component Create_Particles_After_Death it means that we must create particles in the place where it dies.
                if (cpad != null && aj.has_attacked)
                {
                    for (int i = 0; i < cpad.particles_number; i++)
                    {
                        GameObject particle = UnityEngine.Object.Instantiate<GameObject>(cpad.particles_prefab, go.transform.position, go.transform.rotation);
                        GameObjectManager.bind(particle);
                        particle.GetComponent<SpriteRenderer>().color = go.GetComponent<SpriteRenderer>().color;
                        particle.transform.Rotate(new Vector3(0, 0, Random.value * 360));
                        angle = Mathf.PI * particle.transform.rotation.eulerAngles.z / 180.0f;
                        Rigidbody2D prb = particle.GetComponent<Rigidbody2D>();
                        prb.AddForce(new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * cpad.explosion_force * Random.Range(0.0f, 1.0f));
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
        // ATTENTION This loop would probably force a RETURN OF THE METHOD that is why it is necessary to put it at the very end of this method!!!
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