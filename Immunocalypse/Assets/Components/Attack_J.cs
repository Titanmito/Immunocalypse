using UnityEngine;

public class Attack_J : MonoBehaviour {
	// This component is present in entities that can hurt the player health (virus and bacterie now).

	// How much damage an entity does to a player's health.
	public int strength;

	// If the entity has already done it's damage to the player or not. It's used as a sign that the entity has already done it's job and can be deleted. 
	public bool has_attacked = false;
}