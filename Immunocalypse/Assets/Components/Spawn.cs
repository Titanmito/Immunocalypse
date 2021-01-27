using UnityEngine;
using System.Collections.Generic;

public class Spawn : MonoBehaviour {
	// Component present in the Level entity that contains information about how many and which enemies will be created in a level. 
	// This component together with Active_Lvl_Buttons allows us to control what happens in a level (except the way enemies go).
	// It also has the factories needed to create entities (so, enemies, towers and special powers).

	// The number of waves in a level. 
	public int nb_waves = 10;

	// i = 0 -> virus1
	// i = 1 -> virus2
	// i = 2 -> bacterie1
	// i = 3 -> bacterie2
	// Each wave will have nb_enemies[i] of each i enemy.
	public float[] nb_enemies = new float[4]{2, 3, 1, 1};

	// How much HP each type of enemy has in this level.
	public int[] hp_enemies = new int[4] {20, 30, 100, 80};

	// How much damage each type of enemy has in this level.
	public int[] atk_enemies = new int[4] {4, 5, 15, 13};

	// How much speed each type of enemy has in this level.
	public float[] speed_enemies = new float[4] {1.7f, 1.5f, 1, 1.2f};

	// The size of each type of enemy.
	public float[] size_enemies = new float[4] {0.25f, 0.3f, 0.4f, 0.5f};

	// How many energy the player gains each second.
	public int energy_sec = 1;

	// How many energy the player gains for each enemy killed.
	public int energy_enemy = 5;

	// How many energy the player start with.
	public int energy_start = 200;

	// How many score points the player gains for each kill.
	public int score_enemy = 10;

	// How many seconds between each wave.
	public float wait_time = 10f;
	// How many seconds since the last wave.
	public float spawn_prog = 8f;
	// How long since the last increase of energy.
	public float energy_prog = 0f;

	// we add a quantity to each enemy spawn (so that we have more enemies at the end of a level).
	public float[] add_enemies = new float[4] {0.2f, 0.2f, 0.2f, 0.2f};

	// each level may have different spawn and target points.
	public List<Vector3> lvl_spawn_point = new List<Vector3>() {new Vector3(-10.5f, 0.5f), new Vector3(-10.5f, 0.5f)};


	public List<Vector3> lvl_checkpoints = new List<Vector3>();
	public Vector3 lvl_target_final = new Vector3(9.0f, -0.5f);

	// Factories for enemies, towers and special powers. New towers and enemies must have their factories added here.
	// Enemies
	public GameObject virus1_prefab; 
	public GameObject virus2_prefab;
	public GameObject bacterie1_prefab;
	public GameObject bacterie2_prefab;
	// Towers (do not forget to add a factory for their shadow too!)
	public GameObject macro_prefab;
	public GameObject macro_shadow_prefab;
	public GameObject lymp1_prefab;
	public GameObject lymp1_shadow_prefab;
	public GameObject lymp2_prefab;
	public GameObject lymp2_shadow_prefab;
	// Special powers
	public GameObject anti_prefab;
	public GameObject vaci_prefab;



}