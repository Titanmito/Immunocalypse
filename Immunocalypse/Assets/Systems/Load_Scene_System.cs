using UnityEngine;
using UnityEngine.SceneManagement;
using FYFY;
using UnityEngine.UI;

public class Load_Scene_System : FSystem {
    bool Scene1Loaded = false;
    Family testF;
    Family buttonFamily = FamilyManager.getFamily(new AnyOfTags("Button"));
    float timeBeforeLoad = 0.5f, progressBeforeLoad = 0.0f;

    public Load_Scene_System() {
        if (!Scene1Loaded) {
            GameObjectManager.loadScene("Scene1", LoadSceneMode.Additive);
            Scene1Loaded = true;
        }
        buttonFamily.addEntryCallback(newButton);
    }
    // Use this to update member variables when system pause. 
    // Advice: avoid to update your families inside this function.
    protected override void onPause(int currentFrame) {
	}

	// Use this to update member variables when system resume.
	// Advice: avoid to update your families inside this function.
	protected override void onResume(int currentFrame){
	}

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount) {

        // if (Input.GetKey(KeyCode.Space)) {
        //testF = FamilyManager.getFamily(new AllOfComponents(typeof(Has_Health)));
        //foreach (GameObject go in testF) {
        //    Debug.Log(go.name);
        //}
        if (progressBeforeLoad >= 0)
            progressBeforeLoad += Time.deltaTime;
        if (progressBeforeLoad > timeBeforeLoad) {
            progressBeforeLoad = -1f;
            foreach (FSystem system in FSystemManager.fixedUpdateSystems()) {
                system.Pause = false;
                // Debug.Log(system.GetType().Name);
            }
            foreach (FSystem system in FSystemManager.updateSystems()) {
                system.Pause = false;
                // Debug.Log(system.GetType().Name);
            }
            foreach (FSystem system in FSystemManager.lateUpdateSystems()) {
                system.Pause = false;
                // Debug.Log(system.GetType().Name);
            }
        }
        // }
        if (Input.GetKey(KeyCode.U)) {
            if (Scene1Loaded) {
                GameObjectManager.loadScene("MasterScene", LoadSceneMode.Single);
                Scene1Loaded = false;
            }
        }
    }

    private void newButton(GameObject go) {
        if (go.name == "macro_button") {
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(delegate { Energy_System.instance.Macro_Button(1); });
        }
        if (go.name == "lymp_button") {
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(delegate { Energy_System.instance.Lymp_Button(1); });
        }
        if (go.name == "anti_button") {
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(delegate { Energy_System.instance.Anti_Button(1); });
        }
    }
}