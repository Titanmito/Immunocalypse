using UnityEngine;
using UnityEngine.UI;
using FYFY;
using System.Collections;

public class Vaccine_Effect_System : FSystem 
{
	private Family _VaciEffectGO = FamilyManager.getFamily(new AnyOfTags("Special"), new AllOfComponents(typeof(Image)));

	public Vaccine_Effect_System()
	{
		// this.Pause = true;
	}

	// Use this to update member variables when system pause. 
	// Advice: avoid to update your families inside this function.
	protected override void onPause(int currentFrame)
    {
		// this.Pause = true;
	}

	// Use this to update member variables when system resume.
	// Advice: avoid to update your families inside this function.
	protected override void onResume(int currentFrame)
	{
		// Debug.Log("System " + this.GetType().Name + " go on resume ; " + currentFrame.ToString());
		if (currentFrame == 1)
		{
			this.Pause = true;
			return;
		}

		_VaciEffectGO = FamilyManager.getFamily(new AnyOfTags("Special"), new AllOfComponents(typeof(Image)));
		MainLoop.instance.StartCoroutine(color_coroutine());
	}

	private IEnumerator color_coroutine()
	{
		for (; ; )
		{
			GameObject vaci = _VaciEffectGO.First();
			if (vaci != null) 
			{ 

				Color color = vaci.GetComponent<Image>().color;
				if (color.a > 0)
				{
					color.a -= 0.05f;
					vaci.GetComponent<Image>().color = color;
				}
			}

			yield return new WaitForSeconds(0.1f);
		}

	}
}