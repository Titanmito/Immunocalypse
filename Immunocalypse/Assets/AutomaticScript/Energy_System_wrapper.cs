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

}