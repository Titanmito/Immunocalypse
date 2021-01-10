using UnityEngine;
using UnityEngine.SceneManagement;
using FYFY;
using System;

public class Pause_Management_System : FSystem {
    // Use this to update member variables when system pause. 
    // Advice: avoid to update your families inside this function.

    private String[] systemsToPause = {
        "Anticorps_System", "Attack_J_System", "Attack_System", "Destruction_System", "Energy_System",
        "Movement_System", "Spawn_System"
    };
    private String[] systemsToResume = {};
    private bool gamePaused = false;
    private bool gameWasSuccessfullyPutOnPause = false;
    private bool sceneReloaded = false;
    private string pauseSceneName = "Pause";
    private float progress = 0.0f, reload = 0.1f;

    private Family masterSceneElementsToPause;
    private Family masterSceneElementsToDestroyOnPause;

    protected override void onPause(int currentFrame) {
	}

	// Use this to update member variables when system resume.
	// Advice: avoid to update your families inside this function.
	protected override void onResume(int currentFrame) {
        if (currentFrame == 1) {
            this.Pause = true;
            return;
        }
    }

	// Use to process your families.
	protected override void onProcess(int familiesUpdateCount) {
        progress += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Escape)) {
            gamePaused = Convert.ToBoolean(1 - Convert.ToInt32(gamePaused));
        }
        if (gamePaused && !gameWasSuccessfullyPutOnPause) {
            foreach (FSystem system in FSystemManager.fixedUpdateSystems())
                if (Array.IndexOf(systemsToPause, system.GetType().Name) != -1)
                    system.Pause = true;
            foreach (FSystem system in FSystemManager.updateSystems())
                if (Array.IndexOf(systemsToPause, system.GetType().Name) != -1)
                    system.Pause = true;
            foreach (FSystem system in FSystemManager.lateUpdateSystems())
                if (Array.IndexOf(systemsToPause, system.GetType().Name) != -1)
                    system.Pause = true;
            foreach (FSystem system in FSystemManager.fixedUpdateSystems())
                if (Array.IndexOf(systemsToResume, system.GetType().Name) != -1)
                    system.Pause = false;
            foreach (FSystem system in FSystemManager.updateSystems())
                if (Array.IndexOf(systemsToResume, system.GetType().Name) != -1)
                    system.Pause = false;
            foreach (FSystem system in FSystemManager.lateUpdateSystems())
                if (Array.IndexOf(systemsToResume, system.GetType().Name) != -1)
                    system.Pause = false;

            //foreach (FSystem system in FSystemManager.fixedUpdateSystems())
            //    if (Array.IndexOf(systemsToPause, system.GetType().Name) != -1) {
            //        try {
            //            ((Level_Management_System)system).reinitOnResume = false;
            //        }
            //        catch { }
            //    }
            //foreach (FSystem system in FSystemManager.updateSystems())
            //    if (Array.IndexOf(systemsToPause, system.GetType().Name) != -1) {
            //        try {
            //            ((Level_Management_System)system).reinitOnResume = false;
            //        }
            //        catch { }
            //    }
            //foreach (FSystem system in FSystemManager.lateUpdateSystems())
            //    if (Array.IndexOf(systemsToPause, system.GetType().Name) != -1) {
            //        try {
            //            ((Level_Management_System)system).reinitOnResume = false;
            //        }
            //        catch { }
            //    }

            Movement_System.instance.reinitOnResume = false;
            // Spawn_System.instance.reinitOnResume = false;

            masterSceneElementsToPause = FamilyManager.getFamily(new AnyOfTags("Tower", "Respawn"));
            foreach (GameObject go in masterSceneElementsToPause)
                go.SetActive(false);
            masterSceneElementsToDestroyOnPause = FamilyManager.getFamily(new AnyOfTags("Particle"));
            foreach (GameObject go in masterSceneElementsToDestroyOnPause) {
                GameObjectManager.unbind(go);
                UnityEngine.Object.Destroy(go);
            }

            gameWasSuccessfullyPutOnPause = true;
            sceneReloaded = false;
            GameObjectManager.unloadScene(Load_Scene_System.instance.Get_Current_Scene_String());
            GameObjectManager.loadScene(pauseSceneName, LoadSceneMode.Additive);

        }
        else if (!gamePaused && gameWasSuccessfullyPutOnPause) {
            if (!sceneReloaded) {
                GameObjectManager.loadScene(Load_Scene_System.instance.Get_Current_Scene_String(), LoadSceneMode.Additive);
                GameObjectManager.unloadScene(pauseSceneName);
                sceneReloaded = true;
                progress = 0.0f;
                return;
            }
            if (progress < reload)
                return;
            foreach (FSystem system in FSystemManager.fixedUpdateSystems())
                if (Array.IndexOf(systemsToPause, system.GetType().Name) != -1)
                    system.Pause = false;
            foreach (FSystem system in FSystemManager.updateSystems())
                if (Array.IndexOf(systemsToPause, system.GetType().Name) != -1)
                    system.Pause = false;
            foreach (FSystem system in FSystemManager.lateUpdateSystems())
                if (Array.IndexOf(systemsToPause, system.GetType().Name) != -1)
                    system.Pause = false;
            foreach (FSystem system in FSystemManager.fixedUpdateSystems())
                if (Array.IndexOf(systemsToResume, system.GetType().Name) != -1)
                    system.Pause = true;
            foreach (FSystem system in FSystemManager.updateSystems())
                if (Array.IndexOf(systemsToResume, system.GetType().Name) != -1)
                    system.Pause = true;
            foreach (FSystem system in FSystemManager.lateUpdateSystems())
                if (Array.IndexOf(systemsToResume, system.GetType().Name) != -1)
                    system.Pause = true;

            foreach (GameObject go in masterSceneElementsToPause)
                go.SetActive(true);

            if (Movement_System.instance.afterSoftPauseResumed)
                Movement_System.instance.reinitOnResume = true;
            else
                return;

            //if (Spawn_System.instance.afterSoftPauseResumed)
            //    Spawn_System.instance.reinitOnResume = true;
            //else
            //    return;

            //foreach (FSystem system in FSystemManager.fixedUpdateSystems())
            //    if (Array.IndexOf(systemsToPause, system.GetType().Name) != -1) {
            //        try { 
            //            if (((Level_Management_System)system).afterSoftPauseResumed)
            //                ((Level_Management_System)system).reinitOnResume = true;
            //            else
            //                return;
            //        }
            //        catch { }
            //    }
            //foreach (FSystem system in FSystemManager.updateSystems())
            //    if (Array.IndexOf(systemsToPause, system.GetType().Name) != -1) {
            //        try { 
            //            if (((Level_Management_System)system).afterSoftPauseResumed)
            //                ((Level_Management_System)system).reinitOnResume = true;
            //            else
            //                return;
            //        }
            //        catch { }
            //    }
            //foreach (FSystem system in FSystemManager.lateUpdateSystems())
            //    if (Array.IndexOf(systemsToPause, system.GetType().Name) != -1) {
            //        try { 
            //            if (((Level_Management_System)system).afterSoftPauseResumed)
            //                ((Level_Management_System)system).reinitOnResume = true;
            //            else
            //                return;
            //        }
            //        catch { }
            //    }

            gameWasSuccessfullyPutOnPause = false;

            //debug.log("systems i want to pause");
            //foreach (string sname in systemstopause)
            //    debug.log(sname);
            //debug.log("systems i want to resume");
            //foreach (string sname in systemstoresume)
            //    debug.log(sname);
        }
    }
}