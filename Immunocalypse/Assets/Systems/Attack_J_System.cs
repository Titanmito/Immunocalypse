using UnityEngine;
using UnityEngine.UI;
using FYFY;
using System.Collections;


public class Attack_J_System : FSystem {
	// This system manages the attack of enemies to the player. It actualizes the health of the Joueur entity by going thru each enemy and testing if it can attack or not.
	// For each enemy that can attack it recalculates the health of the Joueur entity and marks the enemy as having attacked. 

	private Family _AttackingGO = FamilyManager.getFamily(new AllOfComponents(typeof(Attack_J), typeof(Can_Move), typeof(Has_Health)));
	private Family _Joueur = FamilyManager.getFamily(new AnyOfTags("Player"), new AllOfComponents(typeof(Has_Health)));
	private Family _HealthBar = FamilyManager.getFamily(new AnyOfTags("Health_Bar"), new AllOfComponents(typeof(Slider)));

	private GameObject joueur;
	private Slider healthBar;

	public Attack_J_System()
	{
        // this.Pause = true;
		//joueur = _Joueur.First();
		//healthBar = _HealthBar.First().GetComponent<Slider>();
		//healthBar.maxValue = joueur.GetComponent<Has_Health>().max_health;
		//healthBar.value = joueur.GetComponent<Has_Health>().health;
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
        _AttackingGO = FamilyManager.getFamily(new AllOfComponents(typeof(Attack_J), typeof(Can_Move), typeof(Has_Health)));
        _Joueur = FamilyManager.getFamily(new AnyOfTags("Player"), new AllOfComponents(typeof(Has_Health)));
        _HealthBar = FamilyManager.getFamily(new AnyOfTags("Health_Bar"), new AllOfComponents(typeof(Slider)));
        joueur = _Joueur.First();
        healthBar = _HealthBar.First().GetComponent<Slider>();
        healthBar.maxValue = joueur.GetComponent<Has_Health>().max_health;
        healthBar.value = joueur.GetComponent<Has_Health>().health;
    }

    protected override void onProcess(int familiesUpdateCount)
	{
		foreach (GameObject go in _AttackingGO)
		{
			Can_Move cm = go.GetComponent<Can_Move>();
			Attack_J aj = go.GetComponent<Attack_J>();

			if (cm.arrived && !aj.has_attacked)
			{
				aj.has_attacked = true;
				joueur.GetComponent<Has_Health>().health -= aj.strength;
			}
		}
		healthBar.value = joueur.GetComponent<Has_Health>().health;

	}
}