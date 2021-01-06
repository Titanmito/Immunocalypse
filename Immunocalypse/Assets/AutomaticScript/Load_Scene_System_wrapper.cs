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

	public void Fin_Button(System.Int32 amount)
	{
		MainLoop.callAppropriateSystemMethod ("Load_Scene_System", "Fin_Button", amount);
	}

}
