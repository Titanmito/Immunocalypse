using UnityEngine;

public class Can_Move : MonoBehaviour {
	// Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).

	public float move_speed = 2.5f;
	public Vector3 spawn_point = new Vector3(-8.5f, 0.0f);
	public Vector3 target;

}