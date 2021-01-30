using UnityEngine;
using UnityEngine.SceneManagement;
using FYFY;
using UnityEngine.UI;

using System.Collections.Generic;
using System.Linq;

public class Load_Scene_System : FSystem {
    // Basically controls everything lol
    // This is the System that control which scenes are charged, how, when, etc. and when they are destroyed.
    // It also controls the binding of every button that isn't in the master scene.

    // For the binding of the buttons we have to get all of them
    private Family buttonFamily = FamilyManager.getFamily(new AnyOfTags("Button"));

    // Those are the objects that must be destroyed after a level ends
    private Family _AttackingGO = FamilyManager.getFamily(new AllOfComponents(typeof(Attack_J), typeof(Can_Move), typeof(Has_Health)));
    private Family _AllAttackersGO = FamilyManager.getFamily(new AllOfComponents(typeof(Can_Attack)));
    private Family _AllParticlesGO = FamilyManager.getFamily(new AllOfComponents(typeof(Lifespan)), new NoneOfComponents(typeof(Has_Health)));
    private Family _AllLymphocytesGO = FamilyManager.getFamily(new AllOfComponents(typeof(Anticorps_Factory)));
    private Family _ShadowGO = FamilyManager.getFamily(new AllOfComponents(typeof(Shadow)));

    // We need those to test if the level ended 
    private Family _Joueur = FamilyManager.getFamily(new AnyOfTags("Player"), new AllOfComponents(typeof(Has_Health), typeof(Bank), typeof(Current_Lvl), typeof(Score)));
    private Family _Spawn = FamilyManager.getFamily(new AllOfComponents(typeof(Spawn)));

    // This is the family with all the menus
    private Family _Menu = FamilyManager.getFamily(new AllOfComponents(typeof(Menu)));

    // We get the text GameObject at the end scene so we can write different messages on it
    private Family _text = FamilyManager.getFamily(new AllOfComponents(typeof(Msg_Fin), typeof(Text)), new AnyOfTags("Finish"));

    private Family _Particles;

    // To control the music and sound effects !
    // yes, it's complicated =)
    private Family _AudioSources = FamilyManager.getFamily(new AllOfComponents(typeof(AudioSource)));
    private static string main_theme_audio_source_name = "MainThemeAudioSource", level_in_progress_audio_source_name = "LevelInProgressAudioSource",
        level_completed_audio_source_name = "LevelCompletedAudioSource", level_failed_audio_source_name = "LevelFailedAudioSource",
        antibiotic_audio_source_name = "AntibioticAudioSource", vaccine_audio_source_name = "VaccineAudioSource",
        antibody_hit_audio_source_name = "AntibodyHitAudioSource", macrophage_hit_audio_source_name = "MacrophageHitAudioSource";
    private string[] audio_source_names = {
        main_theme_audio_source_name, level_in_progress_audio_source_name, level_completed_audio_source_name, level_failed_audio_source_name,
        antibiotic_audio_source_name, vaccine_audio_source_name, antibody_hit_audio_source_name, macrophage_hit_audio_source_name };
    private string[] sounds_audio_source_names = {
        antibiotic_audio_source_name, vaccine_audio_source_name, antibody_hit_audio_source_name, macrophage_hit_audio_source_name,
        level_completed_audio_source_name, level_failed_audio_source_name };
    private string[] music_audio_source_names = {
        main_theme_audio_source_name, level_in_progress_audio_source_name};
    private Dictionary<string, AudioSource> audio_sources_dict;
    private Dictionary<string, bool> was_playing_before_mute;
    private Dictionary<string, GameObject> go_s_with_audio_source;

    public bool musicMuted = false, soundsMuted = false;
    private Family _MuteCanvas = FamilyManager.getFamily(new AllOfComponents(typeof(Mute_Canvas)));

    // messages we write at the end of a level
    private string text_fin = null;
    private string text_score = null;
    private string text_pass = null;

    // if true we grayout the continue button in the fin scene
    private bool lost = true;

    // here we get all menu we need to activate/deactivate to show things outside of the levels.
    private GameObject bienvenu;
    private GameObject menu_init;
    private GameObject menu_encyclo;
    private GameObject menu_help;
    private GameObject selection;
    private GameObject charging_lvl;

    private GameObject joueur;

    // To select levels
    private int max_scene;
    private int unlocked_scene;
    private int current_scene;

    // controls for the different changes in scenes
    private bool start = false;
    private bool fin = false;
    private bool pause = false;
    private bool in_pause = false;
    private bool replay = false;

    // to be sure the scene is completely loaded before we start the systems again (yes, I tried without it and it doesn't work)
    private float timeBeforeLoad = 0.5f, timeBeforeFin = 1.0f, progressBefore, timeBeforePause = 0.1f, pauseDetectionProgress = 0.0f, pauseDetectionReload = 1.0f;
    private float explosion_force = 100.0f;

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
                    menu_encyclo = go;
                    break;
                case 4:
                    menu_help = go;
                    break;
                case 5:
                    charging_lvl = go;
                    break;
            }
        }

        // We start at the title screen
        bienvenu.SetActive(true);
        menu_init.SetActive(false);
        menu_encyclo.SetActive(false);
        menu_help.SetActive(false);
        selection.SetActive(false);
        charging_lvl.SetActive(false);

        foreach (GameObject m_canvas in _MuteCanvas)
            m_canvas.SetActive(false);

        joueur = _Joueur.First();
        max_scene = joueur.GetComponent<Current_Lvl>().max_scene;
        unlocked_scene = joueur.GetComponent<Current_Lvl>().unlocked_scene;
        current_scene = joueur.GetComponent<Current_Lvl>().current_scene;
        joueur.GetComponent<Score>().max_scores = new int[max_scene];
        for (int i = 0; i < joueur.GetComponent<Score>().max_scores.Length; i++)
        {
            joueur.GetComponent<Score>().max_scores[i] = 0;
        }

        audio_sources_dict = new Dictionary<string, AudioSource>();
        foreach (GameObject go_with_audio_source in _AudioSources)
            if (audio_source_names.Contains(go_with_audio_source.name))
                audio_sources_dict[go_with_audio_source.name] = go_with_audio_source.GetComponent<AudioSource>();
        go_s_with_audio_source = new Dictionary<string, GameObject>();
        foreach (GameObject go_with_audio_source in _AudioSources)
            go_s_with_audio_source[go_with_audio_source.name] = go_with_audio_source;
        was_playing_before_mute = new Dictionary<string, bool>();

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
    // yes, it's big and ugly.
    protected override void onProcess(int familiesUpdateCount)
    {
        pauseDetectionProgress += Time.deltaTime;
        // test to see if we're at the title screen
        if (Input.GetMouseButton(0) && bienvenu.activeInHierarchy)
        {
            bienvenu.SetActive(false);
            menu_init.SetActive(true);
            foreach (GameObject m_canvas in _MuteCanvas)
                m_canvas.SetActive(true);
        }

        // test to see if we're goint to start a level
        if (Input.GetMouseButton(0) && charging_lvl.activeInHierarchy && !start)
        {
            charging_lvl.SetActive(false);
            // unpause all systems
            unpause_systems();
            if (go_s_with_audio_source[main_theme_audio_source_name].activeSelf)
                audio_sources_dict[main_theme_audio_source_name].Stop();
            else
                was_playing_before_mute[main_theme_audio_source_name] = false;
            if (go_s_with_audio_source[level_in_progress_audio_source_name].activeSelf)
                audio_sources_dict[level_in_progress_audio_source_name].Play();
            else
                was_playing_before_mute[level_in_progress_audio_source_name] = true;
            foreach (GameObject m_canvas in _MuteCanvas)
                m_canvas.SetActive(false);
        }

        // test to see if the player choose a level (this is where we unpause the systems after the creation of the level scene)
        // this is necessary because if we try to unpause the systems just after the creation os the level scene, BAD THINGS HAPPEN 
        // (it seems not everything is created at the same time and the systems can't find necessary GameObjects)
        if (start)
        {
            charging_lvl.SetActive(true);
            Family _textGO = FamilyManager.getFamily(new AllOfComponents(typeof(Tips), typeof(Text)));
            foreach (GameObject tgo in _textGO)
            {
                if (tgo.GetComponent<Tips>().lvl_nb == current_scene)
                {
                    tgo.SetActive(true);
                }
                else
                {
                    tgo.SetActive(false);
                }
            }

            progressBefore += Time.deltaTime;
            // test to see if enough time has passed after the creation of the level scene, we can unpause the systems
            if (progressBefore > timeBeforeLoad)
            {
                // the level is loaded so start goes back to false
                start = false;
                // we set the starting energy and current energy in the bank 
                int energy = _Spawn.First().GetComponent<Spawn>().energy_start;
                joueur.GetComponent<Bank>().energy = energy;
                joueur.GetComponent<Bank>().init_energy = energy;

                // the score of the level goes back to zero.
                joueur.GetComponent<Score>().lvl_score = 0;
            }
        }

        // if the player wants to exit a level in the middle. It takes them to the lost level screen
        if (Input.GetKey(KeyCode.Q))
        {
            joueur.GetComponent<Has_Health>().health = -1;
        }

        // Pause 
        if (pauseDetectionProgress > pauseDetectionReload)
        {
            if (Input.GetKey(KeyCode.Escape) && !bienvenu.activeInHierarchy && !menu_init.activeInHierarchy &&
                !selection.activeInHierarchy && !menu_help.activeInHierarchy && !menu_encyclo.activeInHierarchy && text_fin == null)
            {
                pauseDetectionProgress = 0.0f;
                if (!pause)
                {
                    progressBefore = 0.0f;
                    pause = true;
                }
            }
        }
        this.pause_lvl();

        // restart level from the Pause menu
        if (replay)
        {
            progressBefore += Time.deltaTime;
            if (progressBefore > timeBeforeLoad)
            {
                // unpause all systems
                unpause_systems();

                // the level is reloaded so replay goes back to false
                replay = false;
            }
        }

        // test to see if we're at one of the levels
        if (!bienvenu.activeInHierarchy && !menu_init.activeInHierarchy && !selection.activeInHierarchy && !menu_help.activeInHierarchy && !menu_encyclo.activeInHierarchy &&!replay)
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
                    text_fin = "Niveau échoué !";

                    // the score of the level goes back to zero.
                    joueur.GetComponent<Score>().lvl_score = 0;
                    // Actualizes text_score
                    text_score = "Score du niveau : " + joueur.GetComponent<Score>().lvl_score.ToString() + "\nScore total : " + joueur.GetComponent<Score>().max_scores.Sum().ToString();

                    // Actualizes the password text
                    text_pass = "";

                    lost = true;
                    if (go_s_with_audio_source[level_in_progress_audio_source_name].activeSelf)
                        audio_sources_dict[level_in_progress_audio_source_name].Stop();
                    else
                        was_playing_before_mute[level_in_progress_audio_source_name] = false;
                    if (go_s_with_audio_source[level_failed_audio_source_name].activeSelf)
                        audio_sources_dict[level_failed_audio_source_name].Play();
                }
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
                            lost = false;
                            text_fin = "Niveau reussi !";

                            if (current_scene == max_scene)
                            {
                                text_fin = "Vous avez gagné le jeu !";
                            }

                            // Actualizes text_score
                            if (joueur.GetComponent<Score>().max_scores[current_scene - 1] <= joueur.GetComponent<Score>().lvl_score)
                            {
                                joueur.GetComponent<Score>().max_scores[current_scene - 1] = joueur.GetComponent<Score>().lvl_score;
                            }
                            text_score = "Score du niveau : " + joueur.GetComponent<Score>().lvl_score.ToString() + "\nScore total : " + joueur.GetComponent<Score>().max_scores.Sum().ToString();

                            // Actualizes the password text
                            if (current_scene < max_scene)
                            {
                                text_pass = "mot de passe : " + (current_scene + 1).ToString();
                            }
                            else
                            {
                                text_pass = "";
                            }

                            if (current_scene == unlocked_scene && unlocked_scene < max_scene)
                            {
                                unlocked_scene++;
                                joueur.GetComponent<Current_Lvl>().unlocked_scene = unlocked_scene;
                            }
                            if (go_s_with_audio_source[level_in_progress_audio_source_name].activeSelf)
                                audio_sources_dict[level_in_progress_audio_source_name].Stop();
                            else
                                was_playing_before_mute[level_in_progress_audio_source_name] = false;
                            if (go_s_with_audio_source[level_completed_audio_source_name].activeSelf)
                                audio_sources_dict[level_completed_audio_source_name].Play();
                        }
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

    // Pauses and unpauses the game
    private void pause_lvl()
    {
        float angle = 0.0f;
        if (pause)
        {
            progressBefore += Time.deltaTime;
            if (!in_pause) 
            {
                // we pause the systems that deal with level stuff
                pause_systems();

                _Particles = FamilyManager.getFamily(new AnyOfTags("Particle"), new AllOfComponents(typeof(Lifespan)));
                foreach (GameObject particle in _Particles)
                {
                    Rigidbody2D rb2d = particle.GetComponent<Rigidbody2D>();
                    rb2d.velocity = Vector2.zero;
                }
                if (progressBefore > timeBeforePause)
                {
                    GameObjectManager.loadScene("Pause", LoadSceneMode.Additive);
                    pause = false;
                    in_pause = true;
                }
            }
            else
            {
                if (progressBefore > timeBeforePause)
                {
                    GameObjectManager.unloadScene("Pause");
                    pause = false;
                    in_pause = false;

                    // we unpause the systems that deal with level stuff
                    unpause_systems();

                    foreach (GameObject particle in _Particles)
                    {
                        angle = Mathf.PI * particle.transform.rotation.eulerAngles.z / 180.0f;
                        Rigidbody2D rb2d = particle.GetComponent<Rigidbody2D>();
                        rb2d.AddForce(new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * explosion_force * Random.Range(0.5f, 1.0f));
                    }
                }
            }
        }
    }


    // What happens when the player wins or loses a level (aka their HP reaches 0)
    private void fin_lvl()
    {
        // we pause the systems that deal with level stuff
        pause_systems();

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
            foreach (GameObject m_canvas in _MuteCanvas)
                m_canvas.SetActive(true);

            // destroy any objects from the level that still exist
            destroy_lvl_objects();

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
        if (go.name == "lymp1_button")
        {
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(delegate { Energy_System.instance.Lymp1_Button(1); });
        }
        if (go.name == "lymp2_button")
        {
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(delegate { Energy_System.instance.Lymp2_Button(1); });
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
        if (go.name == "des_virus1")
        {
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(delegate { Energy_System.instance.Des_Virus1_Button(1); });
            go.SetActive(false);
        }
        if (go.name == "des_virus2")
        {
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(delegate { Energy_System.instance.Des_Virus2_Button(1); });
            go.SetActive(false);
        }
        if (go.name == "des_bacterie1")
        {
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(delegate { Energy_System.instance.Des_Bacterie1_Button(1); });
            go.SetActive(false);
        }
        if (go.name == "des_bacterie2")
        {
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(delegate { Energy_System.instance.Des_Bacterie2_Button(1); });
            go.SetActive(false);
        }
        if (go.name == "des_cancel")
        {
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(delegate { Energy_System.instance.Des_Cancel_Button(1); });
            go.SetActive(false);
        }
        if (go.name == "back_from_lvl_button")
        {
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(delegate { Load_Scene_System.instance.Back_From_Lvl_Button(1); });
        }
        if (go.name == "menu_from_lvl_button")
        {
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(delegate { Load_Scene_System.instance.Menu_From_Lvl_Button(1); });
        }
        if (go.name == "mute_sound_button")
        {
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(delegate { Load_Scene_System.instance.MuteSound(1); });
        }
        if (go.name == "mute_music_button")
        {
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(delegate { Load_Scene_System.instance.MuteMusic(1); });
        }

        // end scene buttons
        if (go.name == "return_button")
        {
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(delegate {Load_Scene_System.instance.Return_To_Menu_From_Fin_Button(1); });
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
            
            if ((lost && current_scene == unlocked_scene) || current_scene >= max_scene)
            {
                btn.interactable = false;
            }
        }

        // pause scene buttons
        if (go.name == "continue_button")
        {
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(delegate { Load_Scene_System.instance.Continue_Button(1); });
        }
        if (go.name == "replay_pause_button")
        {
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(delegate { Load_Scene_System.instance.Replay_Pause_Button(1); });
        }
        if (go.name == "menu_button")
        {
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(delegate { Load_Scene_System.instance.Return_To_Menu_From_Pause_Button(1); });
        }
        if (go.name == "exit_button")
        {
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(delegate { Load_Scene_System.instance.Exit_From_Pause_Button(1); });
        }
    }

    // We do a callback when both texts at the end scene are created because this is the only way to be sure we can change the text consistently without having a NullReferenceException 
    // (because it seems it takes some time for the scene to load)
    private void end_text(GameObject go)
    {
        foreach (GameObject gg in _text)
        {
            if (gg.GetComponent<Msg_Fin>().txt_nb == 0)
            {
                Text text = gg.GetComponent<Text>();
                text.text = text_fin;
            }
            if (gg.GetComponent<Msg_Fin>().txt_nb == 1)
            {
                Text text = gg.GetComponent<Text>();
                text.text = text_score;
            }
            if (gg.GetComponent<Msg_Fin>().txt_nb == 2)
            {
                Text text = gg.GetComponent<Text>();
                text.text = text_pass;
            }

        }
    }

    //All buttons in the MasterScene have their functions here at the moment as they all control changes in scenes.
    // Buttons controlling scenes should be here so it's easier to control everything and the system has a reason for existing.

    // initial menu buttons

    //Goes to the selection screen and restarts the level dropdown menu so that new unlocked levels are added
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

    // Controls the input thing in the selection menu. The idea is to use a hashtable of some sort so that each lvl has an unique code. 
    // Right now it's simply the nulber of the level so we can navigate between levels easily.
    public void Password(string s)
    {
        Family _pass = FamilyManager.getFamily(new AllOfComponents(typeof(InputField)));
        InputField pass = _pass.First().GetComponent<InputField>();
        string ss = pass.text; 
        int pass_unlocked_scene = 0;
        if (System.Int32.TryParse(ss, out pass_unlocked_scene) && (pass_unlocked_scene <= max_scene) && (pass_unlocked_scene > 0))
        {
            unlocked_scene = pass_unlocked_scene;
        }
        Start_Button();
    }

    // less repeated code haha
    private void Help_Encyclo_Button(int type = 1) 
    {
        Family _HelpPages = FamilyManager.getFamily(new AllOfComponents(typeof(Help)));
        foreach (GameObject go in _HelpPages)
        {
            Help h = go.GetComponent<Help>();
            if (h.help_nb == 0 && h.type == type)
            {
                go.SetActive(true);
            }
            else
            {
                go.SetActive(false);
            }
        }
    }

    // Goes to the first help screen
    public void Help_Button(int amount = 1)
    {
        menu_init.SetActive(false);
        menu_help.SetActive(true);
        Help_Encyclo_Button(2);
        foreach (GameObject m_canvas in _MuteCanvas)
            m_canvas.SetActive(false);
    }

    // Goes to the first encyclopedia screen
    public void Encyclo_Button(int amount = 1)
    {
        menu_init.SetActive(false);
        menu_encyclo.SetActive(true);
        Help_Encyclo_Button(1);
        foreach (GameObject m_canvas in _MuteCanvas)
            m_canvas.SetActive(false);
    }

    // help and encyclopedia menu buttons (back_button is also used in the selection menu)
    public void Back_Button(int obj = 1)
    {
        // obj = 0 -> selection menu
        // obj = 1 -> encyclopedia menu
        // obj = 2 -> help menu

        if (obj == 0)
        {
            selection.SetActive(false);
        }
        if (obj == 1)
        {
            menu_encyclo.SetActive(false);
        }
        if (obj == 2)
        {
            menu_help.SetActive(false);
        }
        menu_init.SetActive(true);
        foreach (GameObject m_canvas in _MuteCanvas)
            m_canvas.SetActive(true);
    }

    // next page of the help/encyclopedia menu (if it exists) goes back to the menu if it doesn't
    public void Next_Button(int obj = 1)
    {
        obj++;
        bool last = true;
        int type = 1;
        if (menu_help.activeSelf)
        {
            type = 2;
        }
        Family _HelpPages = FamilyManager.getFamily(new AllOfComponents(typeof(Help)));
        foreach (GameObject go in _HelpPages)
        {
            Help h = go.GetComponent<Help>();
            if ((h.help_nb == obj) && (h.type == type))
            {
                go.SetActive(true);
                last = false;
            }
            else
            {
                go.SetActive(false);
            }
        }
        //Debug.Log("active:" + menu_help.activeSelf.ToString());
        //Debug.Log("last:" + last.ToString());
        if (last && menu_help.activeSelf)
        {
            menu_help.SetActive(false);
            menu_init.SetActive(true);
            foreach (GameObject m_canvas in _MuteCanvas)
                m_canvas.SetActive(true);
        }
        if (last && menu_encyclo.activeSelf)
        {
            menu_encyclo.SetActive(false);
            menu_init.SetActive(true);
            foreach (GameObject m_canvas in _MuteCanvas)
                m_canvas.SetActive(true);
        }
    }

    // previous page of the help/encyclopedia menu (if it exists) goes back to the menu if it doesn't
    public void Before_Button(int obj = 1)
    {
        obj--;
        int type = 1;
        if (menu_help.activeSelf)
        {
            type = 2;
        }
        if (obj >= 0)
        {
            Family _HelpPages = FamilyManager.getFamily(new AllOfComponents(typeof(Help)));
            foreach (GameObject go in _HelpPages)
            {
                Help h = go.GetComponent<Help>();
                if ((h.help_nb == obj) && (h.type == type))
                {
                    go.SetActive(true);
                }
                else
                {
                    go.SetActive(false);
                }
            }
        }
        else
        {
            if (menu_help.activeSelf)
            {
                menu_help.SetActive(false);
                menu_init.SetActive(true);
            }
            if (menu_encyclo.activeSelf)
            {
                menu_encyclo.SetActive(false);
                menu_init.SetActive(true);
            }
        }
    }

    // Selection of level dropdown
    public void Dropdown(int choix = 1)
    {
        Bank bank = _Joueur.First().GetComponent<Bank>();
        bank.used = false;
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

    // Goes back to the menu screen
    public void Return_To_Menu_From_Fin_Button(int amount = 1)
    {
        GameObjectManager.unloadScene("Fin");
        menu_init.SetActive(true);
        text_fin = null;
        // Debug.Log("Returned to menu");
        if (go_s_with_audio_source[main_theme_audio_source_name].activeSelf)
            audio_sources_dict[main_theme_audio_source_name].Play();
        else
            was_playing_before_mute[main_theme_audio_source_name] = true;
    }

    // Loads next level (the button is deactivated if the player can't do this or if there isn't a next level)
    public void Next_Level_Button(int amount = 1)
    {
        Bank bank = _Joueur.First().GetComponent<Bank>();
        bank.used = false;
        current_scene++;
        joueur.GetComponent<Current_Lvl>().current_scene = current_scene;
        string s = "Scene" + current_scene.ToString();
        GameObjectManager.unloadScene("Fin");
        GameObjectManager.loadScene(s, LoadSceneMode.Additive);
        start = true;
        progressBefore = 0.0f;
        text_fin = null;
    }

    // Reloads the level
    public void Play_Again_Button(int actual_lvl = 0)
    {
        Bank bank = _Joueur.First().GetComponent<Bank>();
        bank.used = false;
        string s = "Scene" + current_scene.ToString();
        GameObjectManager.unloadScene("Fin");
        GameObjectManager.loadScene(s, LoadSceneMode.Additive);
        start = true;
        progressBefore = 0.0f;
        text_fin = null;
    }

    // Pause scene buttons

    // go back to the level
    public void Continue_Button(int amount = 1)
    {
        progressBefore = 0.0f;
        pause = true;
    }

    // Restarts the level
    public void Replay_Pause_Button(int amount = 1)
    {
        replay = true;
        in_pause = false;
        
        // see what level it was
        string s = "Scene" + current_scene.ToString();

        // we pause the systems that deal with level stuff.
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

        //we unload the level (easier way of getting everything back to their default values).
        GameObjectManager.unloadScene(s);
        GameObjectManager.unloadScene("Pause");

        // put the Joueur in their default status
        Bank bank = _Joueur.First().GetComponent<Bank>();
        bank.used = false;
        int max_health = joueur.GetComponent<Has_Health>().max_health;
        joueur.GetComponent<Has_Health>().health = max_health;
        int init_energy = joueur.GetComponent<Bank>().init_energy;
        joueur.GetComponent<Bank>().energy = init_energy;
        // the score of the level goes back to zero.
        joueur.GetComponent<Score>().lvl_score = 0;

        // destroy any objects from the level that still exist
        destroy_lvl_objects();

        // start the counter to the reestart
        progressBefore = 0.0f;

        // reload the scene
        GameObjectManager.loadScene(s, LoadSceneMode.Additive);
    }

    // go back to the menu 
    public void Return_To_Menu_From_Pause_Button(int amount = 1)
    {
        in_pause = false;
        // see what level it was
        string s = "Scene" + current_scene.ToString();

        // we pause the systems that deal with level stuff
        pause_systems();

        //we unload the level and the Pause scene
        GameObjectManager.unloadScene(s);
        GameObjectManager.unloadScene("Pause");

        // put the Joueur in their default status
        Bank bank = _Joueur.First().GetComponent<Bank>();
        bank.used = false;
        int max_health = joueur.GetComponent<Has_Health>().max_health;
        joueur.GetComponent<Has_Health>().health = max_health;

        // destroy any objects from the level that still exist
        destroy_lvl_objects();

        menu_init.SetActive(true);
        text_fin = null;

        if (go_s_with_audio_source[level_in_progress_audio_source_name].activeSelf)
            audio_sources_dict[level_in_progress_audio_source_name].Stop();
        else
            was_playing_before_mute[level_in_progress_audio_source_name] = false;
        if (go_s_with_audio_source[main_theme_audio_source_name].activeSelf)
            audio_sources_dict[main_theme_audio_source_name].Play();
        else
            was_playing_before_mute[main_theme_audio_source_name] = true;
        foreach (GameObject m_canvas in _MuteCanvas)
            m_canvas.SetActive(true);
    }

    // quits the game
    public void Exit_From_Pause_Button(int amount = 1)
    {
        Application.Quit();
    }

    // Level scene buttons (here because it controls the destruction of the level scene)

    // go back to menu
    public void Back_From_Lvl_Button(int amount = 1)
    {
        joueur.GetComponent<Has_Health>().health = -1;
    }

    // pause menu
    public void Menu_From_Lvl_Button(int amount = 1)
    {
        pauseDetectionProgress = 0.0f;
        if (!pause)
        {
            progressBefore = 0.0f;
            pause = true;
        }
    }

    //Controls sounds effects
    public void MuteSound(int amount = 1)
    {
        foreach (GameObject go_with_audio_source in _AudioSources)
            if (sounds_audio_source_names.Contains(go_with_audio_source.name))
                go_with_audio_source.SetActive(!go_with_audio_source.activeSelf);
        soundsMuted = !soundsMuted;
        // Debug.Log("Sound muted");
    }

    // Controls the music
    public void MuteMusic(int amount = 1)
    {
        foreach (GameObject go_with_audio_source in _AudioSources)
        {

            if (music_audio_source_names.Contains(go_with_audio_source.name))
            {
                if (!musicMuted)
                    was_playing_before_mute[go_with_audio_source.name] = audio_sources_dict[go_with_audio_source.name].isPlaying;
                go_with_audio_source.SetActive(!go_with_audio_source.activeSelf);
                if (musicMuted)
                {
                    if (was_playing_before_mute[go_with_audio_source.name])
                        audio_sources_dict[go_with_audio_source.name].Play();
                    else
                        audio_sources_dict[go_with_audio_source.name].Stop();
                }
            }
        }
        musicMuted = !musicMuted;
        // Debug.Log("Music muted");
    }

    // Functions to facilitate things

    private void destroy_lvl_objects()
    {
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
        foreach (GameObject go in _ShadowGO)
        {
            GameObjectManager.unbind(go);
            Object.Destroy(go);
        }
    }

    private void pause_systems()
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
    }

    private void unpause_systems()
    {
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
    }
}