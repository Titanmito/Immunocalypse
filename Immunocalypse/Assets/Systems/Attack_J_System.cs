using UnityEngine;
using FYFY;

public class Attack_J_System : FSystem {

	private Family _AttackingGO = FamilyManager.getFamily(new AllOfComponents(typeof(Attack_J), typeof(Can_Move), typeof(Has_Health)));

	private Family _Joueur = FamilyManager.getFamily(new AnyOfTags("Player"));

	private  GameObject joueur;

	//joueur = GameObject.FindWithTag("Player");

	public Attack_J_System()
	{
		joueur = _Joueur.First();
	}

	// Use to process your families.
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