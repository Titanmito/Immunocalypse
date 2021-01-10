using UnityEngine;
using FYFY;
using System.Collections;

public class Tower_Animation_System : FSystem {

    private Family _TowerGO = FamilyManager.getFamily(new AnyOfTags("Tower"), new AnyOfComponents(typeof(Price), typeof(Can_Attack)));

    public Tower_Animation_System()
    {
        // this.Pause = true;
    }

    protected override void onPause(int currentFrame)
    {
        // Debug.Log("System " + this.GetType().Name + " go on pause");
    }

    protected override void onResume(int currentFrame)
    {
        // Debug.Log("System " + this.GetType().Name + " go on resume ; " + currentFrame.ToString());
        if (currentFrame == 1)
        {
            this.Pause = true;
            return;
        }

        _TowerGO = FamilyManager.getFamily(new AnyOfTags("Tower"), new AllOfComponents(typeof(Price)), new AllOfProperties(PropertyMatcher.PROPERTY.ACTIVE_IN_HIERARCHY));
        MainLoop.instance.StartCoroutine(animation_coroutine());
    }

    private IEnumerator animation_coroutine()
    {
        Vector3 scaleChange = new Vector3(0.005f, 0.005f, 0.005f);
        float rotationDegree = 0.4f;

        int i = 0;
        int j = 0;
        int k = 0;
        int l = 0;

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
            foreach (GameObject go in _TowerGO)
            {
                if (l % 2 == 0)
                {
                    go.transform.eulerAngles += new Vector3(0, 0, 1) * rotationDegree;
                }
                else
                {
                    go.transform.eulerAngles -= new Vector3(0, 0, 1) * rotationDegree;

                }
                l += 1;
            }
            l = 0;
            k += 1;
            if (k >= 20)
            {
                rotationDegree *= -1;
            }
            k = k % 20;
            yield return new WaitForSeconds(0.1f);
        }
    }

}