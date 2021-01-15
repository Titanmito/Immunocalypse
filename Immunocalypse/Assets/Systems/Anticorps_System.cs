using UnityEngine;
using FYFY;
using FYFY_plugins.TriggerManager;
using System;

public class Anticorps_System : FSystem {
	// This system manages the creation of new anticorps.
	// There are two types of anticorps:
	// type 1 -> attacks virus
	// type 2 -> attacks bacterie

	private Family _Lymphocyte = FamilyManager.getFamily(new AllOfComponents(typeof(Anticorps_Factory), typeof(Lymphocyte)));
	private Family _Anti_Creators = FamilyManager.getFamily(new AllOfComponents(typeof(Anticorps_Factory), typeof(Triggered2D), typeof(Lymphocyte)));

	public Anticorps_System()
    {
        //anti = _Lymphocyte.First().GetComponent<Anticorps_Factory>();
        // this.Pause = true;
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
        _Lymphocyte = FamilyManager.getFamily(new AllOfComponents(typeof(Anticorps_Factory), typeof(Lymphocyte)));
		_Anti_Creators = FamilyManager.getFamily(new AllOfComponents(typeof(Anticorps_Factory), typeof(Triggered2D), typeof(Lymphocyte)));
    }

    protected override void onProcess(int familiesUpdateCount)
	{
		// we prepare the next creation of an anticorps by each lymphocyte tower
		foreach (GameObject go in _Lymphocyte)
		{
			Anticorps_Factory anti_f = go.GetComponent<Anticorps_Factory>();

			if (anti_f.spawn_prog < anti_f.wait_time)
			{
				anti_f.spawn_prog += Time.deltaTime;
			}
		}

		// we create anticorps
		foreach (GameObject go in _Anti_Creators)
		{
			Triggered2D t2d = go.GetComponent<Triggered2D>();
			Anticorps_Factory anti_f = go.GetComponent<Anticorps_Factory>();
			int type = go.GetComponent<Lymphocyte>().type;

			// Maybe we could recover only the first GO but to be sure we get an enemy if it is there, we opt to go thru all the GO. 
			foreach (GameObject target in t2d.Targets)
			{
				Has_Health h_e = target.GetComponent<Has_Health>();
				if (h_e != null && target.gameObject.CompareTag("Respawn") && anti_f.spawn_prog >= anti_f.wait_time)
				{
					if (type == 1)
					{
						Virus vv = target.GetComponent<Virus>();
						if (vv != null)
						{
							GameObject anticorps = UnityEngine.Object.Instantiate<GameObject>(anti_f.anti_prefab);

							Color color = target.GetComponent<SpriteRenderer>().color;
							anticorps.GetComponent<SpriteRenderer>().color = color;
								
							GameObjectManager.bind(anticorps);

							anti_f.spawn_prog = 0f;
							anticorps.GetComponent<Can_Move>().spawn_point = new Vector3(go.transform.position.x, go.transform.position.y);
							anticorps.GetComponent<Can_Move>().target = new Vector3(target.transform.position.x, target.transform.position.y);

							// changes the color of the tower to indicate it just created an anticorps
							SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
							sr.color = new Color(1, 0.4f, 1, 1);
						}
					}
					if (type == 2)
                    {
						Bacterie bb = target.GetComponent<Bacterie>();
						if (bb != null)
						{
							GameObject anticorps = UnityEngine.Object.Instantiate<GameObject>(anti_f.anti_prefab);

							Color color = target.GetComponent<SpriteRenderer>().color;
							anticorps.GetComponent<SpriteRenderer>().color = color;

							GameObjectManager.bind(anticorps);

							anti_f.spawn_prog = 0f;
							anticorps.GetComponent<Can_Move>().spawn_point = new Vector3(go.transform.position.x, go.transform.position.y);
							anticorps.GetComponent<Can_Move>().target = new Vector3(target.transform.position.x, target.transform.position.y);
							// changes the color of the tower to indicate it just created an anticorps
							SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
							sr.color = new Color(1, 0.4f, 1, 1);
						}
					}
				}
			}
		}
	}
}