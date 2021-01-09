﻿using UnityEngine;
using UnityEngine.SceneManagement;
using FYFY;
using UnityEngine.UI;

using System.Collections.Generic;

public class Load_Scene_System : FSystem {
    // Basically controls everything lol
    // This is the System that control which scenes are charged, how, when, etc. and when they are destroyed.
    // It also controls the binding of evey button that isn't in the master scene.

    // For the binding of the buttons we have to get all of them
    private Family buttonFamily = FamilyManager.getFamily(new AnyOfTags("Button"));

    // Those are the objects that must be destroyed after a level ends
    private Family _AttackingGO = FamilyManager.getFamily(new AllOfComponents(typeof(Attack_J), typeof(Can_Move), typeof(Has_Health)));
    private Family _AllAttackersGO = FamilyManager.getFamily(new AllOfComponents(typeof(Can_Attack)));
    private Family _AllParticlesGO = FamilyManager.getFamily(new AllOfComponents(typeof(Lifespan)), new NoneOfComponents(typeof(Has_Health)));
    private Family _AllLymphocytesGO = FamilyManager.getFamily(new AllOfComponents(typeof(Anticorps_Factory)));

    // We need those to test if the level ended 
    private Family _Joueur = FamilyManager.getFamily(new AnyOfTags("Player"), new AllOfComponents(typeof(Has_Health), typeof(Bank), typeof(Current_Lvl)));
    private Family _Spawn = FamilyManager.getFamily(new AllOfComponents(typeof(Spawn)));

    // This is the family with all the menus
    private Family _Menu = FamilyManager.getFamily(new AllOfComponents(typeof(Menu)));

    // We get the text GameObject at the end scene so we can write different messages on it
    private Family _text = FamilyManager.getFamily(new AllOfComponents(typeof(Msg_Fin), typeof(Text)));

    // the message we write at the end of a level
    private string text_fin;

    // if true we grayout the continue button in the fin scene
    private bool lost = true;

    // here we get all menu we need to activate/deactivate to show things outside of the levels.
    private GameObject bienvenu;
    private GameObject menu_init;
    private GameObject menu_help;
    private GameObject selection;

    private GameObject joueur;

    // To select levels
    private int max_scene;
    private int unlocked_scene;
    private int current_scene;

    // controls for the different changes in scenes
    private bool start = false;
    private bool fin = false;

    // to be sure the scene is completely loaded before we start the systems again (yes, I tried without it and it doesn't work)
    private float timeBeforeLoad = 0.5f, timeBeforeFin = 1.0f, progressBefore;

    // because there are buttons functions here that are binded to buttons that aren't on the masterscene 
    public static Load_Scene_System instance;

    public Load_Scene_System()
    {
        instance = this;
        buttonFamily.addEntryCallback(newButton);
        _text.addEntryCallback(end_text);

        // we get each of those menu GameObjects so it's easier to navigate between menus in the code
        foreach (GameObject go in _Menu)
        {
            Menu m = go.GetComponent<Menu>();
            switch (m.menu_nb)
            {
                case 0:
                    bienvenu = go;
                    break;
                case 1:
                    menu_init = go;
                    break;
                case 2:
                    selection = go;
                    break;
                case 3:
                    menu_help = go;
                    break;
            }
        }

        // We start at the title screen
        bienvenu.SetActive(true);
        menu_init.SetActive(false);
        menu_help.SetActive(false);
        selection.SetActive(false);

        joueur = _Joueur.First();
        max_scene = joueur.GetComponent<Current_Lvl>().max_scene;
        unlocked_scene = joueur.GetComponent<Current_Lvl>().unlocked_scene;
        current_scene = joueur.GetComponent<Current_Lvl>().current_scene;

    }
    // Use this to update member variables when system pause. 
    // Advice: avoid to update your families inside this function.
    protected override void onPause(int currentFrame)
    {
    }

    // Use this to update member variables when system resume.
    // Advice: avoid to update your families inside this function.
    protected override void onResume(int currentFrame)
    {
    }

    // Use to process your families.
    // yes, it's big and ugly now.
    protected override void onProcess(int familiesUpdateCount)
    {
        // test to see if we're at the title screen
        if (Input.GetMouseButton(0) && bienvenu.activeInHierarchy)
        {
            bienvenu.SetActive(false);
            menu_init.SetActive(true);
        }

        // test to see if the player choose a level (this is where is unpause the systems after the creation of the level scene)
        // this is necessary because if we try to unpause the systems just after the creation os the level scene, BAD THINGS HAPPEN 
        // (it seems not everything is created at the same time and the systems can't find necessary GameObjects)
        if (start)
        {
            progressBefore += Time.deltaTime;
            // test to see if enough time has passed after the creation of the level scene, we can unpause the systems
            if (progressBefore > timeBeforeLoad)
            {
                // unpause all systems
                foreach (FSystem system in FSystemManager.fixedUpdateSystems())
                {
                    system.Pause = false;
                    // Debug.Log(system.GetType().Name);
                }
                foreach (FSystem system in FSystemManager.updateSystems())
                {
                    system.Pause = false;
                    // Debug.Log(system.GetType().Name);
                }
                foreach (FSystem system in FSystemManager.lateUpdateSystems())
                {
                    system.Pause = false;
                    // Debug.Log(system.GetType().Name);
                }
                // the level is loaded so start goes back to false
                start = false;
            }
        }

        // if the player wants to exit a level in the middle. It takes them to the lost level screen
        if (Input.GetKey(KeyCode.U))
        {
            joueur.GetComponent<Has_Health>().health = -1;
        }

        // test to see if we're at one of the levels
        if (!bienvenu.activeInHierarchy && !menu_init.activeInHierarchy && !selection.activeInHierarchy && !menu_help.activeInHierarchy)
        {
            GameObject spawn = _Spawn.First();

            // test to see if the player lost a level
            float health = joueur.GetComponent<Has_Health>().health;
            if (health <= 0)
            {
                // this restarts the counter for the load of the end scene
                if (!fin)
                {
                    progressBefore = 0.0f;
                    fin = true;
                }
                text_fin = "Niveau échoué !";
                lost = true;
                this.fin_lvl();

            }
            // test to see if the player won a level
            if (spawn != null)
            {
                Spawn s = spawn.GetComponent<Spawn>();
                if (_AttackingGO.First() == null)
                {
                    if (s.nb_waves <= 0)
                    {
                        // this restarts the counter for the load of the end scene
                        if (!fin)
                        {
                            progressBefore = 0.0f;
                            fin = true;
                            if (current_scene == unlocked_scene && unlocked_scene < max_scene)
                            {
                                unlocked_scene++;
                                joueur.GetComponent<Current_Lvl>().unlocked_scene = unlocked_scene;
                            }

                        }
                        lost = false;
                        text_fin = "Niveau reussi !";
                        this.fin_lvl();
                    }
                    else
                    {
                        bool enemies = false;
                        foreach (int i in s.nb_enemies)
                        {
                            if (i > 0)
                            {
                                enemies = true;
                            }
                        }
                        if (!enemies && !fin)
                        {
                            s.nb_waves = 0;
                        }
                    }
                }
            }
        }
    }

    // What happens when the player wins or loses a level (aka their HP reaches 0)
    private void fin_lvl()
    {
        // we pause the systems that deal with level stuff
        foreach (FSystem system in FSystemManager.updateSystems())
        {
            system.Pause = true;
            // Debug.Log(system.GetType().Name);
        }
        foreach (FSystem system in FSystemManager.lateUpdateSystems())
        {
            system.Pause = true;
            // Debug.Log(system.GetType().Name);
        }
        progressBefore += Time.deltaTime;
        if (progressBefore > timeBeforeFin)
        {
            int max_health = joueur.GetComponent<Has_Health>().max_health;
            joueur.GetComponent<Has_Health>().health = max_health;

            int init_energy = joueur.GetComponent<Bank>().init_energy;
            joueur.GetComponent<Bank>().energy = init_energy;

            // see what level it was, destroy the correct scene and load the scene for the end of a level
            string s = "Scene" + current_scene.ToString();
            GameObjectManager.unloadScene(s);
            GameObjectManager.loadScene("Fin", LoadSceneMode.Additive);

            // destroy any objects from the level that still exist
            foreach (GameObject go in _AttackingGO)
            {
                GameObjectManager.unbind(go);
                Object.Destroy(go);
            }
            foreach (GameObject go in _AllAttackersGO)
            {
                GameObjectManager.unbind(go);
                Object.Destroy(go);
            }
            foreach (GameObject go in _AllParticlesGO)
            {
                GameObjectManager.unbind(go);
                Object.Destroy(go);
            }
            foreach (GameObject go in _AllLymphocytesGO)
            {
                GameObjectManager.unbind(go);
                Object.Destroy(go);
                
            }

            // fin scene is loaded so fin goes back to false
            fin = false;
        }
    }

    // This function binds all the buttons not in the MasterScene to their respective methods.
    // I wish there were a less ugly way of doing it, but frankly I can't think of a better way and this works so...
    private void newButton(GameObject go)
    {
        // level buttons
        if (go.name == "macro_button")
        {
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(delegate { Energy_System.instance.Macro_Button(1); });
        }
        if (go.name == "lymp_button")
        {
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(delegate { Energy_System.instance.Lymp_Button(1); });
        }
        if (go.name == "anti_button")
        {
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(delegate { Energy_System.instance.Anti_Button(1); });
        }
        if (go.name == "vaci_button")
        {
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(delegate { Energy_System.instance.Vaci_Button(1); });
        }
        if (go.name == "des_virus")
        {
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(delegate { Energy_System.instance.Des_Virus_Button(1); });
            go.SetActive(false);
        }
        if (go.name == "des_bacterie")
        {
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(delegate { Energy_System.instance.Des_Bacterie_Button(1); });
            go.SetActive(false);
        }

        // end scene buttons
        if (go.name == "return_button")
        {
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(delegate {Load_Scene_System.instance.Return_Menu_Button(1); });
        }
        if (go.name == "replay_button")
        {
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(delegate { Load_Scene_System.instance.Play_Again_Button(1); });
        }
        if (go.name == "next_lvl_button")
        {
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(delegate { Load_Scene_System.instance.Next_Level_Button(1); });
            
            if (lost || current_scene >= max_scene)
            {
                btn.interactable = false;
            }

        }
    }

    // We do a callback when the text at the end scene is created because this is the only way to be sure we can change the text consistently without having a NullReferenceException 
    // (because it seems it takes some time for the scene to load)
    private void end_text(GameObject go)
    {
        Text text = _text.First().GetComponent<Text>();
        text.text = text_fin;
    }

    //All buttons in the MasterScene have their functions here at the moment as they all control changes in scenes.
    // Buttons controlling scenes should be here so it's easier to control everything and the system has a reason for existing.

    // initial menu buttons
    public void Start_Button(int amount = 1)
    {
        menu_init.SetActive(false);
        selection.SetActive(true);

        Family _drop = FamilyManager.getFamily(new AllOfComponents(typeof(Dropdown)));
        Dropdown dropdown = _drop.First().GetComponent<Dropdown>();

        dropdown.ClearOptions();

        List<Dropdown.OptionData> m_Messages = new List<Dropdown.OptionData>();

        Dropdown.OptionData m_NewData = new Dropdown.OptionData();
        m_NewData.text = "--Niveaux--";
        m_Messages.Add(m_NewData);

        for (int i = 1; i <= unlocked_scene; i++)
        {
            m_NewData = new Dropdown.OptionData();
            m_NewData.text = "Niveau " + i.ToString();
            m_Messages.Add(m_NewData);
        }
        foreach (Dropdown.OptionData message in m_Messages)
        {
            //Add each entry to the Dropdown
            dropdown.options.Add(message);
        }
    }

    public void Help_Button(int amount = 1) 
    {
        menu_init.SetActive(false);
        menu_help.SetActive(true);
        Family _HelpPages = FamilyManager.getFamily(new AllOfComponents(typeof(Help)));
        foreach (GameObject go in _HelpPages)
        {
            Help h = go.GetComponent<Help>();
            if (h.help_nb == 0)
            {
                go.SetActive(true);
            }
            else
            {
                go.SetActive(false);
            }
        }
    }


    // help menu buttons (back_button is also used in the selection menu)
    public void Back_Button(int obj = 1)
    {
        // obj = 0 -> help menu
        // obj = 1 -> selection menu

        if (obj == 0)
        {
            menu_help.SetActive(false);
        }
        if (obj == 1)
        {
            selection.SetActive(false);
        }

        menu_init.SetActive(true);
    }

    public void Next_Button(int obj = 1)
    {
        obj++;
        bool last = true;
        Family _HelpPages = FamilyManager.getFamily(new AllOfComponents(typeof(Help)));
        foreach (GameObject go in _HelpPages)
        {
            Help h = go.GetComponent<Help>();
            if (h.help_nb == obj)
            {
                go.SetActive(true);
                last = false;
            }
            else
            {
                go.SetActive(false);
            }
        }
        if (last)
        {
            menu_help.SetActive(false);
            menu_init.SetActive(true);
        }

    }

    // Selection of level dropdown
    public void Dropdown(int choix = 1)
    {
        if (choix > 0)
        {
            current_scene = choix;
            joueur.GetComponent<Current_Lvl>().current_scene = current_scene;
            selection.SetActive(false);
            string s = "Scene" + current_scene.ToString();
            GameObjectManager.loadScene(s, LoadSceneMode.Additive);
            start = true;
            progressBefore = 0.0f;
        }
    }

    // Fin scene buttons (they are here because they control scene changes)
    public void Return_Menu_Button(int amount = 1)
    {
        GameObjectManager.unloadScene("Fin");
        menu_init.SetActive(true);
    }

    public void Next_Level_Button(int amount = 1)
    {
        current_scene++;
        joueur.GetComponent<Current_Lvl>().current_scene = current_scene;
        string s = "Scene" + current_scene.ToString();
        GameObjectManager.unloadScene("Fin");
        GameObjectManager.loadScene(s, LoadSceneMode.Additive);
        start = true;
        progressBefore = 0.0f;
    }


    public void Play_Again_Button(int actual_lvl = 0)
    {
        string s = "Scene" + current_scene.ToString();
        GameObjectManager.unloadScene("Fin");
        GameObjectManager.loadScene(s, LoadSceneMode.Additive);
        start = true;
        progressBefore = 0.0f;
    }

}