using UnityEngine;
using FYFY;

public class Attack_J_System : FSystem {
	// This system manages the attack of enemies to the player. It actualizes the health of the Joueur entity by going thru each enemy and testing if it can attack or not.
	// For each enemy that can attack it recalculates the health of the Joueur entity and marks the enemy as having attacked. 

	private Family _AttackingGO = FamilyManager.getFamily(new AllOfComponents(typeof(Attack_J), typeof(Can_Move), typeof(Has_Health)));

	private Family _Joueur = FamilyManager.getFamily(new AnyOfTags("Player"));

	private  GameObject joueur;

	//joueur = GameObject.FindWithTag("Player");

	public Attack_J_System()
	{
		joueur = _Joueur.First();
	}

	protected override void onProcess(int familiesUpdateCount)
	{
		foreach (GameObject go in _AttackingGO)
		{
			Can_Move cm = go.GetComponent<Can_Move>();
			Attack_J aj = go.GetComponent<Attack_J>();

			if (cm.arrived && !aj.has_attacked)
			{
				aj.has_attacked = true;
				joueur.GetComponent<Has_Health>().health -= aj.strength;
			}

		}
	}
}