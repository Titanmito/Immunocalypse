using UnityEngine;
using FYFY;

public class Destruction_System : FSystem {

	private Family _AliveGO = FamilyManager.getFamily(new AllOfComponents(typeof(Has_Health), typeof(Can_Move), typeof(Attack_J)));

	// Use to process your families.
	protected override void onProcess(int familiesUpdateCount) 
	{
		foreach (GameObject go in _AliveGO)
		{
			Has_Health hh = go.GetComponent<Has_Health>();
			Attack_J aj = go.GetComponent<Attack_J>();

			if (hh.health <= 0 || aj.has_attacked)
            {
				GameObjectManager.unbind(go);
				Object.Destroy(go);
			}
		}

		}
}