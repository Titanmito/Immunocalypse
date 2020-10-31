using UnityEngine;

public class Can_Attack : MonoBehaviour
{
	// Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).
	public int strength = 100;

	//They should be the same so that the entity can attack as soon as it's created
	public float attack_speed = 5f;
	public float last_attack = 5f;

	public int range = 1;

}