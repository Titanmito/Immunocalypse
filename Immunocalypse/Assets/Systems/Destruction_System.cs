using UnityEngine;
using FYFY;

public class Destruction_System : FSystem {
	// This system manages the destruction of enemies and ends the level if the Joueur has negative or equal to zero health.
	// The part controling the end of the level is not yet implemented.

	private Family _AliveGO = FamilyManager.getFamily(new AllOfComponents(typeof(Has_Health), typeof(Can_Move), typeof(Attack_J)));

	protected override void onProcess(int familiesUpdateCount) 
	{
		foreach (GameObject go in _AliveGO)
		{
			Has_Health hh = go.GetComponent<Has_Health>();
			Attack_J aj = go.GetComponent<Attack_J>();

			// it destroys all enemies with negative or equal to zero energy and those who have already attacked (and aren't visible anymore)
			if (hh.health <= 0 || aj.has_attacked)
            {
				GameObjectManager.unbind(go);
				Object.Destroy(go);
			}
		}

		}
}