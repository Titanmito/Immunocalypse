using UnityEngine;
using FYFY;

[ExecuteInEditMode]
public class Energy_System_wrapper : MonoBehaviour
{
	private void Start()
	{
		this.hideFlags = HideFlags.HideInInspector; // Hide this component in Inspector
	}

	public void Macro_Button(System.Int32 amount)
	{
		MainLoop.callAppropriateSystemMethod ("Energy_System", "Macro_Button", amount);
	}

	public void Lymp_Button(System.Int32 amount)
	{
		MainLoop.callAppropriateSystemMethod ("Energy_System", "Lymp_Button", amount);
	}

	public void Anti_Button(System.Int32 amount)
	{
		MainLoop.callAppropriateSystemMethod ("Energy_System", "Anti_Button", amount);
	}

	public void Vaci_Button(System.Int32 amount)
	{
		MainLoop.callAppropriateSystemMethod ("Energy_System", "Vaci_Button", amount);
	}

	public void Des_Virus_Button(System.Int32 type)
	{
		MainLoop.callAppropriateSystemMethod ("Energy_System", "Des_Virus_Button", type);
	}

	public void Des_Bacterie_Button(System.Int32 type)
	{
		MainLoop.callAppropriateSystemMethod ("Energy_System", "Des_Bacterie_Button", type);
	}

	public void Des_Cancel_Button(System.Int32 type)
	{
		MainLoop.callAppropriateSystemMethod ("Energy_System", "Des_Cancel_Button", type);
	}

}
