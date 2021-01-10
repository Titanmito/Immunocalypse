using UnityEngine;
using FYFY;

public class Level_Management_System : FSystem {
    public bool reinitOnResume = true;
    public bool afterSoftPauseResumed = false;

    protected override void onPause(int currentFrame) {
        if (!reinitOnResume)
            afterSoftPauseResumed = false;
    }
}