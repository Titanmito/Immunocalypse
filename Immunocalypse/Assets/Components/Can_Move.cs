using UnityEngine;
using System.Collections.Generic;

public class Can_Move : MonoBehaviour {
	// This component is present in entities that aren't fixed in place in the scene and move around somehow (bacterie and virus now). 

	// The speed of the entity. 
	public float move_speed = 2.5f;
	// Where the entity show up when created.
	public Vector3 spawn_point = new Vector3(-8.5f, 0.0f);

	// Where the entity is trying to go.
	public Vector3 target_final;
	public List<Vector3> checkpoints = new List<Vector3>();

	// Whether the entity got to their objective or not. Used to determine if it's time for an entity to attack the player (it only attacks if it got to their objective).
	public bool arrived = false;

}