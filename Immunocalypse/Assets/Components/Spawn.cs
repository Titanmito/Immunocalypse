using UnityEngine;

public class Spawn : MonoBehaviour {
	// Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).
	public int nb_waves = 10;
	public int[] nb_enemies = new int[2]{4, 1};
	public int energy_sec = 1;

	public float wait_time = 10f;
	public float spawn_prog = 8f;
	public float energy_prog = 0f;

	public GameObject virus_prefab;
	public GameObject bacterie_prefab;
	public GameObject macro_prefab;

}