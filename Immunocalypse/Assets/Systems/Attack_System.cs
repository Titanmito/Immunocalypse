using UnityEngine;
using FYFY;
using FYFY_plugins.TriggerManager;

public class Attack_System : FSystem
{
	private Family _AttackersGO = FamilyManager.getFamily(new AllOfComponents(typeof(Can_Attack)));
	private Family _AttackingGO = FamilyManager.getFamily(new AllOfComponents(typeof(Can_Attack), typeof(Triggered2D)));

	// Use to process your families.
	protected override void onProcess(int familiesUpdateCount)
	{
		foreach (GameObject go in _AttackersGO)
        {
			Can_Attack ca = go.GetComponent<Can_Attack>();

			if (ca.last_attack < ca.attack_speed)
			{
				ca.last_attack += Time.deltaTime;
			}
		}

		foreach (GameObject go in _AttackingGO)
		{

			Triggered2D t2d = go.GetComponent<Triggered2D>();
			Can_Attack ca = go.GetComponent<Can_Attack>();

			foreach (GameObject target in t2d.Targets)
			{
				if (target.gameObject.CompareTag("Respawn") && ca.last_attack >= ca.attack_speed)
				{
					target.GetComponent<Has_Health>().health -= ca.strength;
					ca.last_attack = 0f;
				}
			}

		}
	}
}