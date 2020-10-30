using UnityEngine;
using FYFY;

public class Movement_System : FSystem {

	private Family _MovingGO = FamilyManager.getFamily(new AllOfComponents(typeof(Can_Move)));

	// Constructeur
	public Movement_System()
	{
		foreach (GameObject go in _MovingGO)
			onGOEnter(go);

		_MovingGO.addEntryCallback(onGOEnter);
	}

	private void onGOEnter(GameObject go)
	{
		go.transform.position = go.GetComponent<Can_Move>().spawn_point;
	}

	// Use to process your families.
	protected override void onProcess(int familiesUpdateCount)
	{
		foreach (GameObject go in _MovingGO)
		{
			Can_Move cm = go.GetComponent<Can_Move>();

			if (cm.target.Equals(go.transform.position))
			{
				go.transform.position = new Vector3(-20.0f, -20.0f);
				GameObjectManager.unbind(go);
				//Object.Destroy(go);

			}
			else
			{
				go.transform.position = Vector3.MoveTowards(go.transform.position, cm.target, cm.move_speed * Time.deltaTime);
			}
		}
	}

}