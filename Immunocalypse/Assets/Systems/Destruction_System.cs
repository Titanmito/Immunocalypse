using UnityEngine;
using FYFY;

public class Destruction_System : FSystem {
	// This system manages the destruction of enemies and ends the level if the Joueur has negative or equal to zero health.
	// The part controling the end of the level is not yet implemented.

	private Family _EnemiesAliveGO = FamilyManager.getFamily(new AnyOfTags("Respawn"));
	private Family _AlliesAliveGO = FamilyManager.getFamily(new AnyOfTags("Tower"), new AllOfComponents(typeof(Has_Health)), new AllOfComponents(typeof(Lifespan)));

	protected override void onProcess(int familiesUpdateCount)
	{
		// destroy enemies that should be destroyed.
		foreach (GameObject go in _EnemiesAliveGO)
		{
			Has_Health hh = go.GetComponent<Has_Health>();
			Attack_J aj = go.GetComponent<Attack_J>();

			// it destroys all enemies with negative or equal to zero energy and those who have already attacked (and aren't visible anymore).
			if (hh.health <= 0 || aj.has_attacked)
			{
				GameObjectManager.unbind(go);
				Object.Destroy(go);
			}
		}

		// destroy allies that should be destroyed.
		foreach (GameObject go in _AlliesAliveGO)
		{
			Has_Health hh = go.GetComponent<Has_Health>();
			Lifespan ls = go.GetComponent<Lifespan>();

			if (hh.health <= 0 || ls.lifespan <= 0)
            {
				GameObjectManager.unbind(go);
				Object.Destroy(go);
			}
			else
            {
				ls.lifespan -= Time.deltaTime;
			}

		}

	}
}