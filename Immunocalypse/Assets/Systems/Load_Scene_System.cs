using UnityEngine;
using UnityEngine.SceneManagement;
using FYFY;
using UnityEngine.UI;

public class Load_Scene_System : FSystem {
    // Basically controls everything lol
    // This is the System that control which scenes are charged, how, when, etc. and when they are destroyed.
    // It also controls the binding of evey button that isn't in the master scene.

    private Family buttonFamily = FamilyManager.getFamily(new AnyOfTags("Button"));
    private Family _AttackingGO = FamilyManager.getFamily(new AllOfComponents(typeof(Attack_J), typeof(Can_Move), typeof(Has_Health)));
    private Family _AllAttackersGO = FamilyManager.getFamily(new AllOfComponents(typeof(Can_Attack)));
    private Family _AllParticlesGO = FamilyManager.getFamily(new AllOfComponents(typeof(Lifespan)), new NoneOfComponents(typeof(Has_Health)));
    private Family _AllLymphocytesGO = FamilyManager.getFamily(new AllOfComponents(typeof(Anticorps_Factory)));

    private Family _Joueur = FamilyManager.getFamily(new AnyOfTags("Player"), new AllOfComponents(typeof(Has_Health), typeof(Bank)));
    private Family _Spawn = FamilyManager.getFamily(new AllOfComponents(typeof(Spawn)));

    private GameObject selection;    
    
    public bool start = false;
    public bool menu = false;
    public bool fin = false;
    public float timeBeforeLoad = 0.5f, timeBeforeFin = 3.0f, progressBefore;

    public static Load_Scene_System instance;

    public Load_Scene_System()
    {
        instance = this;
        buttonFamily.addEntryCallback(newButton);

        selection = GameObject.Find("Menu");
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
    protected override void onProcess(int familiesUpdateCount)
    {
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

            GameObjectManager.unloadScene("Scene1");
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
            GameObjectManager.unloadScene("Scene1");
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

    public void Start_Button(int amount = 1)
    {
        selection.SetActive(false);

        GameObjectManager.loadScene("Scene1", LoadSceneMode.Additive);
        start = true;
        progressBefore = 0.0f;

    }
    
    public void Fin_Button(int amount = 1)
    {
        GameObjectManager.unloadScene("Fin");
        selection.SetActive(true);
    }
    
}