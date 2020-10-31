using UnityEngine;
using FYFY;
using System;

public class Energy_System : FSystem {
	private Family _Spawn = FamilyManager.getFamily(new AllOfComponents(typeof(Spawn)));
	private Family _Joueur = FamilyManager.getFamily(new AnyOfTags("Player"));
	private Family _Macrophage = FamilyManager.getFamily(new AnyOfTags("Tower_Macro"));

	private Spawn spawn;
	private Price price;
	private Bank bank;

	public Energy_System()
	{
		spawn = _Spawn.First().GetComponent<Spawn>();
		price = _Macrophage.First().GetComponent<Price>();
		bank = _Joueur.First().GetComponent<Bank>();
	}

	public void Macro_Button(int amount)
	{
		if (bank.energy >= price.energy_cost)
		{
			GameObject go = UnityEngine.Object.Instantiate<GameObject>(spawn.macro_prefab);
			GameObjectManager.bind(go);
			bank.energy -= price.energy_cost;
		}
	}

	// Use to process your families.
	protected override void onProcess(int familiesUpdateCount) {
		spawn.energy_prog += Time.deltaTime;

		int secs = (int)Math.Floor(spawn.energy_prog);
		bank.energy += spawn.energy_sec * secs;
		spawn.energy_prog -= secs;

	}
}