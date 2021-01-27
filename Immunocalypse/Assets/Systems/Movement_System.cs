using UnityEngine;
using FYFY;
using FYFY_plugins.TriggerManager;

public class Movement_System : FSystem {
	// This system manages the movement of entities that can move. It also puts each new entity that can move in their right spawn place when they are created. 
	
	// Enemies
	private Family _TargetVirusGO = FamilyManager.getFamily(new AllOfComponents(typeof(Can_Move), typeof(Attack_J), typeof(Virus)), new AnyOfTags("Respawn"));
	private Family _TargetBacterieGO = FamilyManager.getFamily(new AllOfComponents(typeof(Can_Move), typeof(Attack_J), typeof(Bacterie)), new AnyOfTags("Respawn"));
	// Allies 
	private Family _TargetingGO = FamilyManager.getFamily(new AllOfComponents(typeof(Can_Move), typeof(Can_Attack), typeof(Anticorps)), new AnyOfTags("Tower"));

    // Constructeur
    public Movement_System()
	{
        // this.Pause = true;
	}

	private void onGOEnter(GameObject go)
	{
		go.transform.position = go.GetComponent<Can_Move>().spawn_point;
	}

	private void onGOExit(int id)
    {
		_TargetingGO = FamilyManager.getFamily(new AllOfComponents(typeof(Can_Move), typeof(Can_Attack), typeof(Anticorps)), new AnyOfTags("Tower"));
	}

    protected override void onPause(int currentFrame)
    {
        // Debug.Log("System " + this.GetType().Name + " go on pause");
    }

    protected override void onResume(int currentFrame)
    {
        // Debug.Log("System " + this.GetType().Name + " go on resume ; " + currentFrame.ToString());
        if (currentFrame == 1)
        {
            this.Pause = true;
            return;
        }

        _TargetVirusGO = FamilyManager.getFamily(new AllOfComponents(typeof(Can_Move), typeof(Attack_J), typeof(Virus)), new AnyOfTags("Respawn"));
		_TargetBacterieGO = FamilyManager.getFamily(new AllOfComponents(typeof(Can_Move), typeof(Attack_J), typeof(Bacterie)), new AnyOfTags("Respawn"));
		_TargetingGO = FamilyManager.getFamily(new AllOfComponents(typeof(Can_Move), typeof(Can_Attack), typeof(Anticorps)), new AnyOfTags("Tower"));

		foreach (GameObject go in _TargetVirusGO)
		{
			onGOEnter(go);
		}
		foreach (GameObject go in _TargetBacterieGO)
		{
			onGOEnter(go);
		}
		foreach (GameObject go in _TargetingGO)
        {
			onGOEnter(go);
			onGOExit(1);
		}
		_TargetVirusGO.addEntryCallback(onGOEnter);
		_TargetBacterieGO.addEntryCallback(onGOEnter);
		_TargetingGO.addEntryCallback(onGOEnter);
		_TargetingGO.addExitCallback(onGOExit);
	}

    protected override void onProcess(int familiesUpdateCount)
	{
		// We recalculated where each ally that can move (now, only anticorps) should go.
		foreach (GameObject go in _TargetingGO)
		{
			Can_Move cm = go.GetComponent<Can_Move>();
			int type = go.GetComponent<Anticorps>().type;
			float distance = 300000f;
			// each type of anticorps only goes after one type of enemy
			if (type == 1)
			{
				foreach (GameObject target in _TargetVirusGO)
				{
					if (Vector3.Distance(go.transform.position, target.transform.position) < distance)
					{
						cm.target_final = new Vector3(target.transform.position.x, target.transform.position.y);
						distance = Vector3.Distance(go.transform.position, target.transform.position);
					}
				}
			}
			if (type == 2)
            {
				foreach (GameObject target in _TargetBacterieGO)
				{
					if (Vector3.Distance(go.transform.position, target.transform.position) < distance)
					{
						cm.target_final = new Vector3(target.transform.position.x, target.transform.position.y);
						distance = Vector3.Distance(go.transform.position, target.transform.position);
					}
				}
			}
		}

		// Moving enemies
		foreach (GameObject go in _TargetVirusGO)
		{
			Can_Move cm = go.GetComponent<Can_Move>();
			cm.spawn_point = go.transform.position;

			// If the entity has reached it's target and it's not an ally, we remove it from the visible screen.
			if (Vector3.Distance(cm.target_final, go.transform.position) < 0.1f)
			{
				go.transform.position = new Vector3(-20.0f, -20.0f);
				cm.arrived = true;
			}
			// If the entity has reached one of their checkpoints and it's not an ally.
			if (cm.checkpoints.Count > 0)
			{
				if (Vector3.Distance(cm.checkpoints[0], go.transform.position) < 0.1f)
				{
					cm.checkpoints.RemoveAt(0);
				}
				else
				{
					if (!cm.arrived)
					{
						go.transform.position = Vector3.MoveTowards(go.transform.position, cm.checkpoints[0], cm.move_speed * Time.deltaTime);
					}
				}
            }
            else
            {
				if (!cm.arrived)
				{
					go.transform.position = Vector3.MoveTowards(go.transform.position, cm.target_final, cm.move_speed * Time.deltaTime);
				}
			}
		}
		foreach (GameObject go in _TargetBacterieGO)
		{
			Can_Move cm = go.GetComponent<Can_Move>();
			cm.spawn_point = go.transform.position;

			// If the entity has reached it's target and it's not an ally, we remove it from the visible screen.
			if (Vector3.Distance(cm.target_final, go.transform.position) < 0.1f)
			{
				go.transform.position = new Vector3(-20.0f, -20.0f);
				cm.arrived = true;
			}
			// If the entity has reached one of their checkpoints and it's not an ally.
			if (cm.checkpoints.Count > 0)
			{
				if (Vector3.Distance(cm.checkpoints[0], go.transform.position) < 0.1f)
				{
					cm.checkpoints.RemoveAt(0);
				}
				else
				{
					if (!cm.arrived)
					{
						go.transform.position = Vector3.MoveTowards(go.transform.position, cm.checkpoints[0], cm.move_speed * Time.deltaTime);
					}
				}
			}
			else
			{
				if (!cm.arrived)
				{
					go.transform.position = Vector3.MoveTowards(go.transform.position, cm.target_final, cm.move_speed * Time.deltaTime);
				}
			}
		}

		// Moving allies
		foreach (GameObject go in _TargetingGO)
		{
			Can_Move cm = go.GetComponent<Can_Move>();
			go.transform.position = Vector3.MoveTowards(go.transform.position, cm.target_final, cm.move_speed * Time.deltaTime);
		}
	}
}