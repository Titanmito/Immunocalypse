using UnityEngine;
using UnityEngine.UI;
using FYFY;
using System.Collections;

public class Tower_Attack_Effects_System : FSystem {

	// This system controls the color pulse that happens when a tower attacks or creates anticorps.

	private Family _TowerGO = FamilyManager.getFamily(new AnyOfTags("Tower"), new AnyOfComponents(typeof(Price), typeof(Can_Attack)));
	private Family _Lymphocyte = FamilyManager.getFamily(new AllOfComponents(typeof(Anticorps_Factory), typeof(Lymphocyte)));

	public Tower_Attack_Effects_System()
	{
		// this.Pause = true;
	}

	protected override void onPause(int currentFrame)
	{
		// this.Pause = true;
	}

	// Use this to update member variables when system resume.
	// Advice: avoid to update your families inside this function.
	protected override void onResume(int currentFrame){
		// Debug.Log("System " + this.GetType().Name + " go on resume ; " + currentFrame.ToString());
		if (currentFrame == 1)
		{
			this.Pause = true;
			return;
		}
		_TowerGO = FamilyManager.getFamily(new AnyOfTags("Tower"), new AnyOfComponents(typeof(Price), typeof(Can_Attack)));
		_Lymphocyte = FamilyManager.getFamily(new AllOfComponents(typeof(Anticorps_Factory), typeof(Lymphocyte)));
		MainLoop.instance.StartCoroutine(color_coroutine());
	}

	private IEnumerator color_coroutine()
	{
		for (; ; )
		{
			foreach (GameObject go in _TowerGO)
            {
				SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
				if (sr.color.g < 1)
                {
					Color c = sr.color;
					c.g += 0.05f;
					c.b += 0.05f;
					go.GetComponent<SpriteRenderer>().color = c;

				}
            }
            foreach(GameObject go in _Lymphocyte){
				SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
				if (sr.color.g < 1)
				{
					Color c = sr.color;
					c.g += 0.02f;
					go.GetComponent<SpriteRenderer>().color = c;
				}
			}
			yield return new WaitForSeconds(0.1f);
		}

	}
}