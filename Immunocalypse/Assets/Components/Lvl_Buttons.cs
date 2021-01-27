using UnityEngine;

public class Lvl_Buttons : MonoBehaviour
{
	// Indicates which button present in levels we're dealing with.
	// 0 -> macro_button (macrophage tower)
	// 10 -> lymp1_button (lymphocyte1 tower, attacks virus)
	// 11 -> lymp2_button (lymphocyte2 tower, attacks bacterie)
	// 20 -> anti_button (antibiotique special power)
	// 30 -> vaci_button (vaccine special power)
	// 31 -> des_virus1 (uses the vaccine to the virus1 type of enemy)
	// 32 -> des_virus2 (uses the vaccine to the virus2 type of enemy)
	// 33 -> des_bacterie1 (uses the vacine to the bacterie1 type of enemy)
	// 34 -> des_bacterie2 (uses the vacine to the bacterie2 type of enemy)
	// 40 -> des_cancel (cancel the use of a vaccine. Only here because it has to have this componet so that it shows up, but it should always be true)
	// 50 -> back_from_lvl_button (the quit button on each level. Only here so that all buttons in a level have this component as the name of the component suggests)
	// 51 -> menu_from_lvl_button (the menu button on each level. Only here so that all buttons in a level have this component as the name of the component suggests)
	// 60 -> mute_sound_button (the sound button on each level. Only here so that all buttons in a level have this component as the name of the component suggests)
	// 61 -> mute_music_button (the music button on each level. Only here so that all buttons in a level have this component as the name of the component suggests)
	// theres space between each number so that we can create different types of macrophages and lymphocytes towers, as well as other enemies, if we have time (we wont lol).

	public int button_nb;
}