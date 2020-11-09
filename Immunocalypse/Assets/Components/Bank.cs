using UnityEngine;

public class Bank : MonoBehaviour {
	// This component is present only in the Joueur entity and contabilizes it's energy, the currency used to buy new towers. 

	// The amount of energy the player has. It increases every second by a small amount (see Energy_System).
	public int energy;

	// If the player has bought something or not. It's used to indicate there's a tower the player has bought but not yet placed.
	// the idea is to keep the player from buying another tower until they place the current one. 
	public bool used = false;
}