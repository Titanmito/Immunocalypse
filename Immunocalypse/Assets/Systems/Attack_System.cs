using UnityEngine;
using FYFY;
using FYFY_plugins.TriggerManager;
using System.Collections.Generic;
using System.Linq;

public class Attack_System : FSystem
{
	// This system manages the attack of towers to enemies. 
	// First, it actualizes the time since the last attack of each tower. Then, it goes thru each tower that has an enemy close enough (using Triggered2D),
	// actualizes the health of the enemy and resets the last_attack counter so a tower doesn't attack multiple time in one loop.
	private Family _AllAttackersGO = FamilyManager.getFamily(new AllOfComponents(typeof(Can_Attack)));

	private Family _AnticorpsGO = FamilyManager.getFamily(new AllOfComponents(typeof(Can_Attack), typeof(Has_Health), typeof(Anticorps), typeof(Triggered2D)));
	private Family _MacrophageGO = FamilyManager.getFamily(new AllOfComponents(typeof(Can_Attack), typeof(Triggered2D)), new NoneOfComponents(typeof(Has_Health)));

    private Family _AudioSources = FamilyManager.getFamily(new AllOfComponents(typeof(AudioSource)));
    private static string antibody_hit_audio_source_name = "AntibodyHitAudioSource", macrophage_hit_audio_source_name = "MacrophageHitAudioSource";
    private string[] audio_source_names = { antibody_hit_audio_source_name, macrophage_hit_audio_source_name };
    private Dictionary<string, AudioSource> audio_sources_dict;

    public Attack_System()
    {
        // this.Pause = true;
        audio_sources_dict = new Dictionary<string, AudioSource>();
        foreach (GameObject go_with_audio_source in _AudioSources)
            if (audio_source_names.Contains(go_with_audio_source.name))
                audio_sources_dict[go_with_audio_source.name] = go_with_audio_source.GetComponent<AudioSource>();
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
        _AllAttackersGO = FamilyManager.getFamily(new AllOfComponents(typeof(Can_Attack)));
        _AnticorpsGO = FamilyManager.getFamily(new AllOfComponents(typeof(Can_Attack), typeof(Has_Health), typeof(Anticorps), typeof(Triggered2D)));
        _MacrophageGO = FamilyManager.getFamily(new AllOfComponents(typeof(Can_Attack), typeof(Triggered2D)), new NoneOfComponents(typeof(Has_Health)));
    }

    protected override void onProcess(int familiesUpdateCount)
	{
		// we prepare the next attack for each of the allies
		foreach (GameObject go in _AllAttackersGO)
        {
			Can_Attack ca = go.GetComponent<Can_Attack>();

			if (ca.last_attack < ca.attack_speed)
			{
				ca.last_attack += Time.deltaTime;
			}
		}

		// macrophages attack
		foreach (GameObject go in _MacrophageGO)
		{
			Triggered2D t2d = go.GetComponent<Triggered2D>();
			Can_Attack ca = go.GetComponent<Can_Attack>();

			// Maybe we could recover only the first GO but to be sure we get an enemy if it is there, we opt to go thru all the GO. 
			foreach (GameObject target in t2d.Targets)
			{
				Has_Health h_e = target.GetComponent<Has_Health>();
				if (h_e != null && target.gameObject.CompareTag("Respawn") && ca.last_attack >= ca.attack_speed)
				{
					h_e.health -= ca.strength;
					ca.last_attack = 0f;

					// changes the color of the tower to indicate it just attacked
					SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
					sr.color = new Color(1, 0, 0, 1);

                    audio_sources_dict[macrophage_hit_audio_source_name].Play();
				}
			}
		}

		// anticorps attack
		foreach (GameObject go in _AnticorpsGO)
		{
			Triggered2D t2d = go.GetComponent<Triggered2D>();
			Can_Attack ca = go.GetComponent<Can_Attack>();
			Has_Health hh = go.GetComponent<Has_Health>();
			int type = go.GetComponent<Anticorps>().type;

			// Maybe we could recover only the first GO but to be sure we get an enemy if it is there, we opt to go thru all the GO. 
			foreach (GameObject target in t2d.Targets)
			{
				Has_Health h_e = target.GetComponent<Has_Health>();
				if (h_e != null && target.gameObject.CompareTag("Respawn") && ca.last_attack >= ca.attack_speed)
				{
					if (type == 1)
					{
						Virus vv = target.GetComponent<Virus>();
						if (vv != null)
						{
							h_e.health -= ca.strength;
							ca.last_attack = 0f;

							// We want to put the anticorps health to zero if it has attacked so it's destroyed just after.
							hh.health = -1;
						}
					}
					if (type == 2)
                    {
						Bacterie bb = target.GetComponent<Bacterie>();
						if (bb != null)
						{
							h_e.health -= ca.strength;
							ca.last_attack = 0f;

							// We want to put the anticorps health to zero if it has attacked so it's destroyed just after.
							hh.health = -1;
						}
					}

                    audio_sources_dict[antibody_hit_audio_source_name].Play();
				}

			}

		}
	}
}