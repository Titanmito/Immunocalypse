using UnityEngine;
using UnityEngine.SceneManagement;
using FYFY;
using UnityEngine.UI;

public class Lvl_Button_Control_System : FSystem {
	// Responsible for the activation/deactivation of buttons in level scenes, so that we can control what the player can use/build. 
	// This enables us to control the difficulty and complexity, and present each element in different levels.
	
	private Family _Level = FamilyManager.getFamily(new AllOfComponents(typeof(Spawn), typeof(Active_Lvl_Buttons)));
	private Family _Buttons = FamilyManager.getFamily(new AnyOfTags("Button"), new AllOfComponents(typeof(Button), typeof(Lvl_Buttons)), new NoneOfLayers(8, 9),
		new AllOfProperties(PropertyMatcher.PROPERTY.ACTIVE_IN_HIERARCHY));
	private Active_Lvl_Buttons ActButtons;

	// Use this to update member variables when system pause. 
	// Advice: avoid to update your families inside this function.
	protected override void onPause(int currentFrame) {
	}

	// Use this to update member variables when system resume.
	// Doing stuff here because there's no need to repeat the tests each frame (as in inside onProcess).
	protected override void onResume(int currentFrame){
		if (currentFrame == 1)
		{
			this.Pause = true;
			return;
		}
		_Level = FamilyManager.getFamily(new AllOfComponents(typeof(Spawn), typeof(Active_Lvl_Buttons)));
		_Buttons = FamilyManager.getFamily(new AnyOfTags("Button"), new AllOfComponents(typeof(Button), typeof(Lvl_Buttons)), new NoneOfLayers(8, 9),
				new AllOfProperties(PropertyMatcher.PROPERTY.ACTIVE_IN_HIERARCHY));

		ActButtons = _Level.First().GetComponent<Active_Lvl_Buttons>();
		
		foreach (GameObject go in _Buttons)
		{
			Lvl_Buttons lb = go.GetComponent<Lvl_Buttons>();
			switch (lb.button_nb)
			{
				case 0:
					go.SetActive(ActButtons.macro_button);
					break;
				case 10:
					go.SetActive(ActButtons.lymp_button);
					break;
				case 20:
					go.SetActive(ActButtons.anti_button);
					break;
				case 30:
					go.SetActive(ActButtons.vaci_button);
					break;
			}
		}


	}

	// Use to process your families.
	protected override void onProcess(int familiesUpdateCount) {

	}
}