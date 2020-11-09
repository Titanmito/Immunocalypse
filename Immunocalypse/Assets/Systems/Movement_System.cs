using UnityEngine;
using FYFY;

public class Movement_System : FSystem {
	// This system manages the movement of entities that can move. It also put each new entity that can move in their right spawn place when they are created. 
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

	protected override void onProcess(int familiesUpdateCount)
	{
		foreach (GameObject go in _MovingGO)
		{
			Can_Move cm = go.GetComponent<Can_Move>();
			// If the entity has reached it's taret, we remove it from the visible screen.
			if (Vector3.Distance(cm.target, go.transform.position) < 0.1f)
			{
				go.transform.position = new Vector3(-20.0f, -20.0f);
				cm.arrived = true;
			}
			else
			{
				if (! cm.arrived) {
					go.transform.position = Vector3.MoveTowards(go.transform.position, cm.target, cm.move_speed * Time.deltaTime);
				}
			}
		}
	}

}