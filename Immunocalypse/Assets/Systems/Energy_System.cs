using UnityEngine;
using UnityEngine.UI;
using FYFY;
using System;
using System.Collections;

public class Energy_System : FSystem {
	// This system manages the energy of the Joueur. For that it contabilizes and actualizes the energy each second and also each time a tower is bought or a special power is used.
	// We use buttons to implement tower and special power buy.
	// Special powers aren't implemented yet.

	private Family _Spawn = FamilyManager.getFamily(new AllOfComponents(typeof(Spawn)));
	private Family _Joueur = FamilyManager.getFamily(new AnyOfTags("Player"));
	private Family _Macrophage = FamilyManager.getFamily(new AnyOfTags("Tower_Macro"));
	private Family _Energy_nb = FamilyManager.getFamily(new AnyOfTags("Energy"));
	private Family _Inactive = FamilyManager.getFamily(new NoneOfProperties(PropertyMatcher.PROPERTY.ACTIVE_IN_HIERARCHY, PropertyMatcher.PROPERTY.HAS_PARENT));

	private Spawn spawn;
	private Price price;
	private Bank bank;
	private Text energy_nb;

	public Energy_System()
	{
		spawn = _Spawn.First().GetComponent<Spawn>();
		price = _Macrophage.First().GetComponent<Price>();
		bank = _Joueur.First().GetComponent<Bank>();
		energy_nb = _Energy_nb.First().GetComponent<Text>();

	}

	// Used to control the button for the buying of Macrophage towers.
	// The idea is of having one function per button.
	// A player can only buy a tower if the bank isn't used aka if there aren't any other tower waiting to be placed.
	// The tower is desactivated when created so that it isn't taken in consideration anywhere else until placed.
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

			// Actualizes the energy display to the player.
			energy_nb.text = "energy: " + bank.energy.ToString();
		}
	}

	protected override void onProcess(int familiesUpdateCount) {
		spawn.energy_prog += Time.deltaTime;

		int secs = (int)Math.Floor(spawn.energy_prog);
		bank.energy += spawn.energy_sec * secs;
		spawn.energy_prog -= secs;

		// This part places the last tower bought and not placed, when the player clicks on the screen (in the position the player clicks).
		// It reactivates the tower so that it's taken in consideration elsewhere. 
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
		// Actualizes the energy display to the player.
		energy_nb.text = "energy: " + bank.energy.ToString();
	}
}