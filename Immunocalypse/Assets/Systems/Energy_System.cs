using UnityEngine;
using FYFY;
using System;

public class Energy_System : FSystem {
	private Family _Spawn = FamilyManager.getFamily(new AllOfComponents(typeof(Spawn)));
	private Family _Joueur = FamilyManager.getFamily(new AnyOfTags("Player"));
	private Family _Macrophage = FamilyManager.getFamily(new AnyOfTags("Tower_Macro"));
	private Family _Inactive = FamilyManager.getFamily(new NoneOfProperties(PropertyMatcher.PROPERTY.ACTIVE_IN_HIERARCHY, PropertyMatcher.PROPERTY.HAS_PARENT));

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
		if (bank.energy >= price.energy_cost && !bank.used)
		{
			GameObject go = UnityEngine.Object.Instantiate<GameObject>(spawn.macro_prefab);
			go.transform.position = new Vector3(20.0f, 20.0f);
			GameObjectManager.bind(go);
			go.SetActive(false);

			bank.energy -= price.energy_cost;
			bank.used = true;
		}
	}

	// Use to process your families.
	protected override void onProcess(int familiesUpdateCount) {
		spawn.energy_prog += Time.deltaTime;

		int secs = (int)Math.Floor(spawn.energy_prog);
		bank.energy += spawn.energy_sec * secs;
		spawn.energy_prog -= secs;

		if (bank.used)
        {
			if (Input.GetMouseButton(0) && _Inactive.First())
            {
				Vector3 mousePos = Input.mousePosition;
				mousePos.z = Camera.main.nearClipPlane;
				Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePos);

				GameObject go = _Inactive.First();
				go.SetActive(true);
				go.transform.position = worldPosition;

				bank.used = false;
			}


        }

	}
}