using UnityEngine;
using UnityEngine.SceneManagement;
using FYFY;
using UnityEngine.UI;

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
    private Family _Joueur = FamilyManager.getFamily(new AnyOfTags("Player"), new AllOfComponents(typeof(Has_Health), typeof(Bank)));
    private Family _Spawn = FamilyManager.getFamily(new AllOfComponents(typeof(Spawn)));

    // This is the family with all the menus
    private Family _Menu = FamilyManager.getFamily(new AllOfComponents(typeof(Menu)));

    // here we get all menu we need to activate/deactivate to show things outside of the levels.
    private GameObject bienvenu;
    private GameObject menu_init;
    private GameObject menu_help;
    private GameObject selection;

    // For the dropdown to select levels
    private int scene = -1;
    
    // controls for the different changes in scenes
    public bool start = false;
    public bool menu = false;
    public bool fin = false;
    public float timeBeforeLoad = 0.5f, timeBeforeFin = 3.0f, progressBefore;

    // because there's one button function here that is binded to a button that isn't on the masterscene 
    public static Load_Scene_System instance;

    public Load_Scene_System()
    {
        instance = this;
        buttonFamily.addEntryCallback(newButton);

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

        bienvenu.SetActive(true);
        menu_init.SetActive(false);
        menu_help.SetActive(false);
        selection.SetActive(false);
        
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
        if (Input.GetMouseButton(0) && bienvenu.activeInHierarchy)
        {
            bienvenu.SetActive(false);
            menu_init.SetActive(true);
        }

        if (start)
        {
            progressBefore += Time.deltaTime;
          
            if (progressBefore > timeBeforeLoad)
            {
                //GameObject test = GameObject.Find("test");
                //GameObjectManager.dontDestroyOnLoadAndRebind(test);

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
                start = false;
            }

        }

        if (menu)
        {
            progressBefore += Time.deltaTime;

            if (progressBefore > timeBeforeLoad)
            {
                //GameObjectManager.loadScene("MasterScene", LoadSceneMode.Single);
                GameObjectManager.unloadScene("Scene1");
                selection.SetActive(true);

                foreach (GameObject go in _AttackingGO) {
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

                menu = false;
            }

        }

        if (Input.GetKey(KeyCode.U))
        {
            menu = true;
            progressBefore = 0.0f;
            foreach (FSystem system in FSystemManager.fixedUpdateSystems())
            {
                //system.Pause = true;
                // Debug.Log(system.GetType().Name);
            }
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
        }

        GameObject joueur = _Joueur.First();
        if (joueur != null)
        {
            float health = joueur.GetComponent<Has_Health>().health;

            if (health <= 0)
            {
                if (!fin)
                {
                    progressBefore = 0.0f;
                    fin = true;
                }
                this.perdu();
            }
        }

        GameObject spawn = _Spawn.First(); 
        if (spawn != null)
        {
            Spawn s = spawn.GetComponent<Spawn>();
            if (_AttackingGO.First() == null)
            {
                if (s.nb_waves <= 0)
                {
                    if (!fin)
                    {
                        progressBefore = 0.0f;
                        fin = true;
                    }
                    this.gagne();
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

    // What happens when the player wins a level
    private void gagne()
    {
        progressBefore += Time.deltaTime;
        if (progressBefore > timeBeforeFin)
        {
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

            string s = "Scene" + scene.ToString();

            GameObjectManager.unloadScene(s);
            GameObjectManager.loadScene("Fin", LoadSceneMode.Additive);

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
            fin = false;
        }

    }
    // What happens when the player loses the level (aka their HP reaches 0)
    private void perdu()
    {
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
            string s = "Scene" + scene.ToString();
            GameObjectManager.unloadScene(s);
            GameObjectManager.loadScene("Fin", LoadSceneMode.Additive);

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
                fin = false;
            }
        }
    }

    // This function binds all the buttons not in the MasterScene to their respective methods.
    // I wish there were a less ugly way of doing it, but frankly I can't think of a better way and this works so...
    private void newButton(GameObject go)
    {
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
        if (go.name == "return_button")
        {
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(delegate {Load_Scene_System.instance.Fin_Button(1); });
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
    }

    //All buttons in the MasterScene have their functions here at the moment as they all control changes in scenes.
    // Buttons controlling scenes should be here so it's easier to control everything and the system has a sense.

    public void Start_Button(int amount = 1)
    {
        menu_init.SetActive(false);
        selection.SetActive(true);
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

    // The selection of level dropdown
    public void Dropdown(int choix = 1)
    {
        scene = choix;
        scene++;
        selection.SetActive(false);
        string s = "Scene" + scene.ToString();
        GameObjectManager.loadScene(s, LoadSceneMode.Additive);
        start = true;
        progressBefore = 0.0f;
        
    }

    public void Fin_Button(int amount = 1)
    {
        GameObjectManager.unloadScene("Fin");
        selection.SetActive(true);
    }

}