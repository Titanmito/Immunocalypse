using UnityEngine;
using UnityEngine.UI;
using FYFY;
using System.Collections;

public class Special_Powers_Effects_System : FSystem 
{
	// This system controls the color pulse that happens when a player uses the antibiotique or vaccin special powers.
	private Family _SpeEffectGO = FamilyManager.getFamily(new AnyOfTags("Special"), new AllOfComponents(typeof(Image)));
	private Family _AntiTextGO = FamilyManager.getFamily(new AnyOfTags("Special"), new AllOfComponents(typeof(Text)));

	public Special_Powers_Effects_System()
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

		_SpeEffectGO = FamilyManager.getFamily(new AnyOfTags("Special"), new AllOfComponents(typeof(Image)));
		_AntiTextGO = FamilyManager.getFamily(new AnyOfTags("Special"), new AllOfComponents(typeof(Text)));
		MainLoop.instance.StartCoroutine(color_coroutine());
	}

	private IEnumerator color_coroutine()
	{
		for (; ; )
		{
			GameObject spe = _SpeEffectGO.First();
			GameObject text = _AntiTextGO.First();
			if (spe != null) 
			{ 
				Color color = spe.GetComponent<Image>().color;
				if (color.a > 0)
				{
					color.a -= 0.05f;
					spe.GetComponent<Image>().color = color;
				}
			}
			if (text != null)
            {
				Color color = text.GetComponent<Text>().color;
				if (color.a > 0)
				{
					color.a -= 0.025f;
					text.GetComponent<Text>().color = color;
				}
			}

			yield return new WaitForSeconds(0.1f);
		}

	}
}