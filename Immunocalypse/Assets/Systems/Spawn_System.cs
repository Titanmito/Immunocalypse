using UnityEngine;
using FYFY;
using UnityEngine.UI;
using System.Reflection;
using System;


public class Spawn_System : FSystem {
	// This system manages the creation of new enemies.

	private Family _Spawn = FamilyManager.getFamily(new AllOfComponents(typeof(Spawn)));
	private Family _Buttons = FamilyManager.getFamily(new AnyOfTags("Button"));
	private Spawn spawn;

	public Spawn_System()
	{
		spawn = _Spawn.First().GetComponent<Spawn>();

		// Puts the price of each tower and special power in their respective buttons.
		// buttons must be named as *same start as the name of the respective prefab*_button.
		foreach (GameObject button in _Buttons)
        {
			string name = button.name;
			int index = name.IndexOf("_");
			name = name.Substring(0, index + 1) + "prefab";

			GameObject fab = (GameObject)typeof(Spawn).GetField(name).GetValue(spawn);
			Text t = button.transform.GetChild(0).GetComponent<Text>();
			t.text = fab.GetComponent<Price>().energy_cost.ToString();
		}
	}

	// Use to process your families.
	protected override void onProcess(int familiesUpdateCount) {
		spawn.spawn_prog += Time.deltaTime;

		if (spawn.spawn_prog >= spawn.wait_time && spawn.nb_waves > 0)
		{
			// nb_enemies[0] -> virus
			// nb_enemies[1] -> bacterie
			for (int i = 0; i < spawn.nb_enemies[0]; i++)
            {
				GameObject go = UnityEngine.Object.Instantiate<GameObject>(spawn.virus_prefab);
				GameObjectManager.bind(go);

				go.GetComponent<Can_Move>().spawn_point = new Vector3(go.GetComponent<Can_Move>().spawn_point.x, UnityEngine.Random.Range(-1.0f, 1.0f));
			}

			for (int i = 0; i < spawn.nb_enemies[1]; i++)
			{
				GameObject go = UnityEngine.Object.Instantiate<GameObject>(spawn.bacterie_prefab);
				GameObjectManager.bind(go);

				go.GetComponent<Can_Move>().spawn_point = new Vector3(go.GetComponent<Can_Move>().spawn_point.x, UnityEngine.Random.Range(-1.0f, 1.0f));
			}

			spawn.spawn_prog = 0;
			spawn.nb_waves--;
		}

	}
}