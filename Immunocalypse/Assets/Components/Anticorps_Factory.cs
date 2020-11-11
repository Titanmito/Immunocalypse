using UnityEngine;

public class Anticorps_Factory : MonoBehaviour {
	// Component present in the lymphocyte entity that contains information about the creation of anticorps entities.
	// An anticorps is only created if spawn_prog >= wait_time and there is an enemy close to the lymphocyte.

	// How many seconds between each anticorps creation.
	public float wait_time = 5f;
	// How many seconds since the last anticorps creation.
	public float spawn_prog = 5f;

	//Factory for anticorps. 
	public GameObject anti_prefab;
}