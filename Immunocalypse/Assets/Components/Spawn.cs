using UnityEngine;

public class Spawn : MonoBehaviour {
	// Component present in the Level entity that contains information about how many and which enemies will be created in a level. 
	// It also has the factories needed to create entities that aren't fixed in number (so, enemies and towers).

	// The number of waves in a level. 
	public int nb_waves = 10;
	// Each wave will have nb_enemies[i] of each i enemy.
	// i = 0 -> virus
	// i = 1 -> bacterie
	public int[] nb_enemies = new int[2]{4, 1};

	// How many energy the player gains each second.
	public int energy_sec = 1;

	// How many seconds between each wave.
	public float wait_time = 10f;
	// How many seconds since the last wave.
	public float spawn_prog = 8f;
	// How long since the last increase of energy.
	public float energy_prog = 0f;

	// Factories for virus, bacterie and macrophage. New towers and enemies must have their factories added here.
	public GameObject virus_prefab;
	public GameObject bacterie_prefab;
	public GameObject macro_prefab;
	public GameObject lymp_prefab;

	public GameObject anti_prefab;

}