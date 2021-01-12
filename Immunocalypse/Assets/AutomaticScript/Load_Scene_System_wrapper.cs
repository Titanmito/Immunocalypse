using UnityEngine;
using FYFY;

[ExecuteInEditMode]
public class Load_Scene_System_wrapper : MonoBehaviour
{
	private void Start()
	{
		this.hideFlags = HideFlags.HideInInspector; // Hide this component in Inspector
	}

	public void Start_Button(System.Int32 amount)
	{
		MainLoop.callAppropriateSystemMethod ("Load_Scene_System", "Start_Button", amount);
	}

	public void Help_Button(System.Int32 amount)
	{
		MainLoop.callAppropriateSystemMethod ("Load_Scene_System", "Help_Button", amount);
	}

	public void Encyclo_Button(System.Int32 amount)
	{
		MainLoop.callAppropriateSystemMethod ("Load_Scene_System", "Encyclo_Button", amount);
	}

	public void Back_Button(System.Int32 obj)
	{
		MainLoop.callAppropriateSystemMethod ("Load_Scene_System", "Back_Button", obj);
	}

	public void Next_Button(System.Int32 obj)
	{
		MainLoop.callAppropriateSystemMethod ("Load_Scene_System", "Next_Button", obj);
	}

	public void Dropdown(System.Int32 choix)
	{
		MainLoop.callAppropriateSystemMethod ("Load_Scene_System", "Dropdown", choix);
	}

	public void Return_To_Menu_From_Fin_Button(System.Int32 amount)
	{
		MainLoop.callAppropriateSystemMethod ("Load_Scene_System", "Return_To_Menu_From_Fin_Button", amount);
	}

	public void Next_Level_Button(System.Int32 amount)
	{
		MainLoop.callAppropriateSystemMethod ("Load_Scene_System", "Next_Level_Button", amount);
	}

	public void Play_Again_Button(System.Int32 actual_lvl)
	{
		MainLoop.callAppropriateSystemMethod ("Load_Scene_System", "Play_Again_Button", actual_lvl);
	}

	public void Continue_Button(System.Int32 amount)
	{
		MainLoop.callAppropriateSystemMethod ("Load_Scene_System", "Continue_Button", amount);
	}

	public void Replay_Pause_Button(System.Int32 amount)
	{
		MainLoop.callAppropriateSystemMethod ("Load_Scene_System", "Replay_Pause_Button", amount);
	}

	public void Return_To_Menu_From_Pause_Button(System.Int32 amount)
	{
		MainLoop.callAppropriateSystemMethod ("Load_Scene_System", "Return_To_Menu_From_Pause_Button", amount);
	}

	public void Exit_From_Pause_Button(System.Int32 amount)
	{
		MainLoop.callAppropriateSystemMethod ("Load_Scene_System", "Exit_From_Pause_Button", amount);
	}

	public void Back_From_Lvl_Button(System.Int32 amount)
	{
		MainLoop.callAppropriateSystemMethod ("Load_Scene_System", "Back_From_Lvl_Button", amount);
	}

	public void Menu_From_Lvl_Button(System.Int32 amount)
	{
		MainLoop.callAppropriateSystemMethod ("Load_Scene_System", "Menu_From_Lvl_Button", amount);
	}

}
