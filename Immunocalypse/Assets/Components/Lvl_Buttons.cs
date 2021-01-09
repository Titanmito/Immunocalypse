using UnityEngine;

public class Lvl_Buttons : MonoBehaviour {
	// Indicates which button present in levels we're dealing with.
	// 0 -> macro_button (macrophage tower)
	// 10 -> lymp_button (lymphocyte tower)
	// 20 -> anti_button (antibiotique special power)
	// 30 -> vaci_button (vaccine special power)
	// 31 -> des_virus (uses the vaccine to the virus type of enemy)
	// 32 -> des_bacterie (uses the vacine to the bacterie type of enemy)
	// 40 -> des_cancel (cancel the use of a vaccine. Only here because it has to have this componet so that it shows up, but it should always be true)
	// 50 -> back_from_lvl_button (the quit button on each level. Only here so that all buttons in a level have this component as the name of the component suggests)

	// theres space between each number so that we can create different types of macrophages and lymphocytes towers, as well as other enemies, if we have time (we wont).

	public int button_nb;
}