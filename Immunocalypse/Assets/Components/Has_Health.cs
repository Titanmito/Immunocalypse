using UnityEngine;

public class Has_Health : MonoBehaviour {
	// This component is present in entities that have some kind of health bar (Joueur, bacterie and virus now).
	
	// Represents the heath of the entity. The idea is that things happen when it gets to zero (either the entity is destroyed or the player loses the level).
	public int health;
}