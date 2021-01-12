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

	public void Lymp1_Button(System.Int32 amount)
	{
		MainLoop.callAppropriateSystemMethod ("Energy_System", "Lymp1_Button", amount);
	}

	public void Lymp2_Button(System.Int32 amount)
	{
		MainLoop.callAppropriateSystemMethod ("Energy_System", "Lymp2_Button", amount);
	}

	public void Anti_Button(System.Int32 amount)
	{
		MainLoop.callAppropriateSystemMethod ("Energy_System", "Anti_Button", amount);
	}

	public void Vaci_Button(System.Int32 amount)
	{
		MainLoop.callAppropriateSystemMethod ("Energy_System", "Vaci_Button", amount);
	}

	public void Des_Virus1_Button(System.Int32 type)
	{
		MainLoop.callAppropriateSystemMethod ("Energy_System", "Des_Virus1_Button", type);
	}

	public void Des_Virus2_Button(System.Int32 type)
	{
		MainLoop.callAppropriateSystemMethod ("Energy_System", "Des_Virus2_Button", type);
	}

	public void Des_Bacterie1_Button(System.Int32 type)
	{
		MainLoop.callAppropriateSystemMethod ("Energy_System", "Des_Bacterie1_Button", type);
	}

	public void Des_Bacterie2_Button(System.Int32 type)
	{
		MainLoop.callAppropriateSystemMethod ("Energy_System", "Des_Bacterie2_Button", type);
	}

	public void Des_Cancel_Button(System.Int32 type)
	{
		MainLoop.callAppropriateSystemMethod ("Energy_System", "Des_Cancel_Button", type);
	}

}
