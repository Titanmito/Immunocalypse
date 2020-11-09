using UnityEngine;

public class Can_Attack : MonoBehaviour
{
	// This component is present in towers (only Macrophage now) and represents the possibiity of attacking other entities.
	// It's separated from "Attack_J" because towers can attack more than once and have a range (not used now but it's on our plans lol).

	// How much damage a tower can do to an enemy health (bacterie or virus now).
	public int strength = 100;

	// The speed the tower attacks (once every attack_speed seconds) and how many seconds since it last attacked. 
	// They should be the same so that the entity can attack as soon as it's created.
	public float attack_speed = 5f;
	public float last_attack = 5f;

	// The circle radius of the attack area, where the tower can attack (if an enemy is to far from the tower, the tower can't attack it).
	// Not used yet.
	public int range = 1;

}