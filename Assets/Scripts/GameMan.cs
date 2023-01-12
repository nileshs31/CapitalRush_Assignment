using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameMan : MonoBehaviour
{
    [SerializeField]
    GameObject thief, policePrefab, lineRendPrefab;
    GameObject lineRendGO;

    public void SpawnPolice()
    {
        if (lineRendGO != null)
        {
            Destroy(lineRendGO);
        }
        GameObject[] policeGos;
        policeGos = GameObject.FindGameObjectsWithTag("Police");

        if (policeGos.Length != 0)
        {
            foreach(GameObject go in policeGos)
            {
                Destroy(go);
            }
        }

        // --- METHOD 1 -----

        /*int policeCount = 0;
        while (policeCount < 10){
            Vector3 randomPoint = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
            Collider[] colliders = Physics.OverlapSphere(randomPoint, 0.3f);
            Debug.Log(colliders);
            if (colliders.Length == 0)
            {
                Instantiate(policePrefab, randomPoint, Quaternion.identity);
                policeCount++;
            }
        }*/

        // --- METHOD 2 ----- Both written by Nilesh Sharma

        int policeCount = 0;
        while (policeCount < 10)
        {
            Vector3 randomPoint = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
            if (!Physics.CheckSphere(randomPoint, 0.35f))
            {
                Instantiate(policePrefab, randomPoint, Quaternion.identity);
                policeCount++;
            }
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    public void FindDirection()// -------- Written by Nilesh Sharma -------------
    {
        List<int> angles = new List<int>();
        int ang = 0;
        for (int i = 0; i < 36; i++)
        {
            Quaternion rotation = Quaternion.AngleAxis(ang, transform.up);
            var ray = new Ray(thief.transform.position, rotation * thief.transform.forward * 8);
            RaycastHit hit;
            if(!Physics.Raycast(ray, out hit))
            {
                angles.Add(ang);
            }
            ang += 10;
        }

        List<int> angles2 = new List<int>();
        for(int i = 0; i < angles.Count; i++)
        {
            if (i != 0 && i != angles.Count - 1)
            {
                if(angles[i-1]== angles[i]-10 && angles[i + 1] == angles[i] + 10)
                {
                    if (!angles2.Contains(angles[i-1]))
                    {
                        angles2.Add(angles[i-1]);
                    }
                    if (!angles2.Contains(angles[i]))
                    {
                        angles2.Add(angles[i]);
                    }
                    if (!angles2.Contains(angles[i+1]))
                    {
                        angles2.Add(angles[i+1]);
                    }
                }
            }
        }

        int k = 0;
        List<List<int>> conseArr = new List<List<int>>();
        List<int> conseArrLen = new List<int>();
        while (k< angles2.Count-1)
        {
            List<int> temp = new List<int>();
            temp.Add(angles2[k]);
            var first_ele = angles2[k];
            for (int j = k + 1; j < angles2.Count; j++) {
                if (first_ele + 10 == angles2[j])
                {
                    temp.Add(angles2[j]);
                    first_ele = angles2[j];
                    k = j;
                }
                else
                    break;
            }
            conseArr.Add(temp);
            conseArrLen.Add(temp.Count);
            k += 1;
        }

        float bestAngle;
        if (conseArrLen.Count>0) // there are consecutive directions!
        {
            int maxValue = Mathf.Max(conseArrLen.ToArray());
            int maxIndex = conseArrLen.IndexOf(maxValue);

            bestAngle = (float)conseArr[maxIndex].ToArray().Average();
        }
        else
        {
            bestAngle = angles[0];
        }

        Quaternion rot = Quaternion.AngleAxis(bestAngle, transform.up);
        //Debug.DrawRay(thief.transform.position, rot * thief.transform.forward * 8, Color.green, 60);

        var ray2 = new Ray(thief.transform.position, rot * thief.transform.forward * 8);
        var pos = ray2.GetPoint(8f);
        //Debug.Log(pos);
        if (lineRendGO != null)
        {
            Destroy(lineRendGO);
        }
        lineRendGO = Instantiate(lineRendPrefab, pos, Quaternion.identity);
        var lr = lineRendGO.GetComponent<LineRenderer>();
        lr.SetPosition(0, lineRendGO.transform.position);
        lr.SetPosition(1, thief.transform.position);
    }

    private void OnDrawGizmos()  // to see all the angles in editor
    {
        int ang = 0;
        for (int i = 0; i < 36; i++)
        {
            Quaternion rotation = Quaternion.AngleAxis(ang, transform.up);
            Debug.DrawRay(thief.transform.position, rotation * thief.transform.forward * 8, Color.red, 60);
            ang += 10;
        }
    }
}
