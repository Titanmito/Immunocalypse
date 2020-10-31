using UnityEngine;
using FYFY;

public class Spawn_System : FSystem {
	private Family _Spawn = FamilyManager.getFamily(new AllOfComponents(typeof(Spawn)));

	private Spawn spawn;

	public Spawn_System()
	{
		spawn = _Spawn.First().GetComponent<Spawn>();
	}

	// Use to process your families.
	protected override void onProcess(int familiesUpdateCount) {
		spawn.spawn_prog += Time.deltaTime;

		if (spawn.spawn_prog >= spawn.wait_time && spawn.nb_waves > 0)
		{
			for (int i = 0; i < spawn.nb_enemies[0]; i++)
            {
				GameObject go = Object.Instantiate<GameObject>(spawn.virus_prefab);
				GameObjectManager.bind(go);

				go.GetComponent<Can_Move>().spawn_point = new Vector3(go.GetComponent<Can_Move>().spawn_point.x, Random.Range(-1.0f, 1.0f));
			}

			for (int i = 0; i < spawn.nb_enemies[1]; i++)
			{
				GameObject go = Object.Instantiate<GameObject>(spawn.bacterie_prefab);
				GameObjectManager.bind(go);

				go.GetComponent<Can_Move>().spawn_point = new Vector3(go.GetComponent<Can_Move>().spawn_point.x, Random.Range(-1.0f, 1.0f));
			}

			spawn.spawn_prog = 0;
			spawn.nb_waves--;
		}

	}
}