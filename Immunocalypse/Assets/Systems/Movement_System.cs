using UnityEngine;
using FYFY;
using FYFY_plugins.TriggerManager;

public class Movement_System : FSystem {
	// This system manages the movement of entities that can move. It also put each new entity that can move in their right spawn place when they are created. 
	
	// Enemies
	private Family _TargetGO = FamilyManager.getFamily(new AllOfComponents(typeof(Can_Move), typeof(Attack_J)), new AnyOfTags("Respawn"));
	// Allies 
	private Family _TargetingGO = FamilyManager.getFamily(new AllOfComponents(typeof(Can_Move), typeof(Can_Attack)), new AnyOfTags("Tower"));

    public static Movement_System instance;
    public bool reinitOnResume = true;
    public bool afterSoftPauseResumed = false;

    // Constructeur
    public Movement_System()
	{
        instance = this;
        // this.Pause = true;
	}

	private void onGOEnter(GameObject go)
	{
		go.transform.position = go.GetComponent<Can_Move>().spawn_point;
	}

    protected override void onPause(int currentFrame)
    {
        if (!reinitOnResume)
            afterSoftPauseResumed = false;
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

        _TargetGO = FamilyManager.getFamily(new AllOfComponents(typeof(Can_Move), typeof(Attack_J)), new AnyOfTags("Respawn"));
        _TargetingGO = FamilyManager.getFamily(new AllOfComponents(typeof(Can_Move), typeof(Can_Attack)), new AnyOfTags("Tower"));

        if (reinitOnResume)
        {
            foreach (GameObject go in _TargetGO)
            {
                onGOEnter(go);
            }
            foreach (GameObject go in _TargetingGO)
            {
                onGOEnter(go);
            }
            _TargetGO.addEntryCallback(onGOEnter);
            _TargetingGO.addEntryCallback(onGOEnter);
        }
        else
            afterSoftPauseResumed = true;
    }

    protected override void onProcess(int familiesUpdateCount)
	{
		// We recalculated where each ally that can move (now, only anticorps) should go.
		foreach (GameObject go in _TargetingGO)
		{
			Can_Move cm = go.GetComponent<Can_Move>();
			float distance = 100000f;

			foreach (GameObject target in _TargetGO)
			{
				if (Vector3.Distance(go.transform.position, target.transform.position) < distance)
				{
					cm.target = new Vector3(target.transform.position.x, target.transform.position.y);
					distance = Vector3.Distance(go.transform.position, target.transform.position);
				}
			}
		}

		// Moving enemies
		foreach (GameObject go in _TargetGO)
		{
			Can_Move cm = go.GetComponent<Can_Move>();

			// If the entity has reached it's target and it's not an ally, we remove it from the visible screen.
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

		// Moving allies
		foreach (GameObject go in _TargetingGO)
		{
			Can_Move cm = go.GetComponent<Can_Move>();
			go.transform.position = Vector3.MoveTowards(go.transform.position, cm.target, cm.move_speed * Time.deltaTime);
		}
	}
}