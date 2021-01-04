using UnityEngine;
using FYFY;
using FYFY_plugins.TriggerManager;

public class Attack_System : FSystem
{
	// This system manages the attack of towers to enemies. 
	// First, it actualizes the time since the last attack of each tower. Then, it goes thru each tower that has an enemy close enough (using Triggered2D),
	// actualizes the health of the enemy and resets the last_attack counter so a tower doesn't attack multiple time in one loop.
	private Family _AllAttackersGO = FamilyManager.getFamily(new AllOfComponents(typeof(Can_Attack)));

	private Family _AnticorpsGO = FamilyManager.getFamily(new AllOfComponents(typeof(Can_Attack), typeof(Has_Health), typeof(Triggered2D)));
	private Family _MacrophageGO = FamilyManager.getFamily(new AllOfComponents(typeof(Can_Attack), typeof(Triggered2D)), new NoneOfComponents(typeof(Has_Health)));

    public Attack_System()
    {
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
        _AllAttackersGO = FamilyManager.getFamily(new AllOfComponents(typeof(Can_Attack)));
        _AnticorpsGO = FamilyManager.getFamily(new AllOfComponents(typeof(Can_Attack), typeof(Has_Health), typeof(Triggered2D)));
        _MacrophageGO = FamilyManager.getFamily(new AllOfComponents(typeof(Can_Attack), typeof(Triggered2D)), new NoneOfComponents(typeof(Has_Health)));
    }

    protected override void onProcess(int familiesUpdateCount)
	{
		foreach (GameObject go in _AllAttackersGO)
        {
			Can_Attack ca = go.GetComponent<Can_Attack>();

			if (ca.last_attack < ca.attack_speed)
			{
				ca.last_attack += Time.deltaTime;
			}
		}

		foreach (GameObject go in _MacrophageGO)
		{

			Triggered2D t2d = go.GetComponent<Triggered2D>();
			Can_Attack ca = go.GetComponent<Can_Attack>();

			// Maybe we could recover only the first GO but to be sure we get an enemy if it is there, we opt to go thru all the GO. 
			foreach (GameObject target in t2d.Targets)
			{
				if (target.gameObject.CompareTag("Respawn") && ca.last_attack >= ca.attack_speed)
				{
					target.GetComponent<Has_Health>().health -= ca.strength;
					ca.last_attack = 0f;
				}
			}
		}

		foreach (GameObject go in _AnticorpsGO)
		{

			Triggered2D t2d = go.GetComponent<Triggered2D>();
			Can_Attack ca = go.GetComponent<Can_Attack>();
			Has_Health hh = go.GetComponent<Has_Health>();

			// Maybe we could recover only the first GO but to be sure we get an enemy if it is there, we opt to go thru all the GO. 
			foreach (GameObject target in t2d.Targets)
			{
				if (target.gameObject.CompareTag("Respawn") && ca.last_attack >= ca.attack_speed)
				{
					target.GetComponent<Has_Health>().health -= ca.strength;
					ca.last_attack = 0f;
				}
				// We want to put the anticorps health to zero if it has attacked so it's destroyed just after.
				hh.health = -1;
			}

		}
	}
}