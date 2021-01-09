using UnityEngine;

public class Current_Lvl : MonoBehaviour {
	// This component is present only in the Joueur entity and keeps track of the current unlocked level (up to where the player has progressed in terms of levels unblocked). 

	public int current_scene = -1;

	// the number of the last unlocked level
	public int unlocked_scene = 1;

	// this is the number of the last level 
	public int max_scene = 3;
}