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

	public void Fin_Button(System.Int32 amount)
	{
		MainLoop.callAppropriateSystemMethod ("Load_Scene_System", "Fin_Button", amount);
	}

}
