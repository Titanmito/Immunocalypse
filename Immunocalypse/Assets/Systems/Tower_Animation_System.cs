using UnityEngine;
using FYFY;
using System.Collections;

public class Tower_Animation_System : FSystem {

    private Family _TowerGO = FamilyManager.getFamily(new AnyOfTags("Tower"), new AllOfComponents(typeof(Price)), new NoneOfComponents(typeof(Lifespan)));

    public Tower_Animation_System()
    {
		MainLoop.instance.StartCoroutine(animation_coroutine());
    }

	private IEnumerator animation_coroutine()
    {
        Vector3 scaleChange = new Vector3(0.005f, 0.005f, 0.005f);

        int i = 0;
        int j = 0;
		for( ; ; )
        {
            foreach (GameObject go in _TowerGO)
            {
                if (j % 2 == 0)
                {
                    go.transform.localScale += scaleChange;
                }
                else
                {
                    go.transform.localScale -= scaleChange;
                }
                j += 1;
            }
            j = 0;
            i += 1;
            if (i >= 6)
            {
                scaleChange *= -1;
            }

            i = i % 6;
            yield return new WaitForSeconds(0.1f);
        }
    }

}