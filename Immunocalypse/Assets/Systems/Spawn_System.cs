using UnityEngine;
using FYFY;
using UnityEngine.UI;
using System.Reflection;
using System;
using System.Collections.Generic;


public class Spawn_System : FSystem {
	// This system manages the creation of new enemies.

	private Family _Spawn = FamilyManager.getFamily(new AllOfComponents(typeof(Spawn)));
	private Family _Buttons = FamilyManager.getFamily(new AnyOfTags("Button"), new AllOfComponents(typeof(Link_Prefab)));
	private Family _Spawn_nb = FamilyManager.getFamily(new AnyOfTags("Wave"), new AllOfComponents(typeof(Text)));

	private Spawn spawn;
	private Text spawn_nb;

    // we have four types of enemies now 
    // 0 -> Virus1
    // 1 -> Virus2
    // 2 -> Bacterie1
    // 3 -> Bacterie2
    private GameObject[] enemies = new GameObject[4];

	public Spawn_System()
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

        _Spawn = FamilyManager.getFamily(new AllOfComponents(typeof(Spawn)));
        _Buttons = FamilyManager.getFamily(new AnyOfTags("Button"), new AllOfComponents(typeof(Link_Prefab)));
        _Spawn_nb = FamilyManager.getFamily(new AnyOfTags("Wave"), new AllOfComponents(typeof(Text)));

        spawn = _Spawn.First().GetComponent<Spawn>();
        spawn_nb = _Spawn_nb.First().GetComponent<Text>();

        // To facilitate the creation of enemies in onProcess.
        // I couldn't find an automated way of creating this array.
        enemies[0] = spawn.virus1_prefab;
        enemies[1] = spawn.virus2_prefab;
        enemies[2] = spawn.bacterie1_prefab;
        enemies[3] = spawn.bacterie2_prefab;

        // Puts the price of each tower and special power in their respective buttons.
        // buttons must be named as *same start as the name of the respective prefab*_button.
        foreach (GameObject button in _Buttons)
        {
            Text t = button.transform.GetChild(0).GetComponent<Text>();
            t.text = button.GetComponent<Link_Prefab>().prefab.GetComponent<Price>().energy_cost.ToString();
        }

    }

    protected override void onProcess(int familiesUpdateCount) {
		spawn.spawn_prog += Time.deltaTime;

		if (spawn.spawn_prog >= spawn.wait_time && spawn.nb_waves > 0)
		{
			// For each type of enemy we go thru the creation process.
            for (int j = 0; j < enemies.Length; j++)
            {
				// We create as many of an type of enemy as the correspondant number in nb_enemies.
				for (int i = 1; i <= spawn.nb_enemies[j]; i++)
				{
                    // Creation of an enemy
					GameObject go = UnityEngine.Object.Instantiate<GameObject>(enemies[j]);

                    // we put the enemy in the starting place
                    go.GetComponent<Can_Move>().spawn_point = new Vector3(go.GetComponent<Can_Move>().spawn_point.x, UnityEngine.Random.Range(-1.0f, 1.0f));

                    // we set its target
                    go.GetComponent<Can_Move>().target = new Vector3(go.GetComponent<Can_Move>().target.x, UnityEngine.Random.Range(-1.0f, 1.0f));

                    // we set its speed
                    go.GetComponent<Can_Move>().move_speed = spawn.speed_enemies[j] + UnityEngine.Random.Range(0.0f, 0.2f);

                    // we set its health
                    go.GetComponent<Has_Health>().max_health = spawn.hp_enemies[j];
                    go.GetComponent<Has_Health>().health = spawn.hp_enemies[j];

                    // we set its attack strength 
                    go.GetComponent<Attack_J>().strength = spawn.atk_enemies[j];

                    // we set its size
                    float z = +UnityEngine.Random.Range(-0.1f, 0.1f);
                    go.transform.localScale = new Vector3(spawn.size_enemies[j] + z, spawn.size_enemies[j] + z, 1);

                    GameObjectManager.bind(go);
                }
                // adding spawn.add so that we have more enemies in later waves
                float new_spawn = spawn.nb_enemies[j] + spawn.add;
                _Spawn.First().GetComponent<Spawn>().nb_enemies[j] = new_spawn;
            }

			spawn.spawn_prog = 0;
			spawn.nb_waves--;
		}

		spawn_nb.text = "wave: " + spawn.nb_waves.ToString();
		//spawn_nb.color = new Color(0.0f, 0.57f, 0.06f, 1f);
	}
}