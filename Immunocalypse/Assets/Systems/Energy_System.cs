using UnityEngine;
using UnityEngine.UI;
using FYFY;
using System;
using System.Collections;

using System.Diagnostics;

public class Energy_System : FSystem {
	// This system manages the energy of the Joueur. For that it contabilizes and actualizes the energy each second and also each time a tower is bought or a special power is used.
	// We use buttons to implement tower and special power buy.
	// Special Powers are implemented here
	// In the end we keep the buttons that control which enemy is targeted by a vacine here because it's easier as all the Families we need are already in here.

	private Family _Spawn = FamilyManager.getFamily(new AllOfComponents(typeof(Spawn), typeof(Active_Lvl_Buttons)));
	private Family _Joueur = FamilyManager.getFamily(new AnyOfTags("Player"), new AllOfComponents(typeof(Has_Health), typeof(Bank)));
	private Family _Energy_nb = FamilyManager.getFamily(new AnyOfTags("Energy"), new AllOfComponents(typeof(Text)));
	private Family _Inactive_tower = FamilyManager.getFamily(new NoneOfProperties(PropertyMatcher.PROPERTY.ACTIVE_IN_HIERARCHY, PropertyMatcher.PROPERTY.HAS_PARENT), 
		new AnyOfTags("Tower"));
	private Family _Buttons = FamilyManager.getFamily(new AnyOfTags("Button"), new AllOfComponents(typeof(Button)), new NoneOfLayers(8, 9), 
		new AllOfProperties(PropertyMatcher.PROPERTY.ACTIVE_IN_HIERARCHY));

	private Family _Antibiotique = FamilyManager.getFamily(new AllOfComponents(typeof(Efficiency)));

	private Family _Vaccin = FamilyManager.getFamily(new AllOfComponents(typeof(Vaccin)));

	private Spawn spawn;
	private Bank bank;
	private Text energy_nb;

	private Price macro_price;
	private Price lymp_price;
	private Price anti_price;
	private Price vaci_price;

	private Efficiency anti_eff;

    public static Energy_System instance;

    public Energy_System()
	{
        instance = this;
        // this.Pause = true;
    }

    protected override void onPause(int currentFrame)
    {
		//Debug.Log("System " + this.GetType().Name + " go on pause");
		//return;
	}

    protected override void onResume(int currentFrame)
    {
		/*
		StackTrace st = new StackTrace();
		StackFrame[] sf = st.GetFrames();
        foreach (StackFrame s in sf)
        {
			UnityEngine.Debug.Log(s.GetMethod().Name);
        }
		*/

        // Debug.Log("System " + this.GetType().Name + " go on resume ; " + currentFrame.ToString());
        if (currentFrame == 1)
        {
            this.Pause = true;
            return;
        }

		//UnityEngine.Debug.Log(this.Pause);

        _Spawn = FamilyManager.getFamily(new AllOfComponents(typeof(Spawn)));
        _Joueur = FamilyManager.getFamily(new AnyOfTags("Player"), new AllOfComponents(typeof(Has_Health), typeof(Bank)));
        _Energy_nb = FamilyManager.getFamily(new AnyOfTags("Energy"), new AllOfComponents(typeof(Text)));
        _Inactive_tower = FamilyManager.getFamily(new NoneOfProperties(PropertyMatcher.PROPERTY.ACTIVE_IN_HIERARCHY, PropertyMatcher.PROPERTY.HAS_PARENT),
            new AnyOfTags("Tower"));
		_Buttons = FamilyManager.getFamily(new AnyOfTags("Button"), new AllOfComponents(typeof(Button)), new NoneOfLayers(8, 9),
		new AllOfProperties(PropertyMatcher.PROPERTY.ACTIVE_IN_HIERARCHY));

	_Antibiotique = FamilyManager.getFamily(new AllOfComponents(typeof(Efficiency)));

        spawn = _Spawn.First().GetComponent<Spawn>();
		bank = _Joueur.First().GetComponent<Bank>();
		energy_nb = _Energy_nb.First().GetComponent<Text>();

		macro_price = spawn.macro_prefab.GetComponent<Price>();
        lymp_price = spawn.lymp_prefab.GetComponent<Price>();
        anti_price = spawn.anti_prefab.GetComponent<Price>();
		vaci_price = spawn.vaci_prefab.GetComponent<Price>(); 

		anti_eff = _Antibiotique.First().GetComponent<Efficiency>();
	}

    // Used to control the button for buying of Macrophage towers.
    // The idea is of having one function per button.
    // A player can only buy a tower if the bank isn't used aka if there aren't any other tower waiting to be placed.
    // The tower is desactivated when created so that it isn't taken in consideration anywhere else until placed.
    public void Macro_Button(int amount = 1)
    {
		this.Des_Cancel_Button();
		if (bank.energy >= macro_price.energy_cost && !bank.used)
		{
			GameObject go = UnityEngine.Object.Instantiate<GameObject>(spawn.macro_prefab);
			go.transform.position = new Vector3(20.0f, 20.0f);
			GameObjectManager.bind(go);
			go.SetActive(false);

			bank.energy -= macro_price.energy_cost;
			bank.used = true;

			// Actualizes the energy display to the player.
			energy_nb.text = "energy: " + bank.energy.ToString();
		}
	}

	// Used to control the button for buying of Lymphocyte towers.
	public void Lymp_Button(int amount = 1)
	{
		this.Des_Cancel_Button();
		if (bank.energy >= lymp_price.energy_cost && !bank.used)
		{
			GameObject go = UnityEngine.Object.Instantiate<GameObject>(spawn.lymp_prefab);
			go.transform.position = new Vector3(20.0f, 20.0f);
			GameObjectManager.bind(go);
			go.SetActive(false);

			bank.energy -= lymp_price.energy_cost;
			bank.used = true;

			// Actualizes the energy display to the player.
			energy_nb.text = "energy: " + bank.energy.ToString();
		}
	}

	// Used to control the button for the antibiotique special power.
	public void Anti_Button(int amount = 1)
	{
		this.Des_Cancel_Button();

		if (bank.energy >= anti_price.energy_cost && !bank.used)
		{
			bank.energy -= anti_price.energy_cost;
			Family _Respawn = FamilyManager.getFamily(new AnyOfTags("Respawn"), new AllOfComponents(typeof(Has_Health), typeof(Bacterie)));
			float pourcentage = anti_eff.nb_used / 5.0f;
			
			//Debug.Log("pourcentage = " + pourcentage);

			foreach (GameObject go in _Respawn)
			{
				float nb = UnityEngine.Random.Range(0.0f, 1.0f);
				
				//Debug.Log("nb = " + nb);

				if (nb <= 1 - pourcentage)
				{
					go.GetComponent<Has_Health>().health = -1;
				}
			}
			anti_eff.nb_used += 1;

			// Actualizes the energy display to the player.
			energy_nb.text = "energy: " + bank.energy.ToString();
		}
	}

	// Controls the vaci_button
	public void Vaci_Button(int amount = 1)
	{
		if (bank.energy >= vaci_price.energy_cost && !bank.used)
		{
			Family _Des_buttons = FamilyManager.getFamily(new AnyOfTags("Button"), new AllOfComponents(typeof(Button), typeof(Lvl_Buttons)), new AnyOfLayers(8));
			Active_Lvl_Buttons ActButtons = _Spawn.First().GetComponent<Active_Lvl_Buttons>();
			foreach (GameObject go in _Des_buttons)
            {

				Lvl_Buttons lb = go.GetComponent<Lvl_Buttons>();
				switch (lb.button_nb)
				{
					case 31:
						go.SetActive(ActButtons.des_virus1);
						break;
					case 32:
						go.SetActive(ActButtons.des_virus2);
						break;
					case 33:
						go.SetActive(ActButtons.des_bacterie1);
						break;
					case 34:
						go.SetActive(ActButtons.des_bacterie2);
						break;
					case 40:
						go.SetActive(ActButtons.des_cancel);
						break;
				}
			}
		}
	}

	// I couldn't make the wrapper for this in Unity pass a number to the function (I think it's linked to the fack we're not really using the wrapper and doing the bind by hand? maybe?)
	// so I ended up creating a function that set the correct case at nb_enemies in Spawn to zero and 4 functions that call it with the correct argument.
	
	private void Destruction(int i = 1)
    {
		if (bank.energy >= vaci_price.energy_cost && !bank.used)
		{
			bank.energy -= vaci_price.energy_cost;

			Family _Des_buttons = FamilyManager.getFamily(new AnyOfTags("Button"), new AllOfComponents(typeof(Button), typeof(Lvl_Buttons)), new AnyOfLayers(8));

			foreach (GameObject go in _Des_buttons)
			{
				go.SetActive(false);
			}
			spawn.nb_enemies[i] = 0;
			// Actualizes the energy display to the player.
			energy_nb.text = "energy: " + bank.energy.ToString();
		}
	}
	// Controls des_virus1 button
	public void Des_Virus1_Button(int type = 1)
    {
		Destruction(0);
	}
	// Controls des_virus2 button
	public void Des_Virus2_Button(int type = 1)
	{
		Destruction(1);
	}
	// Controls des_bacterie1 button
	public void Des_Bacterie1_Button(int type = 1)
	{
		Destruction(2);
	}
	// Controls des_bacterie2 button
	public void Des_Bacterie2_Button(int type = 1)
	{
		Destruction(3);
	}

	// Controls the cancel button of the vaccine
	public void Des_Cancel_Button(int type = 1)
    {
		Family _Des_buttons = FamilyManager.getFamily(new AnyOfTags("Button"), new AllOfComponents(typeof(Button), typeof(Lvl_Buttons)), new AnyOfLayers(8));
		foreach (GameObject go in _Des_buttons)
		{
			go.SetActive(false);
		}
	}

	protected override void onProcess(int familiesUpdateCount) {
		spawn.energy_prog += Time.deltaTime;

		int secs = (int)Math.Floor(spawn.energy_prog);
		bank.energy += spawn.energy_sec * secs;
		spawn.energy_prog -= secs;

		// This part places the last tower bought and not placed, when the player clicks on the screen (in the position the player clicks).
		// It reactivates the tower so that it's taken in consideration elsewhere. 
		if (bank.used)
        {
			if (Input.GetMouseButton(0) && _Inactive_tower.First())
            {
				Vector3 mousePos = Input.mousePosition;
				mousePos.z = Camera.main.nearClipPlane;
				Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePos);

				GameObject go = _Inactive_tower.First();
				go.SetActive(true);
				go.transform.position = worldPosition;

				bank.used = false;
			}
        }
		// Actualizes the energy display to the player.
		energy_nb.text = "energy: " + bank.energy.ToString();

		// Enables and disables buttons according to their prices. 
		foreach (GameObject b in _Buttons){
			Text t = b.transform.GetChild(0).GetComponent<Text>();
			int value = int.Parse(t.text);
			if (bank.energy >= value)
            {
				Button button = b.GetComponent<Button>();
				button.interactable = true;
			}
            else
            {
				Button button = b.GetComponent<Button>();
				button.interactable = false;
			}
		}
	}
}